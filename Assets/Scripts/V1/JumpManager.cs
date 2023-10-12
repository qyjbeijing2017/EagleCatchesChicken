using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class JumpManager : MonoBehaviour
{
    public event System.Action onGrounded;
    public event System.Action onJump;

    void OnTriggerEnter(Collider other)
    {
        onGrounded?.Invoke();
    }

    void OnTriggerExit(Collider other){
        onJump?.Invoke();
    }

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Reset() {
        GetComponent<Collider>().isTrigger = true;
    }
}
