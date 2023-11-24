using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(SphereCollider))]
public class Bullet : MonoBehaviour
{
    public BulletScriptableObject BulletConfig;

    public float StartTime;

    public PlayerController Owner;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject == Owner.gameObject) return;
        if((BulletConfig.TargetLayer.value & (1 << collider.gameObject.layer)) == 0) Destroy(gameObject);
        var playerHealth = collider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.BeAttacked(BulletConfig, Owner);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = BulletConfig.Size;
        StartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - StartTime > BulletConfig.During)
        {
            Destroy(gameObject);
        }
    }

    List<PlayerHealth> SomeOneInRange(AttackRangeScriptableObject range, LayerMask mask)
    {
        if(range == null) return new List<PlayerHealth>();
        var result = new List<PlayerHealth>();
        var dir = Quaternion.AngleAxis(range.OffsetAngle, Vector3.up) * transform.forward;
        switch (range.AttackRangeType)
        {
            case AttackRangeType.Circle:
                var circleColliders = Physics.OverlapSphere(transform.position, range.FarDistance, mask);
                foreach (var collider in circleColliders)
                {
                    var playerHealth = collider.GetComponent<PlayerHealth>();
                    if ((collider.transform.position - transform.position).magnitude < range.NearDistance) continue;
                    if (playerHealth != null)
                    {
                        result.Add(playerHealth);
                    }
                }
                break;
            case AttackRangeType.Sector:
                var sectorColliders = Physics.OverlapSphere(transform.position, range.FarDistance, mask);
                foreach (var collider in sectorColliders)
                {
                    var playerHealth = collider.GetComponent<PlayerHealth>();
                    var vectorToTarget = collider.transform.position - transform.position;
                    var angle = Vector3.Angle(dir, vectorToTarget);
                    if (angle > range.SectorAngle / 2) continue;
                    if (playerHealth != null)
                    {
                        result.Add(playerHealth);
                    }
                }
                break;
            case AttackRangeType.Rectangle:
                var RectangleColliders = Physics.OverlapBox(
                    transform.position + dir * (range.NearDistance + range.FarDistance) / 2,
                    new Vector3(range.Width, 1, range.FarDistance - range.NearDistance),
                    Quaternion.LookRotation(dir),
                    mask
                );
                foreach (var collider in RectangleColliders)
                {
                    var playerHealth = collider.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        result.Add(playerHealth);
                    }
                }
                break;
        }
        return result;
    }

}
