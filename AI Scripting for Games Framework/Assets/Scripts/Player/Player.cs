using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingEntity
{
    public float m_Acceleration;

    //input
    float m_Horizontal;
    float m_Vertical;

    public bool m_CanMoveWhileAttacking;
    bool m_Attacking;

    void Update()
    {
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");
        
        if(Input.GetKeyDown(KeyCode.Space))
		{
            m_Animator.SetTrigger("Attack");
            m_Attacking = true;
        }

        if (!m_Attacking || (m_Attacking && m_CanMoveWhileAttacking))
        {
            //up
            if (m_Vertical > 0)
            {
                m_Animator.SetInteger("Direction", 0);
            }
            //right
            else if (m_Horizontal > 0)
            {
                m_Animator.SetInteger("Direction", 1);
            }
            //down
            else if (m_Vertical < 0)
            {
                m_Animator.SetInteger("Direction", 2);
            }
            //left
            else if (m_Horizontal < 0)
            {
                m_Animator.SetInteger("Direction", 3);
            }
            //idle
            else
            {
                m_Animator.SetInteger("Direction", -1);
            }
        }
    }

    public void StopAttack()
	{
        m_Attacking = false;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity ent = collision.GetComponent<Entity>();

        if (ent)
        {
            ent.TakeDamage(m_AttackPower);
        }
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Entity ent = collision.gameObject.GetComponent<Entity>();

		if (ent)
		{
			TakeDamage(ent.m_AttackPower);
		}
	}

	protected override Vector2 GenerateVelocity()
	{
		return new Vector2(m_Horizontal, m_Vertical) * m_Acceleration;
    }
}
