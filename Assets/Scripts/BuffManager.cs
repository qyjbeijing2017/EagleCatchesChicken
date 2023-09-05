using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuffManager : NetworkBehaviour
{
    [Header("Debug")]
    public List<Buff> Buffs = new List<Buff>();

    // Slow Down Settings
    float SlowDownSpeed = 0;
    public float slowDownSpeed { get { return SlowDownSpeed; } }
    float SlowDownPer = 0;
    public float slowDownPer { get { return SlowDownPer; } }

    // Stagger Settings
    bool IsStagger = false;
    public bool isStagger { get { return IsStagger; } }

    int DamageDealt = 0;
    public int damageDealt { get { return DamageDealt; } }
    int DamageTaken = 0;
    public int damageTaken { get { return DamageTaken; } }
    int ExecuteDealtUnderHp = 0;
    public int executeDealtUnderHp { get { return ExecuteDealtUnderHp; } }
    float ExecuteDealtUnderPercent = 0;
    public float executeDealtUnderPercent { get { return ExecuteDealtUnderPercent; } }
    int ExecuteDealtDamageModifier = 0;
    public int executeDealtDamageModifier { get { return ExecuteDealtDamageModifier; } }
    bool ExecuteDealtOnce = false;
    public bool executeDealtOnce { get { return ExecuteDealtOnce; } }
    int ExecuteTakenUnderHp = 0;
    public int executeTakenUnderHp { get { return ExecuteTakenUnderHp; } }
    float ExecuteTakenUnderPercent = 0;
    public float executeTakenUnderPercent { get { return ExecuteTakenUnderPercent; } }
    int ExecuteTakenDamageModifier = 0;
    public int executeTakenDamageModifier { get { return ExecuteTakenDamageModifier; } }
    bool ExecuteTakenOnce = false;
    public bool executeTakenOnce { get { return ExecuteTakenOnce; } }
    bool Invincible = false;
    public bool invincible { get { return Invincible; } }




    Player MyPlayer;

    [Server]
    public Buff AddBuff(Buff buff, int murdererId)
    {
        var buffInstance = Instantiate(buff);
        NetworkServer.Spawn(buffInstance.gameObject);
        buffInstance.VictimId = MyPlayer.PlayerId;
        buffInstance.MurdererId = murdererId;
        return buffInstance;
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
            DamageDealt += buff.DamageDealt;
            DamageTaken += buff.DamageTaken;
            ExecuteDealtUnderHp = Mathf.Max(ExecuteDealtUnderHp, buff.ExecuteDealtUnderHp);
            ExecuteDealtUnderPercent = Mathf.Max(ExecuteDealtUnderPercent, buff.ExecuteDealtUnderPercent);
            ExecuteDealtDamageModifier = Mathf.Max(ExecuteDealtDamageModifier, buff.ExecuteDealtDamageModifier);
            ExecuteDealtOnce |= buff.ExecuteDealtOnce;
            ExecuteTakenUnderHp = Mathf.Max(ExecuteTakenUnderHp, buff.ExecuteTakenUnderHp);
            ExecuteTakenUnderPercent = Mathf.Max(ExecuteTakenUnderPercent, buff.ExecuteTakenUnderPercent);
            ExecuteTakenDamageModifier = Mathf.Max(ExecuteTakenDamageModifier, buff.ExecuteTakenDamageModifier);
            ExecuteTakenOnce |= buff.ExecuteTakenOnce;
            Invincible |= buff.Invincible;

        }
    }
}
