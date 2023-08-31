using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class Buff : NetworkBehaviour
{
    Source PlayerSource;
    BuffManager PlayerBuffManager;

    [Tooltip("Duration <= 0 means infinite")]
    public float Duration = 10;

    [Header("Dot Settings")]
    [Tooltip("Dot <= 0 means add hp")]
    public int Dot = 0;
    public Vector3 Impulse = Vector3.zero;
    private ImpulseMode ImpulseMode = ImpulseMode.DamageToTarget;
    public float DotTick = 1;

    [Header("Slow Down Settings")]
    [Tooltip("SlowDownSpeed <= 0 means speed up")]
    public float SlowDownSpeed = 0;
    public float SlowDownPer = 0;

    [Header("Stagger Settings")]
    public bool IsStagger = false;

    [Header("Damage Modifier Settings")]
    public int DamageDealt = 0;
    public int DamageTaken = 0;

    [Header("Execute Dealt Settings")]
    public int ExecuteDealtUnderHp = 0;
    public float ExecuteDealtUnderPercent = 0;
    public int ExecuteDealtDamageModifier = 0;
    public bool ExecuteDealtOnce = false;

    [Header("Execute Taken Settings")]
    public int ExecuteTakenUnderHp = 0;
    public float ExecuteTakenUnderPercent = 0;
    public int ExecuteTakenDamageModifier = 0;
    public bool ExecuteTakenOnce = false;

    [Header("Invincible Setting")]
    public bool Invincible = false;

    [Header("Debug")]
    [SyncVar]
    public int VictimId = -1;
    [SyncVar]
    public int MurdererId = -1;
    [SyncVar]
    [SerializeField]
    float TimeLeft;


    public Player victim{
        get{
            if(VictimId < 0) return null;
            if(VictimId >= ECCNetworkManager.instance.PlayerList.Count) return null;
            return ECCNetworkManager.instance.PlayerList[VictimId];
        }
    }

    public Player murderer{
        get{
            if(MurdererId < 0) return null;
            if(MurdererId >= ECCNetworkManager.instance.PlayerList.Count) return null;
            return ECCNetworkManager.instance.PlayerList[MurdererId];
        }
    }

    public BuffManager buffManager{
        get{
            return PlayerBuffManager;
        }
    }

    void Start()
    {
        if(victim == null) return;
        PlayerBuffManager = victim.GetComponent<BuffManager>();
        PlayerBuffManager.Buffs.Add(this);
        if (isServer)
        {
            TimeLeft = Duration;
            PlayerSource = victim.GetComponent<Source>();
            if (Dot > 0 || Impulse != Vector3.zero)
            {
                StartCoroutine(DotCoroutine());
            }
        }

    }

    void Update()
    {
        if (isServer)
        {
            if (Duration > 0)
            {
                TimeLeft -= Time.deltaTime;
                if (TimeLeft <= 0)
                {
                    RemoveBuff();
                }
            }
        }
        transform.position = victim.transform.position;
        transform.rotation = victim.transform.rotation;
    }

    IEnumerator DotCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(DotTick);
            PlayerSource.TakeDamage(Dot, murderer, null, this);
            if(Impulse != Vector3.zero)
            {
                var impulse = Damage.ImpulseModeToGlobal(ImpulseMode, Impulse, transform, victim.transform, murderer.transform);
                var rigidbody = victim.GetComponent<Rigidbody>();
                if(rigidbody != null)
                {
                    rigidbody.AddForce(impulse, ForceMode.Impulse);
                }
            }
        }
    }

    [Server]
    public void RemoveBuff()
    {
        PlayerBuffManager.Buffs.Remove(this);
        StopAllCoroutines();
        RpcRemoveBuff();
        NetworkServer.Destroy(gameObject);
    }

    public void RpcRemoveBuff()
    {
        PlayerBuffManager.Buffs.Remove(this);
    }
}
