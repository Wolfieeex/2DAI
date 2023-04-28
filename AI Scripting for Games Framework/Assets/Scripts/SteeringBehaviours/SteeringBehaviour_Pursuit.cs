using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Pursuit : SteeringBehaviour
{
    public MovingEntity m_PursuingEntity;

    public override Vector2 CalculateForce()
    {
        if (m_PursuingEntity == null)
        {
            Debug.LogError("Pursuing Target is null", this);
            return Vector2.zero;
        }
        else
        {
            Vector3 m_PlayerPosition = m_PursuingEntity.transform.position;
            m_DesiredVelocity = new Vector2(m_PlayerPosition.x - m_Manager.m_Entity.transform.position.x, m_PlayerPosition.y - m_Manager.m_Entity.transform.position.y);
            Rigidbody2D m_PlayerRigidbody = m_PursuingEntity.GetComponent<Rigidbody2D>();
            float m_CombinedSpeed = Maths.Magnitude(m_PlayerRigidbody.velocity) + Maths.Magnitude(m_Manager.m_Entity.m_Velocity);
            float m_PredictionTime = Maths.Magnitude(m_DesiredVelocity) / m_CombinedSpeed;
            Vector2 m_TargetPosition = new Vector2(m_PlayerPosition.x + (m_PlayerRigidbody.velocity.x * m_PredictionTime), m_PlayerPosition.y + (m_PlayerRigidbody.velocity.y * m_PredictionTime));
            m_DesiredVelocity = m_TargetPosition - new Vector2(m_Manager.m_Entity.transform.position.x, m_Manager.m_Entity.transform.position.y);
            m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            return Maths.Normalise(m_Steering) * m_Weight;
        }
        
    }
}
