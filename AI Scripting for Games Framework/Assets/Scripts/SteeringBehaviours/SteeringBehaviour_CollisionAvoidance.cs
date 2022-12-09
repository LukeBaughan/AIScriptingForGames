using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SteeringBehaviour_CollisionAvoidance : SteeringBehaviour
{
    [System.Serializable]
    public struct Feeler
	{
        [Range(0, 360)]
        public float m_Angle;
        public float m_MaxLength;
        public Color m_Colour;
    }

    public Feeler[] m_Feelers;
    Vector2[] m_FeelerVectors;
    float[] m_FeelersLength;
    
    [SerializeField]
    LayerMask m_FeelerLayerMask;

    private void Start()
    {
        m_FeelersLength = new float[m_Feelers.Length];
        m_FeelerVectors = new Vector2[m_Feelers.Length];
    }

    public override Vector2 CalculateForce()
    {
        //The entity has multiple 'feelers' protruding in front of it (the amount of feelers and the angle of the feelers can be changed in the editor)
        // Sets the feelers to have the angle and rotation specified in the editor
        UpdateFeelers();
        
        m_Steering = Vector2.zero;
        m_DesiredVelocity = Vector2.zero;
        // Set a very high value as the for loop will be looking for the feeler collision with the closest distance
        float closestDistance = float.MaxValue;
        Vector2 avoidancePoint = Vector2.zero;
        bool flee = false;

        // Loops through all of the feelers
        for (int i = 0; i < m_Feelers.Length; i++)
        {
            RaycastHit2D tempHit = Physics2D.Raycast(transform.position, m_FeelerVectors[i], m_FeelersLength[i], m_FeelerLayerMask.value);
            //print(tempHit.distance);

            // Checks if the raycast hit a collider
            if (tempHit.collider != null)
            {
                //print(tempHit.collider.gameObject.name);

                // If the distance from the collision point is less than the previous point, make this the new closest point
                if (tempHit.distance < closestDistance)
                {
                    closestDistance = tempHit.distance;
                    avoidancePoint = tempHit.point;
                    flee = true;
                }
            }
        }

        // Entity flees if one of the feelers hit a collider
        if(flee == true)
        {
            // Gets the vector between the entity and the flee target
            Vector2 fleePositionVector = (new Vector2(transform.position.x, transform.position.y) - avoidancePoint);
            // Gets the unit vector of the fleePositionVector and multiplies it by the entity's max speed to get the desired velocity
            m_DesiredVelocity = Maths.Normalise(fleePositionVector) * m_Manager.m_Entity.m_MaxSpeed;
            // Seek force = desired velocity - current velocity of the entity
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            // Returns a unit vector of m_Steering multiplied by the weight
            return Maths.Normalise(m_Steering) * m_Weight;
        }

        return Vector2.zero;
      
    }

    void UpdateFeelers()
    {
        for (int i = 0; i < m_Feelers.Length; ++i)
        {
            m_FeelersLength[i] = Mathf.Lerp(1, m_Feelers[i].m_MaxLength, Maths.Magnitude(m_Manager.m_Entity.m_Velocity) / m_Manager.m_Entity.m_MaxSpeed);
            m_FeelerVectors[i] = Maths.RotateVector(Maths.Normalise(m_Manager.m_Entity.m_Velocity), m_Feelers[i].m_Angle) * m_FeelersLength[i];
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                for (int i = 0; i < m_Feelers.Length; ++i)
                {
                    Gizmos.color = m_Feelers[i].m_Colour;
                    Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_FeelerVectors[i]);
                }

                base.OnDrawGizmosSelected();
            }
        }
    }
}
