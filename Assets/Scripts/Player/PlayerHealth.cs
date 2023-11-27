using System.Collections;
using Mirror;
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

    public float healthPercent => (float)health / playerConfig.MaxHealth;

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
        if(m_HealthBar == null)
        {
            m_HealthBar = new GameObject("HealthBar").transform;
            m_HealthBar.SetParent(transform);
            m_HealthBar.localPosition = new Vector3(0, 2.8f, 0);
        }
        if (isServer)
        {
            health = playerConfig.MaxHealth;
        }
    }

    [Server]
    public void BeAttacked(IAttack attack, PlayerController owner, Bullet bullet = null)
    {
        if (m_IsDead) return;
        StartCoroutine(HandleAttack(attack, owner, bullet));
    }

    IEnumerator HandleAttack(IAttack attack, PlayerController owner, Bullet bullet)
    {
        m_Health -= attack.Damage;
        var originTransfrom = bullet == null ? owner.transform : bullet.transform;
        if (attack.KnockbackDuration > 0)
        {
            m_IsKnockedBackTime = attack.KnockbackDuration;
            yield return new WaitForSeconds(attack.KnockbackDuration);
            var backWorldPosition = originTransfrom.rotation * attack.KnockbackDistance;
            m_PlayerMove.AddMovePosition(backWorldPosition);
        }
        if (attack.KnockoffDuration > 0)
        {
            m_IsKnockedOffTime = attack.KnockoffDuration;
            var offWorldVelocity = originTransfrom.rotation * attack.KnockoffInitialVelocity;
            m_PlayerMove.AddVelocity(offWorldVelocity);
        }
        foreach (var buff in attack.Buffs)
        {
            if(buff == null)
            {
                continue;
            }
            Instantiate(buff, transform.position, Quaternion.identity).StartBuff(m_PlayerBuff, owner);
        }
        yield return null;
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
