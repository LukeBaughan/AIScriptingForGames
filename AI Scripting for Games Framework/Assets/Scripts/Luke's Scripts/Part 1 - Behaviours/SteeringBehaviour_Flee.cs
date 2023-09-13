using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Flee : SteeringBehaviour
{
    public Transform m_FleeTarget;
    public float m_FleeRadius;

    public override Vector2 CalculateForce()
    {
        // Gets the vector between the entity and the flee target
        Vector2 fleePositionVector = (transform.position - m_FleeTarget.position);
        // Gets the unit vector of the fleePositionVector and multiplies it by the entity's max speed to get the desired velocity
        m_DesiredVelocity = Maths.Normalise(fleePositionVector) * m_Manager.m_Entity.m_MaxSpeed;
        // Seek force = desired velocity - current velocity of the entity
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        // Returns a unit vector of m_Steering multiplied by the weight (which scales with the flee radius)
        return Maths.Normalise(m_Steering) * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(fleePositionVector), m_FleeRadius) / m_FleeRadius);
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, m_FleeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
