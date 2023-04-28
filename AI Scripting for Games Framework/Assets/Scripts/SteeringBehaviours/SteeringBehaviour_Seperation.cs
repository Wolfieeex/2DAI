using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seperation : SteeringBehaviour
{
    public float m_SeperationRange;
    Vector2 accumulatedSeperationForce = Vector2.zero;
    
    [Range(1,-1)]
    public float m_FOV;
    public Vector2 ASF;
    public override Vector2 CalculateForce()
    {
        m_DesiredVelocity = Vector2.zero;
        m_Steering = Vector2.zero;
        ASF = Vector2.zero;
        Vector2 m_characterPosition = (Vector2)this.GetComponent<Transform>().position;
        Vector2 m_currentVelocity = GetComponent<Rigidbody2D>().velocity;
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_SeperationRange);
        if (entities.Length > 0)
        {
            foreach (Collider2D Enemy in entities)
            {
                Vector2 m_enemyPosition = (Vector2)Enemy.GetComponent<Transform>().position;
                Vector2 m_targetVelocity = m_characterPosition - m_enemyPosition;
                float m_Dot = Maths.Dot(m_currentVelocity, m_targetVelocity);
                if (m_Dot >= m_FOV)
                {
                    Vector2 FtA = Maths.Normalise(m_targetVelocity) / Maths.Magnitude(m_targetVelocity);
                    ASF += FtA;
                }
            }
        }
        m_DesiredVelocity = ASF * GetComponent<MovingEntity>().m_MaxSpeed;
        if (Maths.Magnitude(m_currentVelocity) > 0.0f) m_Steering = m_DesiredVelocity - m_currentVelocity;
        else m_Steering = m_DesiredVelocity;
        m_Steering = Maths.Normalise(m_Steering) * m_Weight;
        if (Maths.Magnitude(m_Steering) > 0.0f) return m_Steering;
        else return Vector2.zero;
    }
}
