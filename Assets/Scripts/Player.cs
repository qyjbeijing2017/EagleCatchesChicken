using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Experimental;

[RequireComponent(typeof(Move))]
[RequireComponent(typeof(Source))]
[RequireComponent(typeof(BuffManager))]
[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(AnimatorManager))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkRigidbody))]
[RequireComponent(typeof(SkillManager))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    public int PlayerId;
    // Start is called before the first frame update
    void Start()
    {
        if(isClientOnly) {
            ECCNetworkManager.instance.PlayerList.Add(this);
            ECCNetworkManager.instance.PlayerList.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
