using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task6_ArrivingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Arrive m_Arrive;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Arrive = GetComponent<SteeringBehaviour_Arrive>();

		if (!m_Arrive)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
	}

	protected void Start()
	{
		m_Arrive.m_TargetPosition = Grid.GetRandomWalkableTile().transform.position;
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

	protected void Update()
	{
		if(Maths.Magnitude((Vector2)transform.position - m_Arrive.m_TargetPosition) < 0.1f)
		{
			m_Arrive.m_TargetPosition = Grid.GetRandomWalkableTile().transform.position;
		}

	}
}
