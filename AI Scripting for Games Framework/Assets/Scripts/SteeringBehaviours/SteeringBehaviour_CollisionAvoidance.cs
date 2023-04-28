using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SteeringBehaviour_CollisionAvoidance : SteeringBehaviour
{
    [System.Serializable]
    public struct Feeler
	{
        [Range(0, 360)]
        public float m_Angle;
        public float m_MaxLength;
        public Color m_Colour;
    }

    public Feeler[] m_Feelers;
    Vector2[] m_FeelerVectors;
    float[] m_FeelersLength;
    
    [SerializeField]
    LayerMask m_FeelerLayerMask;

    private void Start()
    {
        m_FeelersLength = new float[m_Feelers.Length];
        m_FeelerVectors = new Vector2[m_Feelers.Length];
    }

    public override Vector2 CalculateForce()
    {
        UpdateFeelers();
        m_DesiredVelocity = Vector2.zero;
        m_Steering = Vector2.zero;
        Vector2 m_EntityPosition = new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y);

        float tempDistance = 0.0f;
        Transform closestFeeler = GetComponent<Transform>();
        float tempMaxDistance = 0.0f;
        for (int i = 0; i < m_Feelers.Length; ++i)
        {
            RaycastHit2D tempHit = Physics2D.Raycast(m_EntityPosition, m_FeelerVectors[i], m_FeelersLength[i], m_FeelerLayerMask.value);
            if (tempHit.distance > 0 && tempHit.transform != GetComponent<Transform>())
            {
                if (tempDistance == 0.0f)
                {
                    tempDistance = tempHit.distance;
                    closestFeeler = tempHit.transform;
                    tempMaxDistance = m_Feelers[i].m_MaxLength;
                }
                else
                {
                    if (tempDistance > tempHit.distance)
                    {
                        tempDistance = tempHit.distance;
                        closestFeeler = tempHit.transform;
                        tempMaxDistance = m_Feelers[i].m_MaxLength;
                    }
                }
            }
        }
        
        if (tempDistance == 0) return Vector2.zero;
        else
        {
            Vector2 m_FleeTarget = closestFeeler.position;
            Vector2 m_Distance = m_FleeTarget - m_EntityPosition;
            m_DesiredVelocity = Maths.Normalise(m_Distance) * GetComponent<MovingEntity>().m_MaxSpeed;
            m_DesiredVelocity = new Vector2(m_DesiredVelocity.x * -1, m_DesiredVelocity.y * -1);
            m_Steering = m_DesiredVelocity - GetComponent<Rigidbody2D>().velocity;
            m_Steering = Maths.Normalise(m_Steering) * m_Weight;
            return m_Steering;
        }
    }

    void UpdateFeelers()
    {
        for (int i = 0; i < m_Feelers.Length; ++i)
        {
            m_FeelersLength[i] = Mathf.Lerp(1, m_Feelers[i].m_MaxLength, Maths.Magnitude(m_Manager.m_Entity.m_Velocity) / m_Manager.m_Entity.m_MaxSpeed);
            m_FeelerVectors[i] = Maths.RotateVector(Maths.Normalise(m_Manager.m_Entity.m_Velocity), m_Feelers[i].m_Angle) * m_FeelersLength[i];
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                for (int i = 0; i < m_Feelers.Length; ++i)
                {
                    Gizmos.color = m_Feelers[i].m_Colour;
                    Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_FeelerVectors[i]);
                }

                base.OnDrawGizmosSelected();
            }
        }
    }
}
