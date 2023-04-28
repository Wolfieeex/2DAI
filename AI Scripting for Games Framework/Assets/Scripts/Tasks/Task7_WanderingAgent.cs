using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task7_WanderingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Wander m_Wander;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Wander = GetComponent<SteeringBehaviour_Wander>();

		if (!m_Wander)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}
}
