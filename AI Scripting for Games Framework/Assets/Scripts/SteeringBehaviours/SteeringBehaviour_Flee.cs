using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Flee : SteeringBehaviour
{
    public Transform m_FleeTarget;
    public float m_FleeRadius;

    public override Vector2 CalculateForce()
    {
        if (m_FleeTarget == null)
        {
             Debug.LogError("Flee Target is null", this);
            return Vector2.zero;
        } 
        else
        {
            Vector2 m_Distance = new Vector2(m_FleeTarget.position.x - m_Manager.m_Entity.transform.position.x, m_FleeTarget.position.y - m_Manager.m_Entity.transform.position.y);
            m_DesiredVelocity = Maths.Normalise(m_Distance) * m_Manager.m_Entity.m_MaxSpeed;
            m_DesiredVelocity = new Vector2(m_DesiredVelocity.x * - 1, m_DesiredVelocity.y * - 1);
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            m_Steering = Maths.Normalise(m_Steering) * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(m_Distance), m_FleeRadius) / m_FleeRadius);
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
                Gizmos.DrawWireSphere(transform.position, m_FleeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
