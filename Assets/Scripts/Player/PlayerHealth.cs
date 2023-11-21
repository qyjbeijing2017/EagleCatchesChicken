using System.Collections;
using Mirror;
using NPOI.HSSF.Record.Chart;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerHealth : PlayerComponent
{
    [SerializeField]
    [SyncVar]
    private int m_Health;
    public int health
    {
        get => m_Health;
        private set
        {
            m_Health = value;
            if (m_Health <= 0)
            {
                m_IsDead = true;
                m_IsKnockedBackTime = 0;
                m_IsKnockedOffTime = 0;
            }
        }
    }

    [SerializeField]
    private Transform m_HealthBar;
    public Transform healthBar => m_HealthBar;

    [SerializeField]
    [SyncVar]
    private float m_IsKnockedBackTime;
    public bool isKnockedBack => m_IsKnockedBackTime > 0;

    [SyncVar]


    [SerializeField]
    private bool m_IsDead;
    public bool isDead => m_IsDead;

    [SerializeField]
    [SyncVar]
    private float m_IsKnockedOffTime;
    public bool isKnockedOff => m_IsKnockedOffTime > 0;


    PlayerMove m_PlayerMove;
    PlayerBuff m_PlayerBuff;

    void Start()
    {
        m_PlayerMove = GetComponent<PlayerMove>();
        m_PlayerBuff = GetComponent<PlayerBuff>();
    }

    [Server]
    public void BeAttacked(Attack attack, PlayerController owner)
    {
        if (m_IsDead) return;
        m_Health -= attack.Damage;
        m_IsKnockedBackTime = attack.KnockbackDuration;
        var backWorldPosition = owner.transform.TransformPoint(attack.KnockbackDistance);
        m_PlayerMove.AddMovePosition(backWorldPosition);
        m_IsKnockedOffTime = attack.KnockoffDuration;
        var offWorldVelocity = owner.transform.TransformVector(attack.KnockoffInitialVelocity);
        m_PlayerMove.AddVelocity(offWorldVelocity);
        foreach (var buff in attack.Buffs)
        {

        }
    }

    public void Update()
    {
        if (isDead) return;
        if (isKnockedBack)
        {
            m_IsKnockedBackTime -= Time.deltaTime;
        }

        if (isKnockedOff)
        {
            m_IsKnockedOffTime -= Time.deltaTime;
        }

    }
}
