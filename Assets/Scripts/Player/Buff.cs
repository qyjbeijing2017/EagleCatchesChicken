using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
public class Buff : NetworkBehaviour
{
    [SerializeField]
    public BuffScriptableObject buffConfig;

    [SyncVar]
    private PlayerBuff m_Target;
    public PlayerBuff target => m_Target;

    [SyncVar]
    private PlayerBuff m_Owner;
    public PlayerBuff owner => m_Owner;


    [SyncVar]
    float m_StartTime;
    public float startTime => m_StartTime;

    public bool isActivated => Time.time - m_StartTime < buffConfig.Duration;

    [Server]
    public void StartBuff(PlayerBuff target, PlayerBuff owner)
    {
        m_Target = target;
        m_Owner = owner;
        target.Buffs.Add(this);
        m_StartTime = Time.time;
        enabled = true;
        if(buffConfig.BeDamageOverTime != 0) {
            StartCoroutine(DamageOverTime());
        }
    }

    [Server]
    IEnumerator DamageOverTime()
    {
        while(isActivated) {
            yield return new WaitForSeconds(buffConfig.Interval);
        }
    }

    [Server]
    public void EndBuff()
    {
        m_StartTime = -1000;
        m_Target.Buffs.Remove(this);
        enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActivated) {
            EndBuff();
            return;
        }
        transform.position = m_Target.transform.position;
        transform.rotation = m_Target.transform.rotation;
        transform.localScale = m_Target.transform.localScale;
    }
} 
