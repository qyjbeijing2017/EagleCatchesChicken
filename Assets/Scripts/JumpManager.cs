using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class JumpManager : MonoBehaviour
{
    public event System.Action onGrounded;

    private float Height;

    public float height {
        get {
            return Height;
        }
    }
    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        onGrounded?.Invoke();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        var ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground"))) {
            Height = hit.distance;
        }
    }
}
