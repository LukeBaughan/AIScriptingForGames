using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMakingEntity : MovingEntity
{

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Pursuit m_Pursuit;
    SteeringBehaviour_Evade m_Evade;
    SteeringBehaviour_Seek m_Seek;

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

    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    // Update is called once per frame
    void Update()
    {
        // Persues Player
        // If player is aggresive, evade
        // If low on health, seek pickup
        /*
         * if(health < x)
         * {
         *      m_Seek.m_TargetPosition = closest health pickup;
         * }
        */

    }
}
