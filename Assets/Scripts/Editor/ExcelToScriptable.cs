using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class ExcelToScriptable : Editor
{

    static string s_WorkDir = "Configuration";
    static string s_RelativePath = $"Assets/{s_WorkDir}";
    static string s_Workspace = $"{Application.dataPath}/{s_WorkDir}";
    static string s_Designer = $"{s_Workspace}/Designer.xlsx";

    static List<Type> s_GlobalTypes = new List<Type>(){
        typeof(GlobalScriptableObject),
        typeof(CharacterListScriptableObject),
    };
    static List<Type> s_LocalTypes = new List<Type>()
    {
        typeof(CharacterScriptableObject),
    };

    static void CreateGlobalTable()
    {
        var path = $"{s_Designer}/Global.xlsx";
        var xlsx = new XLSX(path);
        var sheet = xlsx["global"];
        var obj = new GlobalScriptableObject();
        sheet.serializeFormScriptableObject(obj, false);
        xlsx.Save();
    }

    [MenuItem("ECC/Configuration/Init Excels")]
    public static void InitExcels()
    {
        var timeStart = DateTime.Now;
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }


        // GlobalScriptableObject
        var xlsx = new XLSX(s_Designer);

        foreach (var globalType in s_GlobalTypes)
        {
            var name = globalType.Name.Replace("ScriptableObject", "");
            var sheet = xlsx[name];
            sheet.serializeFormScriptableObject(globalType, Activator.CreateInstance(globalType), false);
            sheet.GetCell("A1").setValue("Global");
        }

        foreach (var localType in s_LocalTypes)
        {
            var name = localType.Name.Replace("ScriptableObject", "");
            var sheet = xlsx[name];
            var exampleValue = Activator.CreateInstance(localType);
            sheet["ExampleValue"].serializeFormScriptableObject(localType, exampleValue, false);
            sheet.GetCell("A1").setValue("Local");
        }

        xlsx.Save();
        Debug.Log($"Init Excels Success, time: {(DateTime.Now - timeStart).TotalSeconds}s");
    }


    [MenuItem("ECC/Configuration/Generate Scriptable")]
    public static void GenerateScriptable()
    {
        var timeStart = DateTime.Now;

        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }

        if (!File.Exists(s_Designer))
        {
            Debug.LogWarning("Designer is not exist");
            return;
        }


        var xlsx = new XLSX(s_Designer);

        foreach (var localType in s_LocalTypes)
        {
            var name = localType.Name.Replace("ScriptableObject", "");
            var sheet = xlsx[name];

            foreach (var raw in sheet.raws)
            {
                var objectName = $"{name}_{raw.name}";
                var path = $"{s_RelativePath}/{objectName}.asset";

                var asset = AssetDatabase.LoadAssetAtPath(path, localType);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance(localType);
                    AssetDatabase.CreateAsset(asset, path);
                }
                raw.serializeToScriptableObject(localType, asset);
            }
        }

        foreach (var globalType in s_GlobalTypes)
        {
            var name = globalType.Name.Replace("ScriptableObject", "");
            var sheet = xlsx[name];
            var path = $"{s_RelativePath}/{name}.asset";

            var asset = AssetDatabase.LoadAssetAtPath(path, globalType);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(globalType);
                AssetDatabase.CreateAsset(asset, path);
            }
            sheet.serializeToScriptableObject(globalType, asset);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Generate Scriptable Success, time: " + (DateTime.Now - timeStart).TotalSeconds + "s");
    }


    [MenuItem("ECC/Configuration/Overwrite Excels")]
    public static void OverwriteExcels()
    {
        var timeStart = DateTime.Now;
        if (!EditorUtility.DisplayDialog("Overwrite Excels", "This function will overwrite Designer.xlsx", "Yes", "No"))
        {
            return;
        }
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }

        if (!File.Exists(s_Designer))
        {
            Debug.LogWarning("Designer is not exist");
            return;
        }

        var filesPath = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { s_RelativePath });
        var xlsx = new XLSX(s_Designer);

        foreach (var filePath in filesPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(filePath));
            var names = fileName.Split('_');
            var name = names[0];
            var instanceName = names.Length > 1 ? names[1] : null;
            var typeName = $"{name}ScriptableObject";

            var type = s_GlobalTypes.Find(t => t.Name == typeName) ?? s_LocalTypes.Find(t => t.Name == typeName);
            if (type == null)
            {
                Debug.LogWarning($"ScriptableObject {fileName} is not exist");
                continue;
            }
            var isGlobal = s_GlobalTypes.Contains(type);
            var sheet = xlsx[name];
            var asset = AssetDatabase.LoadAssetAtPath($"{s_RelativePath}/{fileName}.asset", type);

            if (isGlobal)
            {
                sheet.serializeFormScriptableObject(type, asset);
            }
            else
            {
                if (instanceName == null)
                {
                    Debug.LogWarning($"LocalScriptableObject {fileName} is not instance");
                    continue;
                }
                var raw = sheet[instanceName];
                raw.serializeFormScriptableObject(type, asset);
            }
        }


        xlsx.Save();
        Debug.Log("Ovewrite Excels Success, time: " + (DateTime.Now - timeStart).TotalSeconds + "s");
    }

}
