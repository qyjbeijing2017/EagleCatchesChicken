using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerController : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // public void CreateCharactor(string name)
    // {
    //     Addressables.LoadAsset<GameObject>($"Assets/Prefabs/Characters/{name}.prefab").Completed += prefab =>
    //     {
    //         NetworkClient.RegisterPrefab(prefab.Result);
    //         NetworkClient.Send(new CreateCharacterMessage(name));
    //     };
    // }


    // Update is called once per frame
    void Update()
    {

    }
}
