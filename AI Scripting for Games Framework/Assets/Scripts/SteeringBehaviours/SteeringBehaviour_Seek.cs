using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seek : SteeringBehaviour
{
    public Vector2 m_TargetPosition;

    public override Vector2 CalculateForce()
    {
        Vector3 m_CharacterPosition = m_Manager.m_Entity.transform.position;
        m_DesiredVelocity = m_TargetPosition - new Vector2(m_CharacterPosition.x, m_CharacterPosition.y);
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
        return Maths.Normalise(m_Steering) * m_Weight;
    }
}
