using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seek : SteeringBehaviour
{
    public Vector2 m_TargetPosition;

    public override Vector2 CalculateForce()
    {
        // Gets the vector between the entity and the target
        Vector2 seekPositionVector = m_TargetPosition - new Vector2(transform.position.x, transform.position.y);
        // Gets the unit vector of the seekPositionVector and multiplies it by the entity's max speed to get the desired velocity
        m_DesiredVelocity = Maths.Normalise(seekPositionVector) * m_Manager.m_Entity.m_MaxSpeed;
        // Seek force = desired velocity - current velocity of the entity
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        // Returns a unit vector of m_Steering multiplied by however strong the weight is set (in options)
        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
