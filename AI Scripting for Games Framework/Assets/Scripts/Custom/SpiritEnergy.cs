using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritEnergy : MonoBehaviour
{
    private Collider2D E_EnterArea;

    private float t_CurrentCooldownTimer = 0.0f;
    private bool Active = true;
    public SpriteRenderer Sprite;
    public float t_CooldownTimer = 10.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Active)
        {
            GameObject Object = other.gameObject;
            DecisionMakingEntity Entity = Object.GetComponent<DecisionMakingEntity>();
            if (Entity != null)
            {
                Entity.RestoreSpirit();
                t_CurrentCooldownTimer = t_CooldownTimer;
                Active = false;
                Sprite.enabled = false;
            }
            else if (Object.layer == LayerMask.NameToLayer("Player"))
            {
                t_CurrentCooldownTimer = t_CooldownTimer;
                Active = false;
                Sprite.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (t_CurrentCooldownTimer > 0.0f) t_CurrentCooldownTimer -= Time.deltaTime;
        else if (!Active)
        {
            Active = true;
            Sprite.enabled = true;
        } 
    }

}
