using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

public class DecisionMakingEntity : MovingEntity
{

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Wander m_Wander;
    SteeringBehaviour_Pursuit m_Pursuit;
    SteeringBehaviour_Evade m_Evade;
    SteeringBehaviour_Seek m_Seek;
    Pathfinding_AStar m_AStar = new Pathfinding_AStar(true, false);

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

    public float m_FOV;
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
                // Pursues the player if they are within range and are not behind a wall
                if (getPlayerInRange() && !getPlayerBehindWall())
                    m_CurrentState = state.pursuePlayer;
                break;
            case state.pursuePlayer:
                activateBehaviour(state.pursuePlayer);
                // If the player is not in the pursue range, return to wandering
                if (!getPlayerInRange())
                    m_CurrentState = state.wander;
                // If the player is behind a wall, go to their last known position
                else if (getPlayerBehindWall())
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
                // If the player is in range and is not behind a wall, pursue them and reset the a star algorithm
                if (getPlayerInRange() && !getPlayerBehindWall())
                {
                    m_AStar = new Pathfinding_AStar(false, false);
                    m_CurrentState = state.pursuePlayer;
                }
                else
                {
                    // If the entity has reached the player's last seen position, return to wandering
                    if (Maths.Magnitude(new Vector2(transform.position.x, transform.position.y) - m_LastSeenPlayerPosition) < 0.75)
                    {
                        m_AStar = new Pathfinding_AStar(false, false);
                        m_CurrentState = state.wander;
                    }
                    // If the entity hasn't reached the player's last seen position, move to it
                    else
                    {
                        activateBehaviour(state.goToLastSeenPosition);
                        AStarPath(m_LastSeenPlayerPosition);
                        //m_Seek.m_TargetPosition = m_LastSeenPlayerPosition;
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
                        // Uses A star to find and seek the closest health pickup
                        activateBehaviour(state.seekHealth);
                        AStarPath(m_closestHealthPickup.transform.position);
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
        // Activates the specified behaviour
        switch (behaviour)
        {
            case state.wander:
                m_SteeringBehaviours.EnableExclusive(m_Wander);
                break;
            case state.pursuePlayer:
                m_SteeringBehaviours.EnableExclusive(m_Pursuit);
                break;
            case state.goToLastSeenPosition:
                m_SteeringBehaviours.EnableExclusive(m_Seek);
                break;
            case state.seekHealth:
                m_SteeringBehaviours.EnableExclusive(m_Seek);
                break;
            case state.evadePlayer:
                m_SteeringBehaviours.EnableExclusive(m_Evade);
                break;
        }
    }

    private void AStarPath(Vector2 targetPosition)
    {
        // Uses the A Star pathfinding to generate a path to the target position
        if (m_AStar.m_Path.Count == 0)
        {
            m_AStar.GeneratePath(Grid.GetNodeClosestWalkableToLocation(transform.position), Grid.GetNodeClosestWalkableToLocation(targetPosition));
        }
        else
        {
            if (m_AStar.m_Path.Count > 0)
            {
                Vector2 closestPoint = m_AStar.GetClosestPointOnPath(transform.position);

                if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.6f)
                    closestPoint = m_AStar.GetNextPointOnPath(transform.position);

                m_Seek.m_TargetPosition = closestPoint;
            }
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

    public bool getPlayerBehindWall()
    {
        // Performs a raycast between the entity and the player
        RaycastHit2D[] entities = Physics2D.LinecastAll(transform.position, m_PlayerMovingEntity.transform.position);

        foreach (RaycastHit2D hit in entities)
        {
            // If a wall is hit in the raycast, then return true
            if (hit.transform.GetComponent<TilemapCollider2D>() != null)
            {
                return true;
            }
        }
        // If there is no wall between the entity and the player, then return false
        return false;

    }

    public bool getPlayerInFOV() // Not in use - Didn't work well with the randomness of wander
    {
        // Checks if the dot product of the entity's position to the player's position is greater than whatever the FOV is set to
        float dotProduct = Maths.Dot(m_SteeringBehaviours.m_Entity.m_Velocity, m_PlayerMovingEntity.transform.position);
        Debug.Log(dotProduct);

        if (dotProduct > m_FOV)
            return true;

        // If the player is out of the entity's FOV, return false
        return false;

    }

    public void onHealthPickedUp()
    {
        // Resets the A star pathfinding algorithm
        m_HealthPickedUp = true;
        m_AStar = new Pathfinding_AStar(false, false);
    }
}
