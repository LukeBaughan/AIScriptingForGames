using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Flee : SteeringBehaviour
{
    public Transform m_FleeTarget;
    public float m_FleeRadius;

    public override Vector2 CalculateForce()
    {
        //delete me
        return Vector2.zero;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, m_FleeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
