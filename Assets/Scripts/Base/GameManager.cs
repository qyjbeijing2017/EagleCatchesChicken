using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using HybridCLR;


#if !DISABLESTEAMWORKS
using Steamworks;
#endif

public class GameManager : MonoSingleton<GameManager>
{
        #region HybridCLR&Addressables
        private Dictionary<string, Assembly> m_HotUpdateAsses = new Dictionary<string, Assembly>();

        static public string platformName
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
                if (m_HotUpdateAsses.ContainsKey(name))
                {
                        return m_HotUpdateAsses[name];
                }
                return null;
        }

        public bool ContainsAssembly(string name)
        {
                return m_HotUpdateAsses.ContainsKey(name);
        }

        public void ReleaseAssembly(string name)
        {
                if (m_HotUpdateAsses.ContainsKey(name))
                {
                        m_HotUpdateAsses.Remove(name);
                }
        }

        public void ClearAssembly()
        {
                m_HotUpdateAsses.Clear();
        }

        public IEnumerator LoadMetadataForAOTAssembly(string name)
        {
                Debug.Log($"Load AOT source Assets/HotFix/{platformName}/{name}.bytes");
                var handle = Addressables.LoadAssetAsync<TextAsset>($"Assets/HotFix/{platformName}/{name}.bytes");
                yield return handle;
                var assbleData = handle.Result.bytes;
                RuntimeApi.LoadMetadataForAOTAssembly(assbleData, HomologousImageMode.SuperSet);
        }

        public IEnumerator LoadScript(string name)
        {
                Debug.Log($"Load source Assets/HotFix/{platformName}/ECC{name}.dll.bytes");
                var handle = Addressables.LoadAssetAsync<TextAsset>($"Assets/HotFix/{platformName}/ECC{name}.dll.bytes");
                yield return handle;
                var assbleData = handle.Result.bytes;
                var ass = Assembly.Load(assbleData);
                m_HotUpdateAsses[name] = ass;

        }
        // Start is called before the first frame update

        IEnumerator LoadSceneHandler(string name)
        {
                yield return StartCoroutine(LoadLoadingScene());
                var loading = FindObjectOfType<LoadingBase>();
                loading.maxValue = 100;
                loading.Tick("Loading scripts...", 0);
                yield return LoadScript(name);
                var loadingAss = GetAssembly(name);
                var loadingType = loadingAss.GetType(name);

                // Loading something from Hotfix
                var extraLoadingMethod = loadingType.GetMethod("ExtraLoading");
                if (extraLoadingMethod != null)
                {
                        // yield return StartCoroutine((IEnumerator)extraLoadingMethod.Invoke(null, new object[] { loading }));
                }

                // Loading scene
                var operation = Addressables.LoadSceneAsync(
                        $"Assets/Scenes/{name}.unity",
                        UnityEngine.SceneManagement.LoadSceneMode.Single,
                        false
                );

                var lastPercent = 0f;
                var sceneProgress = loading.maxValue - loading.progress;
                while (!operation.IsDone)
                {
                        var percent = operation.PercentComplete;
                        var add = percent - lastPercent;
                        lastPercent = percent;
                        loading.Tick($"Loading Scene...", add * sceneProgress);
                        yield return null;
                }

                loading.Tick($"Starting Scene");
                yield return operation.Result.ActivateAsync();
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

        IEnumerator LoadAllMetadataForAOTAssemblyHandler()
        {
                yield return StartCoroutine(LoadLoadingScene());
                var loading = FindObjectOfType<LoadingBase>();
                loading.maxValue = AOTGenericReferences.PatchedAOTAssemblyList.Count;
                loading.Tick("Loading AOT metadata...", 0);

                foreach (var name in AOTGenericReferences.PatchedAOTAssemblyList)
                {
                        loading.Tick($"Loading AOT metadata for {name}...");
                        yield return StartCoroutine(LoadMetadataForAOTAssembly(name));
                        loading.Tick($"Loading AOT metadata for {name} finished!", 1);
                }
        }

        public Coroutine LoadAllMetadataForAOTAssembly()
        {
                return StartCoroutine(LoadAllMetadataForAOTAssemblyHandler());
        }
        #endregion

        #region Steamworks
#if !DISABLESTEAMWORKS

        CSteamID m_CSteamID;
        Callback<GameOverlayActivated_t> m_GameOverlayActivated;

        void StartSteamNetwork()
        {
                if (SteamManager.Initialized)
                {
                        m_CSteamID = SteamUser.GetSteamID();
                        string name = SteamFriends.GetPlayerNickname(m_CSteamID) ?? SteamFriends.GetPersonaName();
                        Debug.Log($"Steam Manager Initialized, name: {name}");
                }
                else
                {
                        Debug.LogWarning("Steam Manager Failed");
                }
        }

        void OnEnable()
        {
                if (SteamManager.Initialized)
                {
                        if (SteamManager.Initialized)
                        {
                                m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
                        }
                }
        }


        void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
        {
                if (pCallback.m_bActive != 0)
                {
                        Time.timeScale = 0;
                        Debug.Log("Steam Overlay has been activated");
                }
                else
                {
                        Time.timeScale = 1;
                        Debug.Log("Steam Overlay has been closed");
                }
        }

#endif
        #endregion

        void Start()
        {
#if !DISABLESTEAMWORKS
                StartSteamNetwork();
#endif
        }

}
