using System.Collections;
using Mirror;
using UnityEngine;

public partial class NetworkController : NetworkManager
{
    [SerializeField]
    private GlobalScriptableObject m_Global;
    public GlobalScriptableObject global
    {
        get
        {
            return m_Global;
        }
    }

    public static new NetworkController singleton
    {
        get
        {
            return NetworkManager.singleton as NetworkController;
        }
    }

    public override void OnStartServer()
    {
        ScanResponsibleMethod();
        InitServerResponsible();
        StartCoroutine(ResponsibleTimeoutMonitor());
        InitSpawnRegister();
        base.OnStartServer();
    }

    public override void OnClientConnect()
    {
        StartCoroutine(HandleClientConnect());
    }

    IEnumerator HandleClientConnect()
    {
        if (mode == NetworkManagerMode.ClientOnly)
        {
            ScanResponsibleMethod();
            StartCoroutine(ResponsibleTimeoutMonitor());
        }
        InitClientResponsible();
        InitSpawnRegister();

        yield return RegisterSpawns(registeredSpawnIds);
        yield return null;
        base.OnClientConnect();
    }
}
