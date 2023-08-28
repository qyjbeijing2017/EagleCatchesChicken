using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ECCNetworkManager : NetworkManager
{

    public static ECCNetworkManager instance {
        get{
            return (ECCNetworkManager)singleton;
        }
    }

    public List<Player> RoleList;

    [Header("Debug")]
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
        GameObject character = Instantiate(RoleList[id].gameObject);
        NetworkServer.AddPlayerForConnection(conn, character);
        var Player = character.GetComponent<Player>();
        PlayerList.Add(Player);
        Player.PlayerID = id;
    }
}
