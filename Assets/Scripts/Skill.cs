using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[Serializable]
public struct DamageEvent
{
    public float Time;
    public Damage Damage;
}

[RequireComponent(typeof(NetworkIdentity))]
public class Skill : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Cooldown <= 0 means no cooldown")]
    private float Cooldown = 0f;
    [SerializeField]
    private bool isCooldownAfterSkill = true;
    // Start is called before the first frame update

    [SerializeField]
    List<DamageEvent> DamageEvents = new List<DamageEvent>();

    [SerializeField]
    private List<Damage> DamageList = new List<Damage>();

    [SyncVar]
    private float CooldownTimer = 0f;
    [SyncVar]
    private bool isRunning = false;

    public float coolDownLeft
    {
        get
        {
            return CooldownTimer;
        }
    }

    [Command]
    virtual public void exec(DamageType damageType)
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(SkillCoroutine(damageType));
    }

    virtual protected IEnumerator SkillCoroutine(DamageType damageType)
    {
        yield return null;
        isRunning = false;
        if(isCooldownAfterSkill)
        CooldownTimer = Cooldown;
    }

    [Command]
    virtual public void Stop()
    {
        if(!isRunning) return;
        StopAllCoroutines();
        isRunning = false;
        if(isCooldownAfterSkill)
        CooldownTimer = Cooldown;
    }

    void Start()
    {
        CooldownTimer = Cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (CooldownTimer > 0)
        {
            CooldownTimer -= Time.deltaTime;
        }
    }
}
