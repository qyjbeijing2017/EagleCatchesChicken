using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public struct HotFixAssembly
{
    public MemoryStream dll;
    public MemoryStream pdb;
}

public class GameManager : MonoSingleton<GameManager>
{
    ILRuntime.Runtime.Enviorment.AppDomain EccAppDomain;
    public ILRuntime.Runtime.Enviorment.AppDomain appDomain
    {
        get
        {
            return EccAppDomain;
        }
    }

    public Dictionary<string, HotFixAssembly> HotFixAssemblys = new Dictionary<string, HotFixAssembly>();

    public IEnumerator LoadHotFix(string key)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(key + ".dll.bytes");
        yield return handle;
        if (handle.OperationException != null)
        {
            throw handle.OperationException;
        }
        var dll = handle.Result.bytes;

        var hotFixAssembly = new HotFixAssembly
        {
            dll = new MemoryStream(dll),
            pdb = null
        };
        HotFixAssemblys[key] = hotFixAssembly;
        

#if DEBUG
        handle = Addressables.LoadAssetAsync<TextAsset>(key + ".pdb.bytes");
        yield return handle;
        if (handle.OperationException != null)
        {
            throw handle.OperationException;
        }
        var pdb = handle.Result.bytes;
        hotFixAssembly.pdb = new MemoryStream(pdb);
        EccAppDomain.LoadAssembly(hotFixAssembly.dll, hotFixAssembly.pdb, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#else
        appDomain.LoadAssembly(hotFixAssembly.dll);
#endif
    }

    public void DestroyHotFix(string key)
    {
        if (HotFixAssemblys.ContainsKey(key))
        {
            var hotFixAssembly = HotFixAssemblys[key];
            Debug.Log("DestroyHotFix:" + key);
            if(hotFixAssembly.dll != null)
                hotFixAssembly.dll.Dispose();
            if(hotFixAssembly.pdb != null)
                hotFixAssembly.pdb.Dispose();
            HotFixAssemblys.Remove(key);
        }
    }

    IEnumerator Test()
    {
        yield return StartCoroutine(LoadHotFix("Assets/HotFix/MainMenu"));
        EccAppDomain.Invoke("MainMenu.MainMenu", "Main", null, null);
        yield return StartCoroutine(LoadHotFix("Assets/HotFix/Test"));
        EccAppDomain.Invoke("Test.Test", "Main", null, null);
    }

    // Start is called before the first frame update
    void Start()
    {
        EccAppDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
#if DEBUG
        EccAppDomain.DebugService.StartDebugService();
        EccAppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (EccAppDomain != null)
        {
            EccAppDomain.Dispose();
            EccAppDomain = null;
        }

        foreach (var hotFixAssembly in HotFixAssemblys)
        {
            if (hotFixAssembly.Value.dll != null)
                hotFixAssembly.Value.dll.Dispose();
            if (hotFixAssembly.Value.pdb != null)
                hotFixAssembly.Value.pdb.Dispose();
        }
    }
}
