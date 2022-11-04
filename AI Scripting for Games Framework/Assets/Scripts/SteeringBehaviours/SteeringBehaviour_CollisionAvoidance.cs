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
        /*
        m_Steering = Vector2.zero;
        m_DesiredVelocity = Vector2.zero;
        // Set a very high value as the for loop will be looking for the feeler collision with the closest distance
        float closestDistance = 999999.0f;

        for(int i = 0; i < m_Feelers.Length; i++)
        {
            RaycastHit2D tempHit = Physics2D.Raycast(transform.position, m_FeelerVectors[i], m_FeelersLength[i], m_FeelerLayerMask.value);
            print(tempHit.distance);

            if (tempHit.distance < closestDistance)
            {
                closestDistance = tempHit.distance;
            }
        }
       // print(closestDistance);
          */
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
