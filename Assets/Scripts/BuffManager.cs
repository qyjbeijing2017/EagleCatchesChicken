using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuffManager : NetworkBehaviour
{
    [SyncVar]
    public List<Buff> BuffList = new List<Buff>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
