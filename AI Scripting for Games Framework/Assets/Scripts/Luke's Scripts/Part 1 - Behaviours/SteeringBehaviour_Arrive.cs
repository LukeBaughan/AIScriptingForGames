using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 
    public float m_WeightLerp = 0.0f; 

    public override Vector2 CalculateForce()
    {

        float deceleration = 1.0f;

        // Gets the vector between the entity and the target
        Vector2 seekPositionVector = m_TargetPosition - new Vector2(transform.position.x, transform.position.y);

        float arriveSpeed = Maths.Magnitude(seekPositionVector) * deceleration;
        // If the arrive speed is greater than the max speed, then cap the desired speed to the value of max speed
        if (arriveSpeed > m_Manager.m_Entity.m_MaxSpeed)
        {
            arriveSpeed = m_Manager.m_Entity.m_MaxSpeed;
        }

        // Gets the desired velocity by multiplying the target vector by the arrive speed
        m_DesiredVelocity = Maths.Normalise(seekPositionVector) * arriveSpeed;

        // Subtract the desired velocity by the entity's velocity to get the arrive force
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
