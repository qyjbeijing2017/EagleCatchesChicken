using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public abstract class Skill : NetworkBehaviour
{
    [SerializeField]
    private float Cooldown = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        if(isLocalPlayer) {

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
