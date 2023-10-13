using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using UnityEditor;
using UnityEngine.ResourceManagement.ResourceProviders;
using Unity.Loading;
using System;

public class GameManager : MonoSingleton<GameManager>
{
        private Dictionary<string, Assembly> hotUpdateAsses = new Dictionary<string, Assembly>();

        public string platformName
        {
                get
                {
#if UNITY_STANDALONE_WIN
                        return "StandaloneWindows64";
#elif UNITY_STANDALONE_OSX
                        return "StandaloneOSXUniversal";
#elif UNITY_STANDALONE_LINUX
                        return "StandaloneLinuxUniversal";
#elif UNITY_ANDROID
                        return "Android";
#elif UNITY_IOS
                        return "iOS";
#elif UNITY_WSA
                        return "WSA";
#elif UNITY_WEBGL
                        return "WebGL";
#endif
                }
        }

        public Assembly GetAssembly(string name)
        {
                if (hotUpdateAsses.ContainsKey(name))
                {
                        return hotUpdateAsses[name];
                }
                return null;
        }

        public bool ContainsAssembly(string name)
        {
                return hotUpdateAsses.ContainsKey(name);
        }

        public void ReleaseAssembly(string name)
        {
                if (hotUpdateAsses.ContainsKey(name))
                {
                        hotUpdateAsses.Remove(name);
                }
        }

        public void ClearAssembly()
        {
                hotUpdateAsses.Clear();
        }

        public IEnumerator LoadScript(string name)
        {
                var handle = Addressables.LoadAssetAsync<TextAsset>($"Assets/HotFix/{platformName}/ECC{name}.dll.bytes");
                yield return handle;
                var assbleData = handle.Result.bytes;
                var ass = Assembly.Load(assbleData);
                hotUpdateAsses[name] = ass;
        }
        // Start is called before the first frame update

        IEnumerator LoadSceneHandler(string name)
        {
                yield return StartCoroutine(LoadLoadingScene());
                var loading = FindObjectOfType<LoadingBase>();
                loading.maxValue = 100;
                // TODO: Loading Scene
        }

        IEnumerator LoadLoadingScene()
        {
                if (!ContainsAssembly("Loading"))
                {
                        yield return StartCoroutine(LoadScript("Loading"));
                }
                yield return Addressables.LoadSceneAsync("Assets/Scenes/Loading.unity");
        }

        public Coroutine LoadScene(string name)
        {
                if (name == "Loading")
                {
                        return StartCoroutine(LoadLoadingScene());
                }
                return StartCoroutine(LoadSceneHandler(name));
        }

        bool _isReady = false;

        IEnumerator InitManager()
        {
                yield return StartCoroutine(LoadScript("Loading"));
                yield return Addressables.LoadSceneAsync("Assets/Scenes/Loading.unity");
        }


        void Start()
        {
                StartCoroutine(InitManager());
        }

        // Update is called once per frame
        void Update()
        {

        }
}
