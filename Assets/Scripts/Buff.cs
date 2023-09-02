using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class Buff : NetworkBehaviour
{
    Source PlayerSource;
    BuffManager PlayerBuffManager;

    public float Duration = 10;

    [Header("Dot Settings")]
    public int Dot = 0;
    public float DotTick = 1;

    [Header("Slow Down Settings")]
    public float SlowDownSpeed = 0;
    public float SlowDownPer = 0;

    [Header("Stagger Settings")]
    public bool IsStagger = false;


    [SyncVar]
    float TimeLeft;

    void Start()
    {
        PlayerBuffManager = transform.parent.GetComponent<BuffManager>();
        PlayerBuffManager.Buffs.Add(this);
        if(isServer) {
            TimeLeft = Duration;
            PlayerSource = GetComponent<Source>();
        }
        if(Dot > 0) {
            StartCoroutine(DotCoroutine());
        }
    }

    void Update()
    {
        if(isServer) {
            TimeLeft -= Time.deltaTime;
            if(TimeLeft <= 0) {
                Destroy(gameObject);
            }
        }
    }


    IEnumerator DotCoroutine()
    {
        while(true) {
            yield return new WaitForSeconds(DotTick);
            PlayerSource.TakeDamage(Dot);
        }
    }

    void OnDestroy()
    {
        PlayerBuffManager.Buffs.Remove(this);
    }
}
