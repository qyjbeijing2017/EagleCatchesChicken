using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface BuffAttribute
{
    public float speedMultiplier { get; }
    public float speedAddition { get; }
    public float damageMultiplier { get; }
    public float damageAddition { get; }


}

[RequireComponent(typeof(NetworkIdentity))]
public class Buff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
