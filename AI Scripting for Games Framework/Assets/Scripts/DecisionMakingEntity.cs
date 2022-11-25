using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMakingEntity : MovingEntity
{

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Pursuit m_Pursuit;
    SteeringBehaviour_Evade m_Evade;
    SteeringBehaviour_Seek m_Seek;

    enum state
    {
        pursuePlayer,
        seekHealth
    }

    state m_CurrentState;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

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
        m_Pursuit.m_PursuingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
        m_Evade.m_EvadingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();

        // Only pursuit needs to be active on start since the enemy will be pursuing the player
        m_Evade.m_Active = false;
        m_Seek.m_Active = false;

        m_CurrentState = state.pursuePlayer;

    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentHealth < (m_MaxHealth * 0.25f))
        {
            m_CurrentState = state.seekHealth;
        }
        else
        {
            m_CurrentState = state.pursuePlayer;
        }
        // Persues Player
        // If player is aggresive, evade
        // If low on health, seek pickup

        // Use a switch statement for different states
        switch (m_CurrentState)
        {
            case state.pursuePlayer:
                stopAllBehaviours();
                m_Seek.m_Active = false;
                m_Pursuit.m_Active = true;
                break;
            case state.seekHealth:
                stopAllBehaviours();
                //HealthPickup healthPickups = FindObjectOfType<HealthPickup>(); // Expensive?
                Vector2 healthLocation = new(-5.159f, -5.039f);
                m_Pursuit.m_Active = false;
                m_Seek.m_Active = true;
                m_Seek.m_TargetPosition = healthLocation;
                break;
        }
    }
    void stopAllBehaviours()
    {
    }
}
