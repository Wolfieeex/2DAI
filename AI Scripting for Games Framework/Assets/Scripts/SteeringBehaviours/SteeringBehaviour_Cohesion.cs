using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Cohesion : SteeringBehaviour
{
    public float m_CohesionRange;

    [Range(1, -1)]
    public float m_FOV;
    public Vector2 AP;
    public override Vector2 CalculateForce()
    {
        m_DesiredVelocity = Vector2.zero;
        m_Steering = Vector2.zero;
        AP = Vector2.zero;
        int agentsCount = 0;
        Vector2 m_characterPosition = (Vector2)this.GetComponent<Transform>().position;
        Vector2 m_currentVelocity = GetComponent<Rigidbody2D>().velocity;
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_CohesionRange);
        if (entities.Length > 0)
        {
            foreach (Collider2D Enemy in entities)
            {
                Vector2 m_enemyPosition = (Vector2)Enemy.GetComponent<Transform>().position;
                Vector2 m_targetVelocity = m_characterPosition - m_enemyPosition;
                float m_Dot = Maths.Dot(m_currentVelocity, m_targetVelocity);
                if (m_Dot >= m_FOV)
                {
                    AP += m_enemyPosition;
                    agentsCount++;
                }
            }
        }
        AP = AP / agentsCount;
        m_DesiredVelocity = Maths.Normalise(AP - m_characterPosition) * GetComponent<MovingEntity>().m_MaxSpeed;
        if (Maths.Magnitude(m_currentVelocity) > 0.0f) m_Steering = m_DesiredVelocity - m_currentVelocity;
        else m_Steering = m_DesiredVelocity;
        m_Steering = Maths.Normalise(m_Steering) * m_Weight;
        if (Maths.Magnitude(m_Steering) > 0.0f) return m_Steering;
        else return Vector2.zero;
    }
}
