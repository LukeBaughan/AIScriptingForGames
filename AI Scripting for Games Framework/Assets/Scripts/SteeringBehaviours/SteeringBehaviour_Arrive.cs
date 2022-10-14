using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 

    public override Vector2 CalculateForce()
    {
        //delete me
        return Vector2.zero;
    }
}
