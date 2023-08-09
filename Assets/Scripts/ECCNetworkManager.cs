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
            Debug.LogError("No more roles");
            NetworkServer.AddPlayerForConnection(conn, null);
            return;
        }
        GameObject player = Instantiate(RoleList[id].gameObject);
        NetworkServer.AddPlayerForConnection(conn, player);
        PlayerList.Add(player.GetComponent<Player>());
    }
}
