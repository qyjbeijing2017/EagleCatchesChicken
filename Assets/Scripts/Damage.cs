using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
public abstract class Damage : NetworkBehaviour
{
    [HideInInspector]
    public DamageType Type = DamageType.None;
    [SerializeField]
    private LayerMask Target = 0;
    [SerializeField]
    protected bool ExecOnStart = false;

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
                return me.TransformVector(impulse);
            case ImpulseMode.Global:
                return impulse;
        }
        return impulse;
    }

    // Start is called before the first frame update
    virtual protected void OnTriggerEnter(Collider other)
    {
        if(!isServer) return;
        if((Target.value & (1 << other.gameObject.layer)) == 0) return;

        var source = other.GetComponent<Source>();
        if(DamageAmountOnEnter > 0 && source != null)
        {
            source.TakeDamage(DamageAmountOnEnter);
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
            Instantiate(buff, buffManager.transform);
        }

        foreach(var buff in BuffsOnStay)
        {
            var buffInstance = Instantiate(buff, buffManager.transform);
            BuffsNeedRemove.Add(buffInstance);
            BuffsOnStay.Remove(buff);
        }
    }

    virtual protected void OnTriggerExit(Collider other)
    {
        if(!isServer) return;
        if((Target.value & (1 << other.gameObject.layer)) == 0) return;


        var source = other.GetComponent<Source>();
        if(DamageAmountOnExit > 0 && source != null)
        {
            source.TakeDamage(DamageAmountOnExit);
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
            Instantiate(buff, buffManager.transform);
        }

        foreach(var buff in BuffsNeedRemove)
        {
            if(buff.buffManager == buffManager)
            {
                buff.RemoveBuff();
            }
        }
    }

    virtual public void Exec() {
        foreach(var trigger in triggers)
        {
            trigger.enabled = true;
        }
        if(animator != null)
            animator.enabled = true;

    }

    virtual public void Stop() {
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

    virtual protected void Start()
    {

        if(ExecOnStart){
            Exec();
        }
    }
}
