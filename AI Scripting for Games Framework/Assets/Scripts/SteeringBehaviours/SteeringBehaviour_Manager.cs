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
        m_RemainingForce = m_MaxForce;
        Vector2 combinedForce = Vector2.zero;
        
        foreach(SteeringBehaviour sb in m_SteeringBehaviours)
        {
            // Checks if the steering behaviour has a force and is also active
            if(sb.CalculateForce() != Vector2.zero && sb.m_Active == true)
            {
                Vector2 tempForce = sb.CalculateForce();
                // If the magnitude of the current force is larger than the maximum force, cap it to the max force
                // (remaining force = max force at this point)
                if (Maths.Magnitude(tempForce) > m_RemainingForce)
                {
                    tempForce = Maths.Normalise(tempForce) * m_RemainingForce;
                }
                // Reduce the remaining force by the magnitude of the temp force
                m_RemainingForce -= Maths.Magnitude(tempForce);
                // Adds the currennt steering behaviour force to the combined force
                combinedForce += tempForce;
            }
        }

        // Returns the combination of all of the steering behaviour forces (capped at maximum force)
        return combinedForce;
    }

    //// Old GenerateSteeringForce code
    //// Returns the force of the first steering behaviour in the list (if the list isn't empty)
    //if(m_SteeringBehaviours.Count > 0)
    //{
    //    return m_SteeringBehaviours[0].CalculateForce();
    //}
    //else
    //{
    //    return Vector2.zero;
    //}

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
