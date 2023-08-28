using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Drawing.Text;

public class PoolAble<T> : NetworkBehaviour
{
    [HideInInspector]
    public PrefabPool Pool = null;

    public PrefabPool pool
    {
        get
        {
            if (Pool == null)
            {
                var poolObj = GameObject.Find($"{name}_pool");
                if (poolObj == null)
                {
                    Debug.LogError($"Pool {name}_pool not found");
                }
                else
                {
                    Pool = poolObj.GetComponent<PrefabPool>();
                }
            }
            return Pool;
        }
    }

    public T GetInstance(Vector3 position, Quaternion rotation)
    {
        return pool.Get(position, rotation).GetComponent<T>();
    }

    public void RemoveInstance()
    {
        if (!isServer) return;
        NetworkServer.UnSpawn(gameObject);
        pool.Return(gameObject);
    }
}
