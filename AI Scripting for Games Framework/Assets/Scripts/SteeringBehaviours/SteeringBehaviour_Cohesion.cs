using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Cohesion : SteeringBehaviour
{
    public float m_CohesionRange;

    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Vector2 accumulatedPosition = Vector2.zero;

        // Gets all nearby entities (within a cricle where radius = m_SeperationRange)
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_CohesionRange);
        int neighbourCount = 0;

        foreach (Collider2D entity in entities)
        {
            // Checks if the entity in the radius is using the cohesion behaviour script and it is not the same entity using this script
            if (entity.GetComponent<SteeringBehaviour_Cohesion>() != null && entity.gameObject != this.gameObject)
            {
                // If the dot product is greater than whatever the FOV is set to, add the entity to the neighbours list
                float dotProduct = Maths.Dot(transform.position, entity.transform.position);
                if (dotProduct > m_FOV)
                {
                    // Adds the neighbour posiiton to the accumulated position
                    accumulatedPosition += new Vector2(entity.transform.position.x, entity.transform.position.y);
                    neighbourCount++;
                }
            }
        }

        Vector2 cohesionPosition = accumulatedPosition / neighbourCount;

        // Gets the vector between the entity and the target
        Vector2 seekPositionVector = cohesionPosition - new Vector2(transform.position.x, transform.position.y);
        // Gets the unit vector of the seekPositionVector and multiplies it by the entity's max speed to get the desired velocity
        m_DesiredVelocity = Maths.Normalise(seekPositionVector) * m_Manager.m_Entity.m_MaxSpeed;
        // Seek force = desired velocity - current velocity of the entity
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        // Returns a unit vector of m_Steering multiplied by however strong the weight is set (in options)
        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
