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

    Player MyPlayer;

    public void AddBuff(Buff buff)
    {
        if (isServer)
        {
            var buffInstance = buff.GetInstance(transform.position, transform.rotation);
            buffInstance.From(MyPlayer.PlayerID);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MyPlayer = GetComponent<Player>();

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
