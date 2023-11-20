using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AddressableAssets;


public partial class NetworkController
{
    void SpawnRegisterClientInit()
    {
        foreach (var prefab in spawnPrefabs)
        {
            var identity = prefab.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                m_RegisteredPrefabs.Add(identity.assetId);
            }
        }
        if (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly)
        {
            NetworkServer.RegisterHandler<SpawnReadyMessage>(OnSpawnReady);
        }
        if (mode == NetworkManagerMode.ClientOnly)
        {
            NetworkClient.RegisterHandler<SpawnCreateMessage>(OnSpawnRegister);
        }
    }


    #region Dynamic Register
    public struct SpawnCreateMessage : NetworkMessage
    {
        public int id;
        public uint spawnId;
    }

    public struct SpawnReadyMessage : NetworkMessage
    {
        public int id;
    }

    public class NetworkRegisterAsyncOperation : CustomYieldInstruction
    {
        NetworkPrefabInfo m_Info;
        public NetworkPrefabInfo info
        {
            get
            {
                return m_Info;
            }
        }


        GameObject m_Result;
        public GameObject result
        {
            get
            {
                if (!m_IsDone) return null;
                return m_Result;
            }
        }


        HashSet<int> m_ReadyClients = new HashSet<int>();
        public HashSet<int> readyClients
        {
            get
            {
                return m_ReadyClients;
            }
        }


        bool m_IsDone = false;
        public bool isDone
        {
            get
            {
                return m_IsDone;
            }
        }


        public override bool keepWaiting => !m_IsDone;

        event System.Action<GameObject> complete;




        public NetworkRegisterAsyncOperation(NetworkPrefabInfo info, GameObject gameObject)
        {
            this.m_Info = info;
            this.m_Result = gameObject;
        }

        public void Finished()
        {
            m_IsDone = true;
            complete?.Invoke(null);
        }
    }


    [SerializeField]
    NetworkPrefabScriptableObject PrefabScriptableObject;
    HashSet<uint> m_RegisteredPrefabs = new HashSet<uint>();
    Dictionary<int, NetworkRegisterAsyncOperation> m_RegisterOperations = new Dictionary<int, NetworkRegisterAsyncOperation>();


    [Client]
    void OnSpawnRegister(SpawnCreateMessage msg)
    {
        var info = PrefabScriptableObject.GetInfo(msg.spawnId);
        if (m_RegisteredPrefabs.Contains(info.ID))
        {
            var message = new SpawnReadyMessage
            {
                id = msg.id
            };
            NetworkClient.Send(message);
            return;
        }

        Addressables.LoadAssetAsync<GameObject>(info.Path).Completed += (op) =>
        {
            var prefab = op.Result;
            var identity = prefab.GetComponent<NetworkIdentity>();
            if (identity == null)
            {
                throw new System.Exception("Prefab does not have a NetworkIdentity");
            }
            if (m_RegisteredPrefabs.Contains(identity.assetId))
            {
                return;
            }
            NetworkClient.RegisterPrefab(prefab);
            m_RegisteredPrefabs.Add(identity.assetId);
            var message = new SpawnReadyMessage
            {
                id = msg.id
            };
            NetworkClient.Send(message);
        };
    }

    [Server]
    void OnSpawnReady(NetworkConnectionToClient conn, SpawnReadyMessage msg)
    {
        var option = m_RegisterOperations[msg.id];
        if (option == null)
        {
            return;
        }

        option.readyClients.Add(conn.connectionId);

        // Check if all clients are ready
        foreach (var connection in NetworkServer.connections)
        {
            if (!option.readyClients.Contains(connection.Value.connectionId))
            {
                return;
            }
        }

        // All clients are ready
        option.Finished();
        m_RegisteredPrefabs.Add(option.info.ID);
        m_RegisterOperations.Remove(msg.id);
    }


    [Server]
    public NetworkRegisterAsyncOperation Register(GameObject prefab)
    {
        var info = PrefabScriptableObject.GetInfo(prefab);
        var option = new NetworkRegisterAsyncOperation(info, prefab);
        if (m_RegisteredPrefabs.Contains(info.ID))
        {
            option.Finished();
            return option;
        }

        var message = new SpawnCreateMessage
        {
            id = option.GetHashCode(),
            spawnId = info.ID
        };
        NetworkServer.SendToAll(message);
        m_RegisterOperations.Add(message.id, option);
        return option;
    }

    #endregion

    #region Sync Register

    public struct SyncRegisterMessage : NetworkMessage
    {
        public List<uint> ids;
    }

    [Client]
    void OnSyncRegister(SyncRegisterMessage message)
    {
        foreach (var id in message.ids)
        {
            var info = PrefabScriptableObject.GetInfo(id);
            if (m_RegisteredPrefabs.Contains(info.ID))
            {
                continue;
            }
            Addressables.LoadAssetAsync<GameObject>(info.Path).Completed += (op) =>
            {
                var prefab = op.Result;
                var identity = prefab.GetComponent<NetworkIdentity>();
                if (identity == null)
                {
                    throw new System.Exception("Prefab does not have a NetworkIdentity");
                }
                if (m_RegisteredPrefabs.Contains(identity.assetId))
                {
                    return;
                }
                NetworkClient.RegisterPrefab(prefab);
                m_RegisteredPrefabs.Add(identity.assetId);
            };
        }

    }
    #endregion
}
