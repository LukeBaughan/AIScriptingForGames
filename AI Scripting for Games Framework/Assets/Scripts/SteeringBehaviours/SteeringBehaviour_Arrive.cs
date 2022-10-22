using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 

    public override Vector2 CalculateForce()
    {
        float deceleration = 0.5f;

        // Gets the vector between the entity and the target
        Vector2 seekPositionVector = m_TargetPosition - new Vector2(transform.position.x, transform.position.y);
        Vector2 arriveSpeed = seekPositionVector * deceleration;
        //Vector2 maxVelocity = (Maths.Normalise(seekPositionVector) * m_Manager.m_Entity.m_MaxSpeed);
        // If the desired speed is greater than the max speed, then cap the desired speed to the value of max speed
        if (Maths.Magnitude(arriveSpeed) > m_Manager.m_Entity.m_MaxSpeed)
            arriveSpeed = Maths.Normalise(seekPositionVector) * m_Manager.m_Entity.m_MaxSpeed;
        // might need to be normalised
        m_DesiredVelocity = Maths.Normalise(seekPositionVector) * arriveSpeed;
        //m_DesiredVelocity = seekPositionVector * arriveSpeed;
        m_Steering = arriveSpeed - m_Manager.m_Entity.m_Velocity;

        return Maths.Normalise(m_Steering) * m_Weight;


        /*
        // Gets the unit vector of the seekPositionVector and multiplies it by the entity's max speed to get the desired velocity
        m_DesiredVelocity = Maths.Normalise(seekPositionVector) * m_Manager.m_Entity.m_MaxSpeed;
        // Seek force = desired velocity - current velocity of the entity
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        // Returns a unit vector of m_Steering multiplied by however strong the weight is set (in options)
        return Maths.Normalise(m_Steering) * m_Weight;
        */
    }
}
