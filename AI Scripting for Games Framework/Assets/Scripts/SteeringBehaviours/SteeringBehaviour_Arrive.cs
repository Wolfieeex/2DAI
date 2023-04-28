using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 

    public override Vector2 CalculateForce()
    {
        Vector3 m_CharacterPosition = m_Manager.m_Entity.transform.position;
        m_DesiredVelocity = m_TargetPosition - new Vector2(m_CharacterPosition.x, m_CharacterPosition.y);
        Vector2 m_Distance = m_DesiredVelocity;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
        return Maths.Normalise(m_Steering) * Mathf.Lerp(0, m_Weight, Mathf.Min(Maths.Magnitude(m_Distance), m_SlowingRadius) / m_SlowingRadius);
    }
}
