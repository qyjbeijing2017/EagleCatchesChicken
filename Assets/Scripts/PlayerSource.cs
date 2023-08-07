using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSource : NetworkBehaviour
{
    public NetworkVariable<float> Health = new NetworkVariable<float>();

    [SerializeField]
    private float MaxHealth = 100f;

    public override void OnNetworkSpawn()
    {
        Health.Value = MaxHealth;
    }
}
