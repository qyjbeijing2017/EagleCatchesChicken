using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Shoot : Damage
{
    [Header("Shoot")]
    [SerializeField]
    Bullet BulletPrefab;
    protected override void OnTriggerEnter(Collider other)
    {
    }
    protected override void OnTriggerExit(Collider other)
    {
    }

    [Server]
    public override void Exec(Skill skill)
    {
        var bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(bullet.gameObject);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.Exec(skill);
    }

    protected override void Awake()
    {
    }
}
