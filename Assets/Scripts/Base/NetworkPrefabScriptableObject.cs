using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[Serializable]
public struct NetworkPrefabInfo
{
    [HideInInspector]
    public string Name;
    public uint ID;
    public string Path;
}

public class NetworkPrefabScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<NetworkPrefabInfo> prefabInfo = new List<NetworkPrefabInfo>();

    public NetworkPrefabInfo GetInfo(string name)
    {
        foreach (var info in prefabInfo)
        {
            if (info.Name == name)
            {
                return info;
            }
        }
        throw new Exception("Prefab not found");
    }

    public NetworkPrefabInfo GetInfo(uint id)
    {
        foreach (var info in prefabInfo)
        {
            if (info.ID == id)
            {
                return info;
            }
        }
        throw new Exception("Prefab not found");
    }

    public NetworkPrefabInfo GetInfo(GameObject prefab)
    {
        var identity = prefab.GetComponent<NetworkIdentity>();
        if (identity == null)
        {
            throw new Exception("Prefab does not have a NetworkIdentity");
        }
        foreach (var info in prefabInfo)
        {
            if (info.ID == identity.assetId)
            {
                return info;
            }
        }
        throw new Exception("Prefab not found");
    }

}
