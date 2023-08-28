using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PoolAble<T> : NetworkBehaviour
{
    PrefabPool Pool = null;

    public PrefabPool pool {
        get {
            if(Pool == null) {
                var poolObj = GameObject.Find($"{name}_pool");
                if(poolObj == null) {
                    poolObj = new GameObject($"{name}_pool");
                    Pool = poolObj.AddComponent<PrefabPool>();
                    Pool.prefab = gameObject;
                    Pool.Init();
                } else {
                    Pool = poolObj.GetComponent<PrefabPool>();
                }
            }
            return Pool;
        }
    }

    public T GetInstance(Vector3 position, Quaternion rotation){
       return pool.Get(position, rotation).GetComponent<T>();
    }

    public void RemoveInstance(){
        if(!isServer) return;
        NetworkServer.UnSpawn(gameObject);
        pool.Return(gameObject);
    }
}
