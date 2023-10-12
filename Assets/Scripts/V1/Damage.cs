using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Damage : NetworkBehaviour
{
    [HideInInspector]
    public DamageType Type = DamageType.None;
    [SerializeField]
    protected LayerMask Target = 0;
    [SerializeField]
    protected bool StopOnSkillStopped = false;

    [Header("Damage Enter")]
    [SerializeField]
    private int DamageAmountOnEnter = 0;
    [SerializeField]
    private List<Buff> BuffsOnEnter = new List<Buff>();
    [SerializeField]
    private List<Buff> BuffsOnEnterForMyself = new List<Buff>();
    [SerializeField]
    private Vector3 ImpulseOnEnter = Vector3.zero;
    [SerializeField]
    private ImpulseMode IsLocalImpulseOnEnter = ImpulseMode.DamageToTarget;

    [Header("Buff Stay")]
    [SerializeField]
    [Tooltip("These buffs will be add on Enter, removed on exit")]
    private List<Buff> BuffsOnStay = new List<Buff>();
    [SerializeField]
    private List<Buff> BuffsOnStayForMyself = new List<Buff>();

    [Header("Damage Exit")]
    [SerializeField]
    private int DamageAmountOnExit = 0;
    [SerializeField]
    private List<Buff> BuffsOnExit = new List<Buff>();
    [SerializeField]
    private List<Buff> BuffsOnExitForMyself = new List<Buff>();
    [SerializeField]
    private Vector3 ImpulseOnExit = Vector3.zero;
    [SerializeField]
    private ImpulseMode IsLocalImpulseOnExit = ImpulseMode.DamageToTarget;

    List<Buff> BuffsNeedRemove = new List<Buff>();
    List<Buff> BuffsNeedRemoveForMyself = new List<Buff>();

    List<Collider> triggers = new List<Collider>();

    Animator animator = null;

    static public Vector3 ImpulseModeToGlobal(ImpulseMode mode, Vector3 impulse, Transform damage, Transform target, Transform player)
    {
        switch (mode)
        {
            case ImpulseMode.DamageToTarget:
                var direction = target.position - damage.position;
                direction.Normalize();
                var rotation = Quaternion.LookRotation(direction, Vector3.up);
                return rotation * impulse;
            case ImpulseMode.DamageLocal:
                return damage.rotation * impulse;
            case ImpulseMode.Global:
                return impulse;
            case ImpulseMode.PlayerToTarget:
                var direction2 = target.position - player.position;
                direction2.Normalize();
                var rotation2 = Quaternion.LookRotation(direction2, Vector3.up);
                return rotation2 * impulse;
            case ImpulseMode.PlayerLocal:
                return player.rotation * impulse;
        }
        return impulse;
    }

    [Server]
    int DamageModifier(int damage)
    {
        if (MurdererBuffManager == null) return damage;
        return damage + MurdererBuffManager.damageDealt;
    }

    // Start is called before the first frame update
    virtual protected void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if ((Target.value & (1 << other.gameObject.layer)) == 0) return;
        var source = other.GetComponent<Source>();
        if (DamageAmountOnEnter > 0 && source != null)
        {
            source.TakeDamage(DamageModifier(DamageAmountOnEnter), Murderer, CurrentSkill, null);
        }

        var rigidbody = other.GetComponent<Rigidbody>();
        if (ImpulseOnEnter != Vector3.zero)
        {
            if (source != null)
            {
                var impulse = ImpulseModeToGlobal(IsLocalImpulseOnEnter, ImpulseOnEnter, transform, other.transform, Murderer.transform);
                rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }

        var buffManager = other.GetComponent<BuffManager>();
        if (buffManager == null) return;
        foreach (var buff in BuffsOnEnter)
        {
            buffManager.AddBuff(buff, Murderer.PlayerId);
        }

        foreach (var buff in BuffsOnStay)
        {
            var buffInstance = buffManager.AddBuff(buff, Murderer.PlayerId);
            BuffsNeedRemove.Add(buffInstance);
        }

        foreach (var buff in BuffsOnEnterForMyself)
        {
            MurdererBuffManager.AddBuff(buff, Murderer.PlayerId);
        }
    }

    virtual protected void OnTriggerExit(Collider other)
    {
        if (!isServer) return;
        if ((Target.value & (1 << other.gameObject.layer)) == 0) return;


        var source = other.GetComponent<Source>();
        if (DamageAmountOnExit > 0 && source != null)
        {
            source.TakeDamage(DamageModifier(DamageAmountOnExit), Murderer, CurrentSkill, null);
        }

        var rigidbody = other.GetComponent<Rigidbody>();
        if (ImpulseOnExit != Vector3.zero && rigidbody != null)
        {
            if (source != null)
            {
                var impulse = ImpulseModeToGlobal(IsLocalImpulseOnExit, ImpulseOnExit, transform, other.transform, Murderer.transform);
                rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }

        var buffManager = other.GetComponent<BuffManager>();
        if (buffManager == null) return;

        foreach (var buff in BuffsOnExit)
        {
            buffManager.AddBuff(buff, Murderer.PlayerId);
        }

        foreach (var buff in BuffsNeedRemove)
        {
            if (buff.buffManager == buffManager)
            {
                buff.RemoveBuff();
            }
        }

        foreach (var buff in BuffsOnExitForMyself)
        {
            MurdererBuffManager.AddBuff(buff, Murderer.PlayerId);
        }
    }

    protected Skill CurrentSkill = null;
    protected Player Murderer = null;
    protected BuffManager MurdererBuffManager = null;

    [Server]
    virtual public void Exec(Skill skill)
    {
        this.CurrentSkill = skill;
        Murderer = skill.murderer;
        if (Murderer)
            MurdererBuffManager = Murderer.GetComponent<BuffManager>();

        foreach (var buff in BuffsOnStayForMyself)
        {
            var buffInstance = MurdererBuffManager.AddBuff(buff, Murderer.PlayerId);
            BuffsNeedRemoveForMyself.Add(buffInstance);
        }

        foreach (var trigger in triggers)
        {
            trigger.enabled = true;
        }
        if (animator != null)
            animator.enabled = true;
    }

    [Server]
    virtual public void Stop()
    {
        this.CurrentSkill = null;
        this.Murderer = null;
        this.MurdererBuffManager = null;
        foreach (var trigger in triggers)
        {
            trigger.enabled = false;
        }
        if (animator != null)
            animator.enabled = false;

        foreach (var buff in BuffsOnStayForMyself)
        {
            buff.RemoveBuff();
        }
    }

    virtual protected void Awake()
    {
        var colliders = GetComponents<Collider>();
        var animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                triggers.Add(collider);
                collider.enabled = false;
            }
        }
    }
}