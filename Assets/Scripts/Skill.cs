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

    SkillManager PlayerSkillManager = null;

    public float coolDownLeft
    {
        get
        {
            return CooldownTimer;
        }
    }

    virtual public void exec()
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(SkillCoroutine());

    }

    virtual protected IEnumerator SkillCoroutine()
    {
        foreach (DamageEvent damageEvent in DamageEvents)
        {
            yield return new WaitForSeconds(damageEvent.Time);
            Damage damage = damageEvent.Damage;
            damage.Exec();
        }
        yield return null;
    }

    virtual public void Stop()
    {
        if(!isRunning) return;
        StopAllCoroutines();
        isRunning = false;
        if(isCooldownAfterSkill)
        CooldownTimer = Cooldown;
    }

    virtual public void OnDamage(int damageIndex){
        if(damageIndex < DamageList.Count){
            var damage =  DamageList[damageIndex];
            damage.Exec();
        }
    }

    void Start()
    {
        PlayerSkillManager = GetComponentInParent<SkillManager>();
        CooldownTimer = Cooldown;
        DamageEvents.Sort((a, b) => a.Time.CompareTo(b.Time));
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
