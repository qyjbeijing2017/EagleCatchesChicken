using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ECCNetworkManager : NetworkManager
{

    public List<Player> RoleList;

    [HideInInspector]
    public List<Player> PlayerList = new List<Player>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var id = PlayerList.Count;
        if(id >= RoleList.Count)
        {
            Debug.LogWarning("No more roles");
            GameObject player = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
            return;
        }
        GameObject charactor = Instantiate(RoleList[id].gameObject);
        NetworkServer.AddPlayerForConnection(conn, charactor);
        PlayerList.Add(charactor.GetComponent<Player>());
    }
}
