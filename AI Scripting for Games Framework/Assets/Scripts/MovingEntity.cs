using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntity : Entity
{
    Rigidbody2D m_Rigidbody;
    public float m_MaxSpeed = 5.0f;
    public Vector2 m_Velocity { get { return m_Rigidbody.velocity; } }
    public float m_Mass = 1.0f;
    public bool m_RotatesBasedOnVelocity;
    public bool m_FlipsBasedOnVelocity;

	protected override void Awake()
	{
		base.Awake();

        m_Rigidbody = GetComponent<Rigidbody2D>();
	}

	protected abstract Vector2 GenerateVelocity();

    protected virtual void FixedUpdate()
	{
        MoveAndRotate();
	}

	protected void MoveAndRotate()
	{
        Vector2 force = GenerateVelocity();
        Vector2 acceleration = force / m_Mass;
        m_Rigidbody.velocity += acceleration * Time.deltaTime;
        m_Rigidbody.velocity = Vector2.ClampMagnitude(m_Rigidbody.velocity, m_MaxSpeed);

        if (Maths.Magnitude(m_Rigidbody.velocity) > 0.01f)
        {
            if (m_RotatesBasedOnVelocity)
            {
                //assume up is facing direction
                float angle = Mathf.Atan2(m_Rigidbody.velocity.y, m_Rigidbody.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
            else if (m_FlipsBasedOnVelocity)
            {
                if (m_Rigidbody.velocity.x <= 0.1f)
                    m_Renderer.flipX = false;
                else if (m_Rigidbody.velocity.x >= 0.1f)
                    m_Renderer.flipX = true;
            }
        }

        if (m_Animator)
            m_Animator.SetFloat("Speed", Maths.Magnitude(m_Rigidbody.velocity));
    }
}
