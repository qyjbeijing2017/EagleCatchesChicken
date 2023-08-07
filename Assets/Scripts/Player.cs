using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
        }
    }
}
