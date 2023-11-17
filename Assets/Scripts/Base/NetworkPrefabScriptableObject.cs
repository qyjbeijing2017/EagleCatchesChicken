using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NetworkPrefabIDMap
{
    [HideInInspector]
    public string Name;
    public uint ID;
    public string Path;
}

public class NetworkPrefabScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<NetworkPrefabIDMap> prefabIDMap = new List<NetworkPrefabIDMap>();
}
