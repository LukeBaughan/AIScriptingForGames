using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SteeringBehaviour_Seperation : SteeringBehaviour
{
    public float m_SeperationRange;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Vector2 accumulatedSeperationForce = Vector2.zero;
        // Gets all nearby entities (within a cricle where radius = m_SeperationRange)
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_SeperationRange);

        foreach (Collider2D entity in entities)
        {
            // Checks if the entity in the radius is using the seperation behaviour script and it does not belong to the same entity using this instance of the script
            if (entity.GetComponent<SteeringBehaviour_Seperation>() != null && entity.gameObject != this.gameObject)
            {
                // Checks if the dot product is greater than whatever the FOV is set to
                float dotProduct = Maths.Dot(transform.position, entity.transform.position);
                if (dotProduct > m_FOV)
                {
                    Vector2 vectorToEntity = new Vector2(transform.position.x, transform.position.y) - new Vector2(entity.transform.position.x, entity.transform.position.y);
                    Vector2 forceToAdd = Maths.Normalise(vectorToEntity) / new Vector2(MathF.Abs(vectorToEntity.x), MathF.Abs(vectorToEntity.y));
                    // Adds the neighbour force to the accumulated force
                    accumulatedSeperationForce += forceToAdd;
                }
            }
        }

        return Maths.Normalise(accumulatedSeperationForce) * m_Weight;
    }
}
