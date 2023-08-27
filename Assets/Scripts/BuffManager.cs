using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuffManager : NetworkBehaviour
{
    [HideInInspector]
    public List<Buff> Buffs = new List<Buff>();

    // Slow Down Settings
    float SlowDownSpeed = 0;
    public float slowDownSpeed { get { return SlowDownSpeed; } }
    float SlowDownPer = 0;
    public float slowDownPer { get { return SlowDownPer; } }

    // Stagger Settings
    bool IsStagger = false;
    public bool isStagger { get { return IsStagger; } }

    public Buff AddBuff(Buff buff)
    {
        var buffInstance = Instantiate(buff, transform);
        if (isServer)
        {
            NetworkServer.Spawn(buffInstance.gameObject);
        }
        return buffInstance;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SlowDownSpeed = 0;
        SlowDownPer = 0;
        IsStagger = false;
        foreach (var buff in Buffs)
        {
            SlowDownSpeed += buff.SlowDownSpeed;
            SlowDownPer += buff.SlowDownPer;
            IsStagger |= buff.IsStagger;
        }
    }
}
