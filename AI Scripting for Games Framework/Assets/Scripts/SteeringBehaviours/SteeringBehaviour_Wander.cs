using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Wander : SteeringBehaviour
{
    public float m_WanderRadius = 2; 
    public float m_WanderOffset = 2;
    public float m_AngleDisplacement = 2;

    Vector2 m_CirclePosition;
    Vector2 m_PointOnCircle;
    float m_Angle = 0.0f;

    public Vector2 m_TargetPosition;

    public override Vector2 CalculateForce()
    {
        m_Angle = Random.Range(0f, 2f) * Mathf.PI;
        m_PointOnCircle = new Vector2(Mathf.Cos(m_Angle) * m_WanderRadius, Mathf.Sin(m_Angle) * m_WanderRadius);
        Vector2 m_EntityPosition = new Vector2(m_Manager.m_Entity.transform.position.x, m_Manager.m_Entity.transform.position.y);
        Vector2 m_TargetVelocity;
        if (Maths.Magnitude(m_Manager.m_Entity.m_Velocity) > 0) m_TargetVelocity = Maths.Normalise(m_Manager.m_Entity.m_Velocity);
        else m_TargetVelocity = new Vector2(0, 0);


        m_CirclePosition = m_EntityPosition + m_TargetVelocity * m_WanderOffset;
        m_TargetPosition = m_CirclePosition + m_PointOnCircle;

        m_DesiredVelocity = m_TargetPosition - m_EntityPosition;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
        return Maths.Normalise(m_Steering) * m_Weight;
    }

	protected override void OnDrawGizmosSelected()
	{
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(m_CirclePosition, m_WanderRadius);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, m_CirclePosition);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(m_CirclePosition, m_PointOnCircle);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, m_PointOnCircle);

                base.OnDrawGizmosSelected();
			}
        }
	}
}
