using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
public class Buff : NetworkBehaviour
{
    [SerializeField]
    public BuffScriptableObject buffConfig;

    [SyncVar]
    private PlayerBuff m_Target;
    public PlayerBuff target => m_Target;
    public PlayerHealth m_TargetHealth;
    public PlayerHealth targetHealth {
        get {
            if(m_TargetHealth == null && m_Target != null) {
                m_TargetHealth = m_Target.GetComponent<PlayerHealth>();
            }
            return m_TargetHealth;
        }
    }

    [SyncVar]
    private PlayerController m_Owner;
    public PlayerController owner => m_Owner;


    [SyncVar]
    float m_StartTime;
    public float startTime => m_StartTime;

    public bool isActivated => Time.time - m_StartTime < buffConfig.Duration;

    [Server]
    public void StartBuff(PlayerBuff target, PlayerController owner)
    {
        m_Target = target;
        m_Owner = owner;
        target.Buffs.Add(this);
        m_StartTime = Time.time;
        if(buffConfig.BeDamageOverTime != 0) {
            StartCoroutine(DamageOverTime());
        }
    }

    [Server]
    IEnumerator DamageOverTime()
    {
        while(isActivated) {
            targetHealth.BeAttacked(new Attack() {
                Damage = buffConfig.BeDamageOverTime,
                KnockbackDistance = buffConfig.KnockbackDistance,
                KnockbackDuration = buffConfig.KnockbackDuration,
                KnockoffInitialVelocity = buffConfig.KnockoffInitialVelocity,
                KnockoffDuration = buffConfig.KnockoffDuration,
                Buffs = new System.Collections.Generic.List<Buff>(),
            }, owner);
            yield return new WaitForSeconds(buffConfig.Interval);
        }
    }

    [Server]
    public void EndBuff()
    {
        m_StartTime = -1000;
        m_Target.Buffs.Remove(this);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActivated) {
            EndBuff();
            return;
        }
        transform.position = m_Target.transform.position;
        transform.rotation = m_Target.transform.rotation;
        transform.localScale = m_Target.transform.localScale;
    }
} 
