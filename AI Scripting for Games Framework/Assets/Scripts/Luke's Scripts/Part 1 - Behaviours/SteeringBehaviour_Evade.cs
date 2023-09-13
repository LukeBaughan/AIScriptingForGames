using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Evade : SteeringBehaviour
{
    public MovingEntity m_EvadingEntity;
    public float m_EvadeRadius;

    public override Vector2 CalculateForce()
    {
        if (m_EvadingEntity)
        {
            // Gets the vector between the entity and the pursue target
            Vector2 evadePositionVector = m_EvadingEntity.transform.position - transform.position;
            // Combines the speeds of both entities (the magnitude of a velocity vector is speed)
            float combinedSpeeds = (m_Manager.m_Entity.m_MaxSpeed + m_EvadingEntity.m_MaxSpeed);
            // PredictionTime = distance between two entities / (they combined speeds of the entities)
            float predictionTime = Maths.Magnitude(evadePositionVector) / combinedSpeeds;

            // Using the Flee code - Will change in the future to calling the Flee function
            Vector2 fleePosition = new Vector2(m_EvadingEntity.transform.position.x, m_EvadingEntity.transform.position.y)
                + m_EvadingEntity.m_Velocity * predictionTime;

            // Gets the vector between the entity and the flee target
            Vector2 fleePositionVector = new Vector2(transform.position.x, transform.position.y) - fleePosition;
            // Gets the unit vector of the fleePositionVector and multiplies it by the entity's max speed to get the desired velocity
            m_DesiredVelocity = Maths.Normalise(fleePositionVector) * m_Manager.m_Entity.m_MaxSpeed;
            // Seek force = desired velocity - current velocity of the entity
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            // Returns a unit vector of m_Steering multiplied by the weight (which scales with the flee radius)
            return Maths.Normalise(m_Steering) * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(fleePositionVector), m_EvadeRadius) / m_EvadeRadius);
        }
        else
        {
            return Vector2.zero;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, m_EvadeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
