using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Collider))]
public class Bullet : Damage
{
    [Header("Bullet")]
    [SerializeField]
    private float Speed = 0;
    [SerializeField]
    private float LifeTime = 0;
    [SerializeField]
    private LayerMask StoppableLayer = 0;

    float LifeTimeCounter = 0;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if ((StoppableLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    public override void Exec(Skill skill)
    {
        this.CurrentSkill = skill;
        Murderer = skill.murderer;
        if (Murderer)
            MurdererBuffManager = Murderer.GetComponent<BuffManager>();
    }

    protected override void Awake()
    {
    }

    void Start()
    {
        LifeTimeCounter = LifeTime;
    }

    void Update()
    {
        if (isServer)
        {
            transform.position += transform.forward * Speed * Time.deltaTime;
            LifeTimeCounter -= Time.deltaTime;
            if (LifeTimeCounter <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
