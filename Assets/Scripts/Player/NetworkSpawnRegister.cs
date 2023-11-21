using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AddressableAssets;
using System;

public partial class NetworkController
{
    [SerializeField]
    NetworkPrefabScriptableObject m_NetworkPrefabs;
    HashSet<uint> registeredSpawnIds = new HashSet<uint>();

    void InitSpawnRegister()
    {
        if (playerPrefab != null)
        {
            registeredSpawnIds.Add(playerPrefab.GetComponent<NetworkIdentity>().assetId);
        }
        foreach (var spawn in spawnPrefabs)
        {
            if (spawn == null) continue;
            registeredSpawnIds.Add(spawn.GetComponent<NetworkIdentity>().assetId);
        }
    }

    public bool IsRegisteredSpawn(uint assetId)
    {
        return registeredSpawnIds.Contains(assetId);
    }

    [Client]
    [Responsible("RegisterSpawns")]
    IEnumerator RegisterSpawns(List<uint> assetIds, Action<bool> callBack)
    {
        foreach (var assetId in assetIds)
        {
            if (IsRegisteredSpawn(assetId)) continue;
            var handler = Addressables.LoadAsset<GameObject>(m_NetworkPrefabs.GetInfo(assetId).Path);
            yield return handler;
            var obj = handler.Result;
            if (obj == null)
            {
                Debug.LogError($"Can't find Object {m_NetworkPrefabs.GetInfo(assetId).Path}");
                callBack?.Invoke(false);
                yield break;
            }
            NetworkClient.RegisterPrefab(obj);
            registeredSpawnIds.Add(assetId);
        }
        callBack?.Invoke(true);
    }

    [Server]
    public void Spawn(GameObject obj)
    {
        var identity = obj.GetComponent<NetworkIdentity>();
        if (identity == null)
        {
            Debug.LogError("Spawn object must have NetworkIdentity");
            return;
        }
        RequestAllClients<bool>("RegisterSpawns", new uint[] { identity.assetId }).completed += (result) => NetworkServer.Spawn(obj);
    }

    [Server]
    public MultipleRequestAsyncOption<bool> RegisterSpawns(GameObject[] objs)
    {
        var assetIds = new List<uint>();
        foreach (var obj in objs)
        {
            var identity = obj.GetComponent<NetworkIdentity>();
            if (identity == null)
            {
                Debug.LogError("Spawn object must have NetworkIdentity");
                continue;
            }
            assetIds.Add(identity.assetId);
        }
        if (assetIds.Count == 0) return null;
        var option = RequestAllClients<bool>("RegisterSpawns", assetIds.ToArray());
        return option;
    }

    [Server]
    public MultipleRequestAsyncOption<bool> RegisterSpawns(HashSet<uint> assetIds) {
        var assetIdsList = new List<uint>();
        foreach (var assetId in assetIds)
        {
            assetIdsList.Add(assetId);
        }
        var option = RequestAllClients<bool>("RegisterSpawns", assetIdsList);
        return option;
    }
}
