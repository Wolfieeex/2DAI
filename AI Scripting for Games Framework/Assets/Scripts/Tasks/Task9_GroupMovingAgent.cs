using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task9_GroupMovingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Pursuit m_Pursuit;
	SteeringBehaviour_Seperation m_Seperation;
	SteeringBehaviour_Cohesion m_Cohesion;
	SteeringBehaviour_Alignment m_Alignment;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();

		if (!m_Pursuit)
			Debug.LogError("Object doesn't have a Pursuit Steering Behaviour attached", this);

		m_Seperation = GetComponent<SteeringBehaviour_Seperation>();

		if (!m_Seperation)
			Debug.LogError("Object doesn't have a Seperation Steering Behaviour attached", this);

		m_Cohesion = GetComponent<SteeringBehaviour_Cohesion>();

		if (!m_Cohesion)
			Debug.LogError("Object doesn't have a Cohesion Steering Behaviour attached", this);

		m_Alignment = GetComponent<SteeringBehaviour_Alignment>();

		if (!m_Alignment)
			Debug.LogError("Object doesn't have a Alignment Steering Behaviour attached", this);
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

	protected void Start()
	{
		m_Pursuit.m_PursuingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
	}
}
