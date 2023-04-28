using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Manager : MonoBehaviour
{
    public MovingEntity m_Entity { get; private set; }
    public float m_MaxForce = 100;
    public float m_RemainingForce;
    public Vector2 m_TotalForce;
    public List<SteeringBehaviour> m_SteeringBehaviours;
    public bool isDecisionMakingScene = false;

	private void Awake()
	{
        m_Entity = GetComponent<MovingEntity>();

        if(!m_Entity)
            Debug.LogError("Steering Behaviours only working on type moving entity", this);
    }

	public Vector2 GenerateSteeringForce()
    {
        /* Week I Force
        if (m_SteeringBehaviours.Count > 0)
        {
            return m_SteeringBehaviours[0].CalculateForce();
        }
        else
        {
            Debug.LogError("List of behaviours is empty", this);
            return Vector2.zero;
        }*/

        m_RemainingForce = m_MaxForce;
        m_TotalForce = Vector2.zero;

        if (!isDecisionMakingScene)
        {
            foreach (SteeringBehaviour SB in m_SteeringBehaviours)
            {
                if (m_RemainingForce > 0 && SB.m_Active)
                {
                    Vector2 m_TempForce = SB.CalculateForce();
                    if (Maths.Magnitude(m_TempForce) > m_RemainingForce) m_TempForce = Maths.Normalise(m_TempForce) * m_RemainingForce;
                    m_RemainingForce -= Maths.Magnitude(m_TempForce);
                    m_TotalForce += m_TempForce;
                }
                else break;
            }
        }
        else
        {
            foreach (SteeringBehaviour SB in m_SteeringBehaviours)
            {
                if (SB.m_Active)
                {
                    Vector2 m_TempForce = SB.CalculateForce();
                    m_TotalForce += m_TempForce * SB.SteeringImportance;
                }
                else break;
            }
        }

        return m_TotalForce;
    }

    public void EnableExclusive(SteeringBehaviour behaviour)
	{
        if(m_SteeringBehaviours.Contains(behaviour))
		{
            foreach(SteeringBehaviour sb in m_SteeringBehaviours)
			{
                sb.m_Active = false;
			}

            behaviour.m_Active = true;
		}
        else
		{
            Debug.Log(behaviour + " does not exist on object", this);
		}
	}
    public void DisableAllSteeringBehaviours()
    {
        foreach (SteeringBehaviour sb in m_SteeringBehaviours)
        {
            sb.m_Active = false;
        }
    }
}
