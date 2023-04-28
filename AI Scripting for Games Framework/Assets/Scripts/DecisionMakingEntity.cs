using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionMakingEntity : MovingEntity
{
    private float P_MaxHealth = 100.0f;
    private float P_CurrentHealth = 100.0f;
    public float P_HealthLossParameter = 5.0f;
    public Slider P_HealthSlider;
    public Image P_SliderFill;
    public Color SafeZone;
    public Color ThreatZone;
    public Color CriticalZone;

    private bool P_InChase = false;
    private float P_VisionRange = 35.0f;
    private float P_SafetyRange = 10.0f;
    private SpiritEnergy ClosestSpirit;

    public float P_MaxLostVisionChaseTime = 5.0f;
    private float P_CurrentLostVisionChaseTime = 0.0f;
 
    public SteeringBehaviour_Manager m_SteeringBehaviours;
    public SteeringBehaviour_Evade m_Evade;
    public SteeringBehaviour_Pursuit m_Pursuit;
    public SteeringBehaviour_Seek m_Seek;
    public SteeringBehaviour_Wander m_Wander;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Evade = GetComponent<SteeringBehaviour_Evade>();
        m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();
        m_Seek = GetComponent<SteeringBehaviour_Seek>();
        m_Wander = GetComponent<SteeringBehaviour_Wander>();

        if (!m_Evade)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
            else
            {
                m_Evade.m_EvadeRadius = P_SafetyRange;
                m_Evade.m_EvadingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
            }
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
            else
            {
                m_Pursuit.m_PursuingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
            }
        if (!m_Seek)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
        if (!m_Wander)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
    }

    public void RestoreSpirit()
    {
        P_CurrentHealth = P_MaxHealth;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        P_CurrentHealth -= Time.deltaTime * P_HealthLossParameter;
        P_HealthSlider.value = P_CurrentHealth / P_MaxHealth;
    }

    protected void CalculateSteeringImportance()
    {
        SpiritEnergy[] SpiritEnergies = GameObject.FindObjectsOfType<SpiritEnergy>();
        float SmallestSpiritDistance = 0.0f;
        foreach (SpiritEnergy Energy in SpiritEnergies)
        {
            if (SmallestSpiritDistance == 0.0f)
            {
                ClosestSpirit = Energy;
                SmallestSpiritDistance = Maths.Magnitude((Vector2)gameObject.transform.position - (Vector2)Energy.gameObject.transform.position);
            }
            else
            {
                if (SmallestSpiritDistance > Maths.Magnitude((Vector2)gameObject.transform.position - (Vector2)Energy.gameObject.transform.position))
                {
                    ClosestSpirit = Energy;
                    SmallestSpiritDistance = Maths.Magnitude((Vector2)gameObject.transform.position - (Vector2)Energy.gameObject.transform.position);
                }
            }
        }

        float PlayerDistance = Maths.Magnitude((Vector2)gameObject.transform.position - (Vector2)GameObject.Find("Player").transform.position);

        if (P_CurrentHealth / P_MaxHealth > 0.5f)
        {
            m_Seek.SteeringImportance = 0.0f;
            m_Evade.SteeringImportance = 0.0f;
            if (P_InChase)
            {
                m_Pursuit.SteeringImportance = 1.0f;
                m_Wander.SteeringImportance = 0.0f;
            } 
            else
            {
                m_Pursuit.SteeringImportance = 0.0f;
                m_Wander.SteeringImportance = 1.0f;
            }
        }
        else if (P_CurrentHealth / P_MaxHealth > 0.25f)
        {
            m_Evade.SteeringImportance = 0.0f;
            m_Seek.m_TargetPosition = (Vector2)ClosestSpirit.gameObject.transform.position;
            if (P_InChase)
            {
                m_Wander.SteeringImportance = 0.0f;
                m_Pursuit.SteeringImportance = (P_CurrentHealth / P_MaxHealth - 0.25f) / 0.25f;
                m_Seek.SteeringImportance = 1.0f - m_Pursuit.SteeringImportance;
            }
            else
            {
                m_Pursuit.SteeringImportance = 0.0f;
                m_Wander.SteeringImportance = (P_CurrentHealth / P_MaxHealth - 0.25f) / 0.25f;
                m_Seek.SteeringImportance = 1.0f - m_Wander.SteeringImportance;
            }
        }
        else
        {
            m_Wander.SteeringImportance = 0.0f;
            m_Pursuit.SteeringImportance = 0.0f;
            m_Seek.m_TargetPosition = (Vector2)ClosestSpirit.gameObject.transform.position;
            if (PlayerDistance > P_SafetyRange)
            {
                m_Evade.SteeringImportance = 0.0f;
                m_Seek.SteeringImportance = 1.0f;
            }
            else
            {
                m_Seek.SteeringImportance = PlayerDistance / P_SafetyRange;
                m_Evade.SteeringImportance = 1.0f - m_Seek.SteeringImportance;
            }
            
            
        }
    }

    protected void Update()
    {
        UpdateThreatColor();
        SeekForPlayer();
        CalculateSteeringImportance();
        if (P_CurrentLostVisionChaseTime > 0.0f) P_CurrentLostVisionChaseTime -= Time.deltaTime;
        else if (P_InChase) P_InChase = false;
    }

    protected void UpdateThreatColor()
    {
        if (P_CurrentHealth / P_MaxHealth > 0.5) P_SliderFill.color = SafeZone;
        else if (P_CurrentHealth / P_MaxHealth > 0.25) P_SliderFill.color = ThreatZone;
        else P_SliderFill.color = CriticalZone;
    }

    protected void SeekForPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, P_VisionRange);
        Vector2 P_EntityPosition = (Vector2)this.GetComponent<Transform>().position;
        Vector2 P_EntityCurrentVelocity = GetComponent<Rigidbody2D>().velocity;
        if (colliders.Length > 0)
        {
            foreach (Collider2D Es in colliders)
            {
                if (Es.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Debug.Log(P_InChase);
                    Vector2 P_PlayerPosition = (Vector2)Es.gameObject.transform.position;
                    RaycastHit2D hit = Physics2D.Raycast(P_EntityPosition, Maths.Normalise(P_PlayerPosition - P_EntityPosition), P_VisionRange);
                    Debug.Log(hit.transform);
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        Debug.Log("Inside");
                        Vector2 targetVelocity = P_PlayerPosition - P_EntityPosition;
                        float m_Dot = Maths.Dot(targetVelocity, P_EntityCurrentVelocity);
                        if (m_Dot >= -0.7f)
                        {
                            
                            P_CurrentLostVisionChaseTime = P_MaxLostVisionChaseTime;
                            P_InChase = true;
                        }
                        else break;
                    }
                    else break;
                }
            }
        }
    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }
}
