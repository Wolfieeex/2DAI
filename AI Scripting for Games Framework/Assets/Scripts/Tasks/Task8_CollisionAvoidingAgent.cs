using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task8_CollisionAvoidingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Seek m_Seek;
	SteeringBehaviour_CollisionAvoidance m_Avoidance;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Seek = GetComponent<SteeringBehaviour_Seek>();

		if (!m_Seek)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

		m_Avoidance = GetComponent<SteeringBehaviour_CollisionAvoidance>();

		if (!m_Avoidance)
			Debug.LogError("Object doesn't have a Collision Avoidance Steering Behaviour attached", this);
	}

	protected void Start()
	{
		m_Seek.m_TargetPosition = Grid.GetRandomWalkableTile().transform.position;
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

	protected void Update()
	{
		if(Maths.Magnitude((Vector2)transform.position - m_Seek.m_TargetPosition) < 0.1f)
		{
			m_Seek.m_TargetPosition = Grid.GetRandomWalkableTile().transform.position;
		}
	}
}
