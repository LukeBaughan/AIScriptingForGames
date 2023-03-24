using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAI : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Pursuit m_Pursuit;

	public UnityEvent m_OnDead;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
	}

	public void Initialise()
	{
        if (m_OnDead != null)
            m_OnDead = new UnityEvent();
    }

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
		if(m_CurrentHealth <= 0)
			m_OnDead.Invoke();
    }
}
