using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator m_Animator;
    protected SpriteRenderer m_Renderer;
    public int m_MaxHealth = 5;
    float m_CurrentHealth;
    public int m_AttackPower = 2;

    protected virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Renderer = GetComponent<SpriteRenderer>();
        m_CurrentHealth = m_MaxHealth;
    }

    public virtual void PlayDeathAnimation()
    {
        if (m_Animator)
            m_Animator.SetTrigger("Die");
    }

    public virtual void DestroyEntity()
    {
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage)
	{
        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0)
            PlayDeathAnimation();
	}
}
