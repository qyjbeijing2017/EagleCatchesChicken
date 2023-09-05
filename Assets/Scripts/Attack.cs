using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Attack : Skill
{
    [Header("Attack")]
    [SerializeField]
    ColiderEvent ReadyCollider;
    [SerializeField]
    ColiderEvent AttackCollider;
    [SerializeField]
    LayerMask Target;

    [Header("Debug")]
    [SyncVar]
    public bool IsReady;
    public event Action onAttack;
    [SerializeField]
    int ReadyCount = 0;


    void FixedUpdate()
    {
        if (!isServer) return;
        IsReady = ReadyCount > 0;
        ReadyCount = 0;

    }
    void OnReadyTriggerStay(Collider collider)
    {
        if (!isServer) return;
        if ((Target.value & (1 << collider.gameObject.layer)) == 0) return;
        ReadyCount++;
    }

    void OnAttackTriggerEnter(Collider collider)
    {
        if (!isServer) return;
        if ((Target.value & (1 << collider.gameObject.layer)) == 0) return;
        if(Exec(PlayerId)){
            onAttack?.Invoke();
        }
    }

    [Server]
    override public void Stop()
    {
        var playerID = PlayerId;
        base.Stop();
        PlayerId = playerID;
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        if (isServer)
        {
            PlayerId = GetComponentInParent<Player>().PlayerId;
            AttackCollider.onTriggerEnter += OnAttackTriggerEnter;
            ReadyCollider.onTriggerStay += OnReadyTriggerStay;
        }
    }
}
