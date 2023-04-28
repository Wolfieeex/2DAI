using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Alignment : SteeringBehaviour
{
    public float m_AlignmentRange;

    [Range(1, -1)]
    public float m_FOV;
    public Vector2 AF;
    public override Vector2 CalculateForce()
    {
        m_DesiredVelocity = Vector2.zero;
        m_Steering = Vector2.zero;
        AF = Vector2.zero;
        int agentsCount = 0;
        Vector2 m_characterPosition = (Vector2)this.GetComponent<Transform>().position;
        Vector2 m_currentVelocity = GetComponent<Rigidbody2D>().velocity;
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_AlignmentRange);
        if (entities.Length > 0)
        {
            foreach (Collider2D Enemy in entities)
            {
                Vector2 m_enemyPosition = (Vector2)Enemy.GetComponent<Transform>().position;
                Vector2 m_targetVelocity = m_characterPosition - m_enemyPosition;
                float m_Dot = Maths.Dot(m_currentVelocity, m_targetVelocity);
                if (m_Dot >= m_FOV)
                {
                    Vector2 m_EnemyFacing = Maths.Normalise(Enemy.GetComponent<Rigidbody2D>().velocity);
                    AF += m_EnemyFacing;
                    agentsCount++;
                }
            }
        }
        AF = AF / agentsCount;
        m_DesiredVelocity = (AF - Maths.Normalise(m_currentVelocity)) * GetComponent<MovingEntity>().m_MaxSpeed;
        if (Maths.Magnitude(m_currentVelocity) > 0.0f) m_Steering = m_DesiredVelocity - m_currentVelocity;
        else m_Steering = m_DesiredVelocity;
        m_Steering = Maths.Normalise(m_Steering) * m_Weight;
        if (Maths.Magnitude(m_Steering) > 0.0f) return m_Steering;
        else return Vector2.zero;
    }
}
