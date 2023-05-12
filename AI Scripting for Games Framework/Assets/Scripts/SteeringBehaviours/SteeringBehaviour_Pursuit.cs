using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Pursuit : SteeringBehaviour
{
    public MovingEntity m_PursuingEntity;
    public float m_SafeDistance = 0.0f;

    public override Vector2 CalculateForce()
    {
        if (m_PursuingEntity)
        {
            // Gets the vector between the entity and the pursue target
            Vector2 pursuePositionVector = m_PursuingEntity.transform.position - transform.position;
            pursuePositionVector = pursuePositionVector - new Vector2(m_SafeDistance, m_SafeDistance);
            // Combines the speeds of both entities (the magnitude of a velocity vector is speed)
            float combinedSpeeds = Maths.Magnitude(m_PursuingEntity.m_Velocity) + Maths.Magnitude(m_Manager.m_Entity.m_Velocity);
            // PredictionTime = distance between two entities / (they combined speeds of the entities)
            float predictionTime = Maths.Magnitude(pursuePositionVector) / combinedSpeeds;


            // Using the Seek code - Will change in the future to calling the seek function
            Vector2 m_TargetPosition = (new Vector2(m_PursuingEntity.transform.position.x, m_PursuingEntity.transform.position.y)
                + m_PursuingEntity.m_Velocity * predictionTime);

            // Gets the vector between the entity and the target
            Vector2 seekPositionVector = m_TargetPosition - new Vector2(transform.position.x, transform.position.y);
            // Gets the unit vector of the seekPositionVector and multiplies it by the entity's max speed to get the desired velocity
            m_DesiredVelocity = Maths.Normalise(seekPositionVector) * m_Manager.m_Entity.m_MaxSpeed;
            // Seek force = desired velocity - current velocity of the entity
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            // Returns a unit vector of m_Steering multiplied by however strong the weight is set (in options)
            return Maths.Normalise(m_Steering) * m_Weight;
        }
        else
        {
            return Vector2.zero;
        }
    }
}
