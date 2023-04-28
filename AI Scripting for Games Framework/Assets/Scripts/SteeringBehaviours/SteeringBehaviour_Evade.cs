using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Evade : SteeringBehaviour
{
    public MovingEntity m_EvadingEntity;
    public float m_EvadeRadius;

    public override Vector2 CalculateForce()
    {
        if (m_EvadingEntity == null)
        {
            Debug.LogError("Flee Target is null", this);
            return Vector2.zero;
        } 
        else
        {
            Vector3 m_PlayerPosition = m_EvadingEntity.transform.position;
            m_DesiredVelocity = new Vector2((m_PlayerPosition.x - m_Manager.m_Entity.transform.position.x), (m_PlayerPosition.y - m_Manager.m_Entity.transform.position.y));
            Vector2 m_Distance = m_DesiredVelocity;
            Rigidbody2D m_PlayerRigidbody = m_EvadingEntity.GetComponent<Rigidbody2D>();
            float m_CombinedSpeed = Maths.Magnitude(m_PlayerRigidbody.velocity) + Maths.Magnitude(m_Manager.m_Entity.m_Velocity);
            float m_PredictionTime = Maths.Magnitude(m_DesiredVelocity) / m_CombinedSpeed;
            Vector2 m_TargetPosition = new Vector2(m_PlayerPosition.x + (m_PlayerRigidbody.velocity.x * m_PredictionTime), m_PlayerPosition.y + (m_PlayerRigidbody.velocity.y * m_PredictionTime));
            m_DesiredVelocity = m_TargetPosition - new Vector2(m_Manager.m_Entity.transform.position.x, m_Manager.m_Entity.transform.position.y);
            m_DesiredVelocity = new Vector2(m_DesiredVelocity.x * - 1, m_DesiredVelocity.y * - 1);
            m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            m_Steering = Maths.Normalise(m_Steering) * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(m_Distance), m_EvadeRadius) / m_EvadeRadius);
            return m_Steering;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, m_EvadeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
