using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using UnityEngine;

public class AutoNetworkPrefabIDMap : Editor
{
    [MenuItem("ECC/Network/CreateIDMap")]
    static void CreateIDMap()
    {
        var networkPrefabScriptableObject = AssetDatabase.LoadAssetAtPath<NetworkPrefabScriptableObject>("Assets/Configurations/NetworkPrefab.asset");
        if(networkPrefabScriptableObject == null)
        {
            networkPrefabScriptableObject = ScriptableObject.CreateInstance<NetworkPrefabScriptableObject>();
            AssetDatabase.CreateAsset(networkPrefabScriptableObject, "Assets/Configurations/NetworkPrefab.asset");
        }
        networkPrefabScriptableObject.prefabIDMap.Clear();
        var prefabGUIDList = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/Prefabs"});
        foreach (var prefabGUID in prefabGUIDList)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<NetworkIdentity>(AssetDatabase.GUIDToAssetPath(prefabGUID));
            if(prefab == null) continue;
            networkPrefabScriptableObject.prefabIDMap.Add(new NetworkPrefabIDMap()
            {
                Name = prefab.name,
                ID = prefab.assetId,
                Path = AssetDatabase.GetAssetPath(prefab)
            });

        }
        EditorUtility.SetDirty(networkPrefabScriptableObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
