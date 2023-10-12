using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEditor;

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

#if UNITY_EDITOR
    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
    {
        if (_color != default(Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (BulletPrefab)
        {
            var length = BulletPrefab.speed * BulletPrefab.lifeTime;
            var center = transform.position + transform.forward * length / 2;
            var scale = BulletPrefab.transform.lossyScale;
            Shoot.DrawWireCapsule(
                center, 
                this.transform.rotation * Quaternion.Euler(90, 0, 0),
                BulletPrefab.transform.lossyScale.x, 
                length
            );
        }
    }
#endif
}