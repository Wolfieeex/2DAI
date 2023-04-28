using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovingEntity))]
public abstract class SteeringBehaviour : MonoBehaviour
{
    [Header("Debugs")]
    public bool m_Debug_ShowDebugLines = false;
    public Color m_Debug_DesiredVelocityColour = Color.blue;
    public Color m_Debug_CurrentVelocityColour = Color.green;
    public Color m_Debug_SteeringColour = Color.red;
    
    [Space(10)]

    [Header("Settings")]
    public bool m_Active = true;
    public float m_Weight = 1;

    protected SteeringBehaviour_Manager m_Manager;
    protected Vector2 m_DesiredVelocity;
    protected Vector2 m_Steering;

    public float SteeringImportance = 0.0f;

    private void Awake()
    {
        m_Manager = GetComponent<SteeringBehaviour_Manager>();

        if (!m_Manager) 
            Debug.LogError("No Steering Behaviour Manager attached to object", this);
    }

    public abstract Vector2 CalculateForce();

    protected virtual void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                //desired velocity
                Gizmos.color = m_Debug_DesiredVelocityColour;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_DesiredVelocity);

                //current velocity
                Gizmos.color = m_Debug_CurrentVelocityColour;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_Manager.m_Entity.m_Velocity);

                //steering
                Gizmos.color = m_Debug_SteeringColour;
                Gizmos.DrawLine((Vector2)transform.position + m_Manager.m_Entity.m_Velocity, (Vector2)transform.position + m_Manager.m_Entity.m_Velocity + m_Steering);
            }
        }
    }
}
