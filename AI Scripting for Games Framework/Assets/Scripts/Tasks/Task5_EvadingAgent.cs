using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task5_EvadingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Evade m_Evade;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Evade = GetComponent<SteeringBehaviour_Evade>();

		if (!m_Evade)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
	}

	protected void Start()
	{
		m_Evade.m_EvadingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}
}
