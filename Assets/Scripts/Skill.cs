using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Unity.VisualScripting;

[Serializable]
public struct DamageEvent
{
    public float Time;
    public Damage Damage;
}

public class Skill : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("CoolDown <= 0 means no cool down")]
    private float CoolDown = 0f;
    [SerializeField]
    private bool isCoolDownAfterSkill = true;
    // Start is called before the first frame update

    [SerializeField]
    List<DamageEvent> DamageEvents = new List<DamageEvent>();

    [SerializeField]
    private List<Damage> DamageList = new List<Damage>();

    [SerializeField]
    private bool MoveInRunning = false;

    public bool moveInRunning
    {
        get
        {
            return MoveInRunning;
        }
    }

    [Header("Debug")]
    [SyncVar]
    [SerializeField]
    private float CoolDownTimer = 0f;
    [SyncVar]
    [SerializeField]
    private bool IsRunning = false;

    public bool isRunning {
        get {
            return IsRunning;
        }
    }

    public float coolDownLeft
    {
        get
        {
            return CoolDownTimer;
        }
    }

    [Server]
    virtual public bool exec()
    {
        if (IsRunning || CoolDownTimer > 0) return false;
        IsRunning = true;
        StartCoroutine(SkillCoroutine());
        return true;
    }

    [Server]
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

    [Server]
    virtual public void Stop()
    {
        if(!IsRunning) return;
        StopAllCoroutines();
        IsRunning = false;
        if(isCoolDownAfterSkill)
        CoolDownTimer = CoolDown;
    }

    [Server]
    virtual public void OnDamage(int damageIndex){
        if(damageIndex < DamageList.Count){
            var damage =  DamageList[damageIndex];
            damage.Exec();
        }
    }

    void Start()
    {
        CoolDownTimer = 0;
        DamageEvents.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && CoolDownTimer > 0)
        {
            CoolDownTimer -= Time.deltaTime;
        }
    }
}
