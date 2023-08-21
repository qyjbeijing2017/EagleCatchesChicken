using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Move))]
[RequireComponent(typeof(Source))]
[RequireComponent(typeof(BuffManager))]
[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(MoveFromAnimator))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    public uint PlayerID;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
