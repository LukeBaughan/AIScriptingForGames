using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovingEntity))]
public abstract class SteeringBehaviour : MonoBehaviour
{
    // Debug Lines
    [Header("Debugs")]
    public bool m_Debug_ShowDebugLines = false;
    public Color m_Debug_DesiredVelocityColour = Color.blue;
    public Color m_Debug_CurrentVelocityColour = Color.green;
    public Color m_Debug_SteeringColour = Color.red;
    
    [Space(10)]

    [Header("Settings")]
    // Should steering behaviour be used in calculations
    public bool m_Active = true;
    // How strong the force of the behaviour should be
    public float m_Weight = 5;

    protected SteeringBehaviour_Manager m_Manager;
    // Direction AI should be heading in
    protected Vector2 m_DesiredVelocity;
    // The force after the object's velocity has been considered
    protected Vector2 m_Steering;

    private void Awake()
    {
        // Gets the manager if its attached
        m_Manager = GetComponent<SteeringBehaviour_Manager>();

        if (!m_Manager) 
            Debug.LogError("No Steering Behaviour Manager attached to object", this);
    }

    public abstract Vector2 CalculateForce();

    // Debug Line drawing
    protected virtual void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                //desired velocity
                Gizmos.color = m_Debug_DesiredVelocityColour;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_DesiredVelocity);

                //current velocity
                Gizmos.color = m_Debug_CurrentVelocityColour;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_Manager.m_Entity.m_Velocity);

                //steering
                Gizmos.color = m_Debug_SteeringColour;
                Gizmos.DrawLine((Vector2)transform.position + m_Manager.m_Entity.m_Velocity, (Vector2)transform.position + m_Manager.m_Entity.m_Velocity + m_Steering);
            }
        }
    }
}
