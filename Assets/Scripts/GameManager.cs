using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using HybridCLR;
using System.Reflection;

public class GameManager : MonoSingleton<GameManager>
{
    private Dictionary<string, Assembly> hotUpdateAsses = new Dictionary<string, Assembly>();
    public IEnumerator LoadScript(string name)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>($"Assets/HotFix/{name}.dll.bytes");
        yield return handle;
        var assbleData = handle.Result.bytes;
        var ass = Assembly.Load(assbleData);
        hotUpdateAsses[name] = ass;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
