using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AddressableAssets;


public class Practice : MonoBehaviour
{
    public static IEnumerator ExtraLoading(LoadingBase loading)
    {
        loading.maxValue += 1;
        loading.Tick("Loading Player...");
        var handler = GameManager.instance.LoadScript("Player");
        yield return GameManager.instance.StartCoroutine(handler);
        loading.Tick("Loading Player Finished", 1);

    }
    
    // Start is called before the first frame update
    void Start()
    {
        // NetworkController.singleton.StartHost();
        Addressables.LoadAsset<GameObject>($"Assets/Prefabs/Characters/BlackBoss.prefab").Completed += prefab =>
        {
            NetworkClient.RegisterPrefab(prefab.Result);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if(NetworkManager.singleton)
            NetworkManager.singleton.StopHost();
    }
}
