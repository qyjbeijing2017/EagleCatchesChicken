using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColiderEvent : MonoBehaviour
{
    public event System.Action<Collider> onTriggerEnter;
    public event System.Action<Collider> onTriggerStay;
    public event System.Action<Collider> onTriggerExit;

    public event System.Action<Collision> onCollisionEnter;
    public event System.Action<Collision> onCollisionStay;
    public event System.Action<Collision> onCollisionExit;

    void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }
    void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        onCollisionStay?.Invoke(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        onCollisionExit?.Invoke(collision);
    }
}
