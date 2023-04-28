using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task3_FleeingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Flee m_Flee;

	protected override void Awake()
	{
		base.Awake();

		m_Flee = GetComponent<SteeringBehaviour_Flee>();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		if (!m_Flee)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
	}

	protected void Start()
	{
		m_Flee.m_FleeTarget = GameObject.Find("Player").transform;
	}

	protected override Vector2 GenerateVelocity()
	{
		float m_FleeRadius = Maths.Magnitude(new Vector2(m_Flee.m_FleeTarget.position.x, m_Flee.m_FleeTarget.position.y) - new Vector2(this.transform.position.x, this.transform.position.y));
		return m_SteeringBehaviours.GenerateSteeringForce();
	}
}
