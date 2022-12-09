using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Alignment : SteeringBehaviour
{
    public float m_AlignmentRange;

    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Vector2 accumulatedHeading = Vector2.zero;

        List<MovingEntity> m_NeighbourEntities = new List<MovingEntity>();
        // Gets all nearby entities (within a cricle where radius = m_SeperationRange)
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_AlignmentRange);
        int neighbourCount = 0;

        foreach (Collider2D entity in entities)
        {
            // Checks if the entity in the radius is using the allignment behaviour script and it does not belong to the same entity using this instance of the script
            if (entity.GetComponent<SteeringBehaviour_Alignment>() != null && entity.gameObject != this.gameObject)
            {
                // If the dot product is greater than whatever the FOV is set to, add the entity to the neighbours list
                float dotProduct = Maths.Dot(transform.position, entity.transform.position);
                if (dotProduct > m_FOV)
                {
                    // Adds the neighbour's forward vector to the accumulated heading vector
                    MovingEntity movingEntity = entity.GetComponent<MovingEntity>();
                    accumulatedHeading += Maths.Normalise(movingEntity.m_Velocity);

                    neighbourCount++;
                }
            }
        }

        Vector2 alignmentForce = (accumulatedHeading / neighbourCount) - Maths.Normalise(m_Manager.m_Entity.m_Velocity);

        return Maths.Normalise(alignmentForce) * m_Weight;
    }
}
