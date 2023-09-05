using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Telepathy;

public class Source : NetworkBehaviour
{
    [SerializeField]
    int MaxHealth = 100;
    public Transform HealthBarAnchor;
    public event System.Action<int, int> OnHealthChanged;
    public event System.Action OnHealthZero;


    [Header("Debug")]
    [SerializeField]
    [SyncVar]
    int Health = 100;

    public float healthPercent
    {
        get
        {
            return (float)Health / MaxHealth;
        }
    }

    BuffManager PlayerBuffManager;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            Health = MaxHealth;
            PlayerBuffManager = GetComponent<BuffManager>();
        }
    }

    [Server]
    private void HandleExecute(ref int hpDamage, BuffManager buffManager, bool isDealt)
    {
        if (isDealt)
        {
            if (buffManager.executeDealtUnderHp > Health || Health / MaxHealth < buffManager.executeDealtUnderPercent)
            {
                if (buffManager.executeDealtOnce)
                {
                    hpDamage = Health;
                }
                else
                {
                    hpDamage += buffManager.executeDealtDamageModifier;
                }
            }
        }
        else
        {
            if (buffManager.executeTakenUnderHp > Health || Health / MaxHealth < buffManager.executeTakenUnderPercent)
            {
                if (buffManager.executeTakenOnce)
                {
                    hpDamage = Health;
                }
                else
                {
                    hpDamage += buffManager.executeTakenDamageModifier;
                }
            }
        }
    }


    public void TakeDamage(int damage, Player murderer, Skill skill, Buff buff)
    {
        if (isServer)
        {
            var hpDamage = damage + PlayerBuffManager.damageTaken;
            if (PlayerBuffManager.invincible && hpDamage > 0) return;
            var murderBuffManager = murderer.GetComponent<BuffManager>();
            HandleExecute(ref hpDamage, murderBuffManager, true);
            HandleExecute(ref hpDamage, PlayerBuffManager, false);


            Health -= hpDamage;
            if (Health <= 0)
            {
                Health = 0;
                NetworkServer.Destroy(gameObject);
            }
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
