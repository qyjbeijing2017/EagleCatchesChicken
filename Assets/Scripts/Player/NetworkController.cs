using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public struct CreateCharacterMessage : NetworkMessage
{
    public CreateCharacterMessage(string name)
    {
        this.name = name;
    }
    public string name;
}

public class NetworkController : NetworkManager
{
    public static NetworkController singleton {
        get
        {
            return NetworkManager.singleton as NetworkController;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCharactorCreate);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    void OnCharactorCreate(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        Addressables.LoadAsset<GameObject>($"Assets/Prefabs/Characters/{message.name}.prefab").Completed += prefab =>
        {
            // NetworkClient.RegisterPrefab(prefab.Result);
            var obj = Instantiate(prefab.Result);
            NetworkServer.ReplacePlayerForConnection(conn, obj);
        };
    }
}
