using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Practice : MonoBehaviour
{
    public static IEnumerator ExtraLoading(LoadingBase loading)
    {
        loading.maxValue += 1;
        loading.Tick("Loading Player...");
        var handler = GameManager.instance.LoadScript("Player");
        yield return handler;
        loading.Tick("Loading Player Finished");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.singleton.StartHost();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        NetworkManager.singleton.StopHost();
        Destroy(PlayerController.instance);
    }
}
