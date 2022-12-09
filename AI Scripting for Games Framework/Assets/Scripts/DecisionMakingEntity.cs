using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DecisionMakingEntity : MovingEntity
{

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Wander m_Wander;
    SteeringBehaviour_Pursuit m_Pursuit;
    SteeringBehaviour_Evade m_Evade;
    SteeringBehaviour_Seek m_Seek;
    Pathfinding_AStar m_AStar = new Pathfinding_AStar(false, false);

    MovingEntity m_PlayerMovingEntity;

    enum state
    {
        wander,
        pursuePlayer,
        goToLastSeenPosition,
        seekHealth,
        evadePlayer
    }

    state m_CurrentState;

    Vector2 m_LastSeenPlayerPosition;

    HealthPickup m_closestHealthPickup;
    bool m_HealthPickedUp = false;

    public float m_DetectionRange;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Wander = GetComponent<SteeringBehaviour_Wander>();
        if (!m_Wander)
            Debug.LogError("Object doesn't have a Wander Steering Behaviour attached", this);

        m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Pursuit Steering Behaviour attached", this);

        m_Evade = GetComponent<SteeringBehaviour_Evade>();
        if (!m_Evade)
            Debug.LogError("Object doesn't have a Evade Steering Behaviour attached", this);

        m_Seek = GetComponent<SteeringBehaviour_Seek>();
        if (!m_Seek)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
    }


    // Start is called before the first frame update
    void Start()
    {
        // Gets the player (to be used for raycasting)
        m_PlayerMovingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();

        // Sets the player to the player to pursue and avoid (depending on the current state)
        m_Pursuit.m_PursuingEntity = m_PlayerMovingEntity;
        m_Evade.m_EvadingEntity = m_PlayerMovingEntity;
        m_Evade.m_EvadeRadius = m_DetectionRange;

        // Sets the initial state to wander
        m_CurrentState = state.wander;

    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    // Update is called once per frame
    void Update()
    {
        // Use a switch statement for different states
        switch (m_CurrentState)
        {
            case state.wander:
                activateBehaviour(state.wander);
                // Pursues the player if they are within range and are visible
                if (getPlayerInRange() && getPlayerVisable())
                    m_CurrentState = state.pursuePlayer;
                break;
            case state.pursuePlayer:
                activateBehaviour(state.pursuePlayer);
                // If the player is not in the pursue range, return to wandering
                if (!getPlayerInRange())
                    m_CurrentState = state.wander;
                // If the player is behind a wall, go to their last known position
                else if (!getPlayerVisable())
                {
                    m_LastSeenPlayerPosition = new Vector2(m_PlayerMovingEntity.transform.position.x, m_PlayerMovingEntity.transform.position.y);
                    m_CurrentState = state.goToLastSeenPosition;
                }
                // If health gets too low, seek a health pack
                if (m_CurrentHealth < (m_MaxHealth * 0.25f))
                {
                    m_CurrentState = state.seekHealth;
                }
                break;
            case state.goToLastSeenPosition:
                // If the player is in range and is visible, pursue them
                if (getPlayerInRange() && getPlayerVisable())
                    m_CurrentState = state.pursuePlayer;
                else
                {
                    // If the entity has reached the player's last seen position, return to wandering
                    if (Maths.Magnitude(new Vector2(transform.position.x, transform.position.y) - m_LastSeenPlayerPosition) < 0.1)
                        m_CurrentState = state.wander;
                    // If the entity hasn't reached the player's last seen position, move to it
                    else
                    {
                        activateBehaviour(state.goToLastSeenPosition);
                        m_Seek.m_TargetPosition = m_LastSeenPlayerPosition;
                    }
                }
                break;
            case state.seekHealth:
                if (!m_HealthPickedUp)
                {
                    float closestDistance = float.MaxValue;
                    HealthPickup[] healthPickups = FindObjectsOfType<HealthPickup>();
                    if (healthPickups.Length > 0)
                    {
                        // Finds the closest health pickup
                        foreach (HealthPickup healthPickup in healthPickups)
                        {
                            float healthDistance = Vector3.Distance(healthPickup.transform.position, transform.position);
                            if (healthDistance < closestDistance)
                            {
                                closestDistance = healthDistance;
                                m_closestHealthPickup = healthPickup;
                            }
                        }
                        // Seeks the closest health pickup
                        activateBehaviour(state.seekHealth);
                        m_Seek.m_TargetPosition = m_closestHealthPickup.transform.position;
                        Debug.Log(m_AStar.m_Path.Count);
                        if (m_AStar.m_Path.Count == 0)
                        {
                            m_AStar.GeneratePath(Grid.GetNodeClosestWalkableToLocation(transform.position), Grid.GetNodeClosestWalkableToLocation(m_closestHealthPickup.transform.position));
                        }
                        else
                        {
                            if (m_AStar.m_Path.Count > 0)
                            {
                                Vector2 closestPoint = m_AStar.GetClosestPointOnPath(transform.position);

                                if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
                                    closestPoint = m_AStar.GetNextPointOnPath(transform.position);

                                m_Seek.m_TargetPosition = closestPoint;
                            }
                        }
                    }
                    else
                    {
                        // If no health pickup is found, evade the player
                        m_CurrentState = state.evadePlayer;
                    }
                }
                else
                {
                    // If the health has been picked up, continue pursuing the player
                    m_HealthPickedUp = false;
                    m_CurrentState = state.pursuePlayer;
                }
                break;
            case state.evadePlayer:
                activateBehaviour(state.evadePlayer);
                // If the player is out of range, return to wandering
                if (!getPlayerInRange())
                    m_CurrentState = state.wander;
                break;
        }
    }

    void activateBehaviour(state behaviour)
    {
        //USE THE FUNCTIONS:
        //m_SteeringBehaviours.DisableAllSteeringBehaviours();
        //m_SteeringBehaviours.EnableExclusive(m_Seek);
        // Deactivates all beahviours
        m_Wander.m_Active = false;
        m_Pursuit.m_Active = false;
        m_Seek.m_Active = false;
        m_Evade.m_Active = false;

        // Activates the specified behaviour
        switch (behaviour)
        {
            case state.wander:
                m_Wander.m_Active = true;
                break;
            case state.pursuePlayer:
                m_Pursuit.m_Active = true;
                break;
            case state.goToLastSeenPosition:
                m_Seek.m_Active = true;
                break;
            case state.seekHealth:
                m_Seek.m_Active = true;
                break;
            case state.evadePlayer:
                m_Evade.m_Active = true;
                break;
        }
    }

    public bool getPlayerInRange()
    {
        // Gets all nearby entities (within a cricle where radius = m_PursueRange)
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_DetectionRange);

        foreach (Collider2D entity in entities)
        {
            // Returns true if the player is within the overlap circle
            if (entity.GetComponent<Player>())
                return true;
        }
        return false;
    }

    public bool getPlayerVisable()
    {
        // Performs a raycast between the entity and the player
        RaycastHit2D[] entities = Physics2D.LinecastAll(transform.position, m_PlayerMovingEntity.transform.position);

        // If a wall is hit in the raycast, then return false
        foreach (RaycastHit2D hit in entities)
        {
           if(hit.transform.GetComponent<TilemapCollider2D>() != null)
            {
                return false;
            }
        }
        // If there is no wall between the entity and the player, then return true
        return true;
    }

    public void onHealthPickedUp()
    {
        m_HealthPickedUp = true;
    }
}
