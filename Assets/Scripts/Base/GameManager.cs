using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using UnityEditor;

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
                var handle = Addressables.LoadAssetAsync<TextAsset>($"Assets/HotFix/{platformName}/{name}.dll.bytes");
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
