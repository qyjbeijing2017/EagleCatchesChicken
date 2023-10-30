using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(MoveController))]
public class ActorController : NetworkBehaviour
{
    public CharacterScriptableObject CharacterConfig;

    [HideInInspector]
    public PlayerController PlayerController;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
