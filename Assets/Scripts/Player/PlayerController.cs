using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkController.singleton.CreateCharactor("BlackBoss");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
