using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Manager : MonoBehaviour
{
    public MovingEntity m_Entity { get; private set; }
    public float m_MaxForce = 100;
    public float m_RemainingForce;
    public List<SteeringBehaviour> m_SteeringBehaviours;

	private void Awake()
    {

        // The entitiy that it is attached to
        m_Entity = GetComponent<MovingEntity>();

        if(!m_Entity)
            Debug.LogError("Steering Behaviours only working on type moving entity", this);
    }

	public Vector2 GenerateSteeringForce()
    {
        // Returns the force of the first steering behaviour in the list (if the list isn't empty)
        if(m_SteeringBehaviours.Count > 0)
        {
            return m_SteeringBehaviours[0].CalculateForce();
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void EnableExclusive(SteeringBehaviour behaviour)
	{
        if(m_SteeringBehaviours.Contains(behaviour))
		{
            foreach(SteeringBehaviour sb in m_SteeringBehaviours)
			{
                sb.m_Active = false;
			}

            behaviour.m_Active = true;
		}
        else
		{
            Debug.Log(behaviour + " doesn't not exist on object", this);
		}
	}
    public void DisableAllSteeringBehaviours()
    {
        foreach (SteeringBehaviour sb in m_SteeringBehaviours)
        {
            sb.m_Active = false;
        }
    }
}
