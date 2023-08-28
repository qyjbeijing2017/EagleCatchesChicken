using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

[RequireComponent(typeof(NetworkIdentity))]
public class Buff : PoolAble<Buff>
{
    Source PlayerSource;
    BuffManager PlayerBuffManager;

    [Tooltip("Duration <= 0 means infinite")]
    public float Duration = 10;

    [Header("Dot Settings")]
    [Tooltip("Dot <= 0 means add hp")]
    public int Dot = 0;
    public float DotTick = 1;

    [Header("Slow Down Settings")]
    [Tooltip("SlowDownSpeed <= 0 means speed up")]
    public float SlowDownSpeed = 0;
    public float SlowDownPer = 0;

    [Header("Stagger Settings")]
    public bool IsStagger = false;

    Player MyPlayer;
    Player BuffFrom;

    public Player player{
        get{
            return MyPlayer;
        }
    }

    public Player buffFrom{
        get{
            return BuffFrom;
        }
    }

    public BuffManager buffManager{
        get{
            return PlayerBuffManager;
        }
    }

    [SyncVar]
    float TimeLeft;

    [ClientRpc]
    public void From(int playerId){
        var eccNetworkManager = ECCNetworkManager.singleton as ECCNetworkManager;
        MyPlayer = eccNetworkManager.PlayerList[playerId];
    }
    void OnEnable()
    {
        Debug.Log("Buff OnEnable");
        PlayerBuffManager = MyPlayer.GetComponent<BuffManager>();
        PlayerBuffManager.Buffs.Add(this);
        if (isServer)
        {
            TimeLeft = Duration;
            PlayerSource = MyPlayer.GetComponent<Source>();
        }
        if (Dot > 0)
        {
            StartCoroutine(DotCoroutine());
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
    }

    IEnumerator DotCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(DotTick);
            PlayerSource.TakeDamage(Dot);
        }
    }

    public void RemoveBuff()
    {
        PlayerBuffManager.Buffs.Remove(this);
        if (isServer)
        {
            StopAllCoroutines();
            RemoveInstance();
        }
    }
}
