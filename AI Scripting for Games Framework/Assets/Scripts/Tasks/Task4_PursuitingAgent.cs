using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task4_PursuitingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Pursuit m_Pursuit;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();

		if (!m_Pursuit)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
	}

	protected void Start()
	{
		m_Pursuit.m_PursuingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}
}
