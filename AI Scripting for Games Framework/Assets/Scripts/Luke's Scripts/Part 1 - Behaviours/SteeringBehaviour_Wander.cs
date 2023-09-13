using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Wander : SteeringBehaviour
{
    public float m_WanderRadius = 5; 
    public float m_WanderOffset = 2;
    public float m_AngleDisplacement = 2;

    Vector2 m_wanderDirection;

    Vector2 m_CirclePosition;
    Vector2 m_PointOnCircle;
    float m_Angle = 0.0f;

    public override Vector2 CalculateForce()
    {
        // Gets a random point on a circle (Min and max values are -1 and 1)
        m_Angle = Mathf.Acos(Random.Range(-1f, 1f));
        // If random value is less than .5, add 0, if not, add PI, then add that value to m_angle
        // 50% chance of adding 180 degrees to make the angle either left or right
        m_Angle = m_Angle + (Random.value < 0.5f ? 0 : Mathf.PI);

        // Sets the circle's position to be in front of the entity by the value of m_WanderOffset
        m_CirclePosition = new Vector2(transform.position.x, transform.position.y) + (Maths.Normalise(m_Manager.m_Entity.m_Velocity) * m_WanderOffset);

        // Rotates m_CirclePosition to face the right direction 
        m_wanderDirection.x = Mathf.Cos(m_Angle) - Mathf.Sin(m_Angle);
        m_wanderDirection.y = Mathf.Sin(m_Angle) + Mathf.Cos(m_Angle);
        // Scale the direction by the radius
        m_wanderDirection = Maths.Normalise(m_wanderDirection) * m_WanderRadius;

        // Sets the wander position to be in the circle
        m_PointOnCircle = transform.position;
        m_PointOnCircle += Maths.Normalise(m_Manager.m_Entity.m_Velocity) * m_WanderOffset;
        m_PointOnCircle += m_wanderDirection;

        // Seek code
        // Gets the vector between the entity and the target
        Vector2 seekPositionVector = m_PointOnCircle - new Vector2(transform.position.x, transform.position.y);
        // Gets the unit vector of the seekPositionVector and multiplies it by the entity's max speed to get the desired velocity
        m_DesiredVelocity = Maths.Normalise(seekPositionVector) * m_Manager.m_Entity.m_MaxSpeed;
        // Seek force = desired velocity - current velocity of the entity
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
        // Returns a unit vector of m_Steering multiplied by however strong the weight is set (in options)
        return Maths.Normalise(m_Steering) * m_Weight;
    }

    protected override void OnDrawGizmosSelected()
	{
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(m_CirclePosition, m_WanderRadius);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, m_CirclePosition);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(m_CirclePosition, m_PointOnCircle);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, m_PointOnCircle);

                base.OnDrawGizmosSelected();
			}
        }
	}
}
