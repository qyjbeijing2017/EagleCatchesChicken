using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Collections.Generic;
#endif

class XLSXBullet : IXLSXFiledAttribute
{
    public XLSXBullet()
    {
#if UNITY_EDITOR
        writer = (object obj) =>
        {
            var bullet = obj as GameObject;
            if (bullet == null) return "";
            return bullet.name;
        };

        reader = (string str) =>
        {
            if (str == "") return null;
            var bulletPath = $"Assets/Prefabs/Bullets/{str}.prefab";
            var configPath = $"Assets/Configurations/Bullet_{str}.asset";
            var bulletObj = AssetDatabase.LoadAssetAtPath<GameObject>(bulletPath);
            var configObj = AssetDatabase.LoadAssetAtPath<BulletScriptableObject>(configPath);

            if (bulletObj == null)
            {
                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                gameObject.AddComponent<Bullet>();
                try
                {
                    bulletObj = PrefabUtility.SaveAsPrefabAsset(gameObject, bulletPath);
                } catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    GameObject.DestroyImmediate(gameObject);
                }
            }
            if (bulletObj.GetComponent<Bullet>() == null)
            {
                bulletObj.AddComponent<Bullet>();
            }

            bulletObj.GetComponent<Bullet>().BulletConfig = configObj;
            EditorUtility.SetDirty(bulletObj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return bulletObj;
        };
#endif
    }
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Bullet_", menuName = "ScriptableObjects/BulletScriptableObject", order = 1)]
#endif
[XLSXLocal]
public class BulletScriptableObject : ScriptableObject, IAttack
{

    public float During = 10f;
    public float Speed = 10f;
    public float Size = 1f;

    public float OffsetAngle = 0f;

    public Vector3 OffsetPosition = new Vector3(0, 1.2f, 2f);

    // [Range((int)PlayerIdentity.Mom, (int)PlayerIdentity.Dummy)]
    public LayerMask TargetLayer = 1 << 8 | 1 << 9;
    public int Damage = 0;
    public Vector3 KnockbackDistance = Vector3.zero;
    public float KnockbackDuration = 0f;

    public Vector3 KnockoffInitialVelocity = Vector3.zero;
    public float KnockoffDuration = 0f;

    [XLSXBuffList]
    public List<GameObject> Buffs = new List<GameObject>();

    int IAttack.Damage => Damage;

    Vector3 IAttack.KnockbackDistance => KnockbackDistance;

    float IAttack.KnockbackDuration => KnockbackDuration;

    Vector3 IAttack.KnockoffInitialVelocity => KnockoffInitialVelocity;

    float IAttack.KnockoffDuration => KnockoffDuration;

    List<Buff> IAttack.Buffs
    {
        get
        {
            List<Buff> buffs = new List<Buff>();
            foreach (var buff in Buffs)
            {
                buffs.Add(buff.GetComponent<Buff>());
            }
            return buffs;

        }
    }
}
