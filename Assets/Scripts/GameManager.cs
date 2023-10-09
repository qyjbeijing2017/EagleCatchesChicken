using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoSingleton<GameManager>
{
    ILRuntime.Runtime.Enviorment.AppDomain appDomain;

    public IEnumerator LoadHotFix(string key)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(key + ".dll.bytes");
        yield return handle;
        if (handle.OperationException != null)
        {
            Debug.LogError(handle.OperationException);
            yield break;
        }
        var dll = handle.Result.bytes;
        Debug.Log(dll.Length);

#if DEBUG
        handle = Addressables.LoadAssetAsync<TextAsset>(key + ".pdb.bytes");
        yield return handle;
        if (handle.OperationException != null)
        {
            Debug.LogError(handle.OperationException);
            yield break;
        }
        var pdb = handle.Result.bytes;
        Debug.Log(pdb.Length);
        appDomain.LoadAssembly(new MemoryStream(dll), new MemoryStream(pdb), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#else
        appDomain.LoadAssembly(new MemoryStream(dll));
#endif
    }

    IEnumerator Test() {
        yield return StartCoroutine(LoadHotFix("Assets/HotFix/MainMenu"));
        appDomain.Invoke("MainMenu.MainMenu", "Main", null, null);
    }

    // Start is called before the first frame update
    void Start()
    {
        appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
#if DEBUG
        appDomain.DebugService.StartDebugService();
        appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        Debug.Log("Start");
#endif
        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
