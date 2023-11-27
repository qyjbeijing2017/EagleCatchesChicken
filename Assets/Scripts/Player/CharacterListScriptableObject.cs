using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif


class XLSXCharacterList : IXLSXFiledAttribute
{
    public XLSXCharacterList()
    {
#if UNITY_EDITOR
        writer = (object obj) =>
        {
            var buffs = obj as List<GameObject>;
            var str = "";
            foreach (var buff in buffs)
            {
                if (str != "")
                {
                    str += "\n";
                }
                str += buff.name;
            }
            return str;
        };

        reader = (string str) =>
        {
            List<GameObject> buffs = new List<GameObject>();
            var buffNames = str.Split('\n');
            if (buffNames.Length == 1 && buffNames[0] == "")
            {
                return buffs;
            }
            foreach (var buffName in buffNames)
            {
                var buffPath = $"Assets/Prefabs/Characters/{buffName}.prefab";
                var configPath = $"Assets/Configurations/Character_{buffName}.asset";
                var buffObj = AssetDatabase.LoadAssetAtPath<GameObject>(buffPath);
                var configObj = AssetDatabase.LoadAssetAtPath<CharacterScriptableObject>(configPath);

                if (buffObj == null)
                {
                    var gameObject = new GameObject(buffName);
                    gameObject.AddComponent<PlayerController>();
                    buffObj = PrefabUtility.SaveAsPrefabAsset(gameObject, buffPath);
                    GameObject.DestroyImmediate(gameObject);
                }
                if (buffObj.GetComponent<PlayerController>() == null)
                {
                    buffObj.AddComponent<PlayerController>();
                }
                
                buffObj.GetComponent<PlayerController>().PlayerConfig = configObj;
                EditorUtility.SetDirty(buffObj);
                buffs.Add(buffObj);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return buffs;
        };
#endif
    }
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "CharacterList", menuName = "ScriptableObjects/CharacterListScriptableObject", order = 1)]
#endif
public class CharacterListScriptableObject : ScriptableObject
{
    [XLSXCharacterList]
    public List<GameObject> Characters = new List<GameObject>();
}