using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Damage : NetworkBehaviour
{
    [HideInInspector]
    public DamageType Type = DamageType.None;
    [SerializeField]
    private LayerMask Target = 0;

    [Header("Damage Enter")]
    [SerializeField]
    private int DamageAmountOnEnter = 0;
    [SerializeField]
    private List<Buff> BuffsOnEnter = new List<Buff>();
    [SerializeField]
    private Vector3 ImpulseOnEnter = Vector3.zero;
    [SerializeField]
    private ImpulseMode IsLocalImpulseOnEnter = ImpulseMode.LookAtTarget;

    [Header("Buff Stay")]
    [SerializeField]
    [Tooltip("These buffs will be add on Enter, removed on exit")]
    private List<Buff> BuffsOnStay = new List<Buff>();

    [Header("Damage Exit")]
    [SerializeField]
    private int DamageAmountOnExit = 0;
    [SerializeField]
    private List<Buff> BuffsOnExit = new List<Buff>();
    [SerializeField]
    private Vector3 ImpulseOnExit = Vector3.zero;
    [SerializeField]
    private ImpulseMode IsLocalImpulseOnExit = ImpulseMode.LookAtTarget;

    List<Buff> BuffsNeedRemove = new List<Buff>();

    List<Collider> triggers = new List<Collider>();

    Animator animator = null;

    static public Vector3 ImpulseModeToGlobal(ImpulseMode mode, Vector3 impulse, Transform me, Transform target)
    {
        switch(mode)
        {
            case ImpulseMode.LookAtTarget:
                var direction = target.position - me.position;
                direction.Normalize();
                var rotation = Quaternion.LookRotation(direction, Vector3.up);          
                return rotation * impulse;
            case ImpulseMode.Local:
                return me.rotation * impulse;
            case ImpulseMode.Global:
                return impulse;
        }
        return impulse;
    }

    [Server]
    int DamageModifier(int damage)
    {
        if(MurdererBuffManager == null) return damage;
        return damage + MurdererBuffManager.damageDealt;
    }

    // Start is called before the first frame update
    virtual protected void OnTriggerEnter(Collider other)
    {
        if(!isServer) return;
        if((Target.value & (1 << other.gameObject.layer)) == 0) return;
        var source = other.GetComponent<Source>();
        if(DamageAmountOnEnter > 0 && source != null)
        {
            source.TakeDamage(DamageModifier(DamageAmountOnEnter));
        }

        var rigidbody = other.GetComponent<Rigidbody>();
        if(ImpulseOnEnter != Vector3.zero)
        {
            if(source != null){
                var impulse = ImpulseModeToGlobal(IsLocalImpulseOnEnter, ImpulseOnEnter, transform, other.transform);
                rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }

        var buffManager = other.GetComponent<BuffManager>();
        if(buffManager == null) return;
        foreach(var buff in BuffsOnEnter)
        {
            buffManager.AddBuff(buff, Murderer.PlayerId);
        }

        foreach(var buff in BuffsOnStay)
        {
            var buffInstance = buffManager.AddBuff(buff, Murderer.PlayerId);
            BuffsNeedRemove.Add(buffInstance);
        }
    }

    virtual protected void OnTriggerExit(Collider other)
    {
        if(!isServer) return;
        if((Target.value & (1 << other.gameObject.layer)) == 0) return;


        var source = other.GetComponent<Source>();
        if(DamageAmountOnExit > 0 && source != null)
        {
            source.TakeDamage(DamageModifier(DamageAmountOnExit));
        }

        var rigidbody = other.GetComponent<Rigidbody>();
        if(ImpulseOnExit != Vector3.zero && rigidbody != null)
        {
            if(source != null){
                var impulse = ImpulseModeToGlobal(IsLocalImpulseOnExit, ImpulseOnExit, transform, other.transform);
                rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }

        var buffManager = other.GetComponent<BuffManager>();
        if(buffManager == null) return;

        foreach(var buff in BuffsOnExit)
        {
            buffManager.AddBuff(buff, Murderer.PlayerId);
        }

        foreach(var buff in BuffsNeedRemove)
        {
            if(buff.buffManager == buffManager)
            {
                buff.RemoveBuff();
            }
        }
    }

    protected Skill CurrentSkill = null;
    protected Player Murderer = null;
    protected BuffManager MurdererBuffManager = null;

    [Server]
    virtual public void Exec(Skill skill) {
        this.CurrentSkill = skill;
        Murderer = skill.murderer;
        if(Murderer)
            MurdererBuffManager = Murderer.GetComponent<BuffManager>();
        foreach(var trigger in triggers)
        {
            trigger.enabled = true;
        }
        if(animator != null)
            animator.enabled = true;
    }

    [Server]
    virtual public void Stop() {
        this.CurrentSkill = null;
        this.Murderer = null;
        this.MurdererBuffManager = null;
        foreach(var trigger in triggers)
        {
            trigger.enabled = false;
        }
        if(animator != null)
            animator.enabled = false;
    }

    virtual protected void Awake()
    {
        var colliders = GetComponents<Collider>();
        var animator = GetComponent<Animator>();
        if(animator != null)
            animator.enabled = false;
        foreach(var collider in colliders)
        {
            if(collider.isTrigger)
            {
                triggers.Add(collider);
                collider.enabled = false;
            }
        }
    }
}
