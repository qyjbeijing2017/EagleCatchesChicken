using Mirror;

public partial class NetworkController : NetworkManager
{
    public static new NetworkController singleton {
        get
        {
            return NetworkManager.singleton as NetworkController;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        SpawnRegisterClientInit();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        SpawnRegisterClientInit();
    }
}
