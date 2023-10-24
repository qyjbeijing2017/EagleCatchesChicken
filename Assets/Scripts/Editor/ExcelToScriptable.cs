using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class ExcelToScriptable : Editor
{
    static string s_Workspace = $"{Application.dataPath}/Configuration";
    static string s_Designer = $"{s_Workspace}/Designer";
    static string s_Runtime = $"{s_Workspace}/Runtime";

    static List<Type> s_SheetTypes = new List<Type>(){
        typeof(GlobalScriptableObject),
    };
    static List<Type> s_RawTypes = new List<Type>()
    {
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
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }

        if (!Directory.Exists(s_Designer))
        {
            Directory.CreateDirectory(s_Designer);
        }


        // GlobalScriptableObject
        var xlsx = new XLSX($"{s_Designer}/Global.xlsx");

        foreach (var sheetType in s_SheetTypes)
        {
            var name = sheetType.Name.Replace("ScriptableObject", "");
            xlsx[name].serializeFormScriptableObject(sheetType, Activator.CreateInstance(sheetType), false);
        }

        xlsx.Save();

        // RawObject

        foreach (var rawType in s_RawTypes)
        {
            var name = rawType.Name.Replace("ScriptableObject", "");
            var xlsxRaw = new XLSX($"{s_Designer}/{name}.xlsx");
            xlsxRaw["default"].serializeFormScriptableObject(rawType, Activator.CreateInstance(rawType), false);
        }
    }


    [MenuItem("ECC/Configuration/Generate Scriptable")]
    public static void GenerateScriptable()
    {
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }

        if (!Directory.Exists(s_Designer))
        {
            Debug.LogWarning("Designer is not exist");
            return;
        }

        if (!Directory.Exists(s_Runtime))
        {
            Directory.CreateDirectory(s_Runtime);
        }

        var files = Directory.GetFiles(s_Designer, "*.xlsx", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var info = new FileInfo(file);
            var name = info.Name.Replace(".xlsx", "");
            if (name == "Global")
            {
                var xlsx = new XLSX(file);
                var sheets = xlsx.sheets;
                foreach (var sheet in sheets)
                {
                    var path = $"{s_Runtime}/{sheet.name}.asset";
                    var type = Type.GetType($"{sheet.name}ScriptableObject");
                    var asset = AssetDatabase.LoadAssetAtPath(path, type);
                    if (asset == null)
                    {
                        asset = (ScriptableObject)type.Assembly.CreateInstance(type.FullName);
                        sheet.serializeFormScriptableObject(type, asset, true);
                        AssetDatabase.CreateAsset(asset, path);
                    }
                    else
                    {
                        sheet.serializeFormScriptableObject(type, asset, true);
                    }
                    AssetDatabase.SaveAssets();
                }

            }
            else
            {
                var xlsx = new XLSX(file);
                var type = Type.GetType($"{name}ScriptableObject");
                var sheet = xlsx["default"];
                var raws = sheet.raws;

                foreach(var raw in raws)
                {
                    if(String.IsNullOrEmpty(raw.name))
                    {
                        continue;
                    }
                    var path = $"{s_Runtime}/{name}_{raw.name}.asset";
                    var asset = AssetDatabase.LoadAssetAtPath(path, type);
                    if (asset == null)
                    {
                        asset = ScriptableObject.CreateInstance(type);
                        AssetDatabase.CreateAsset(asset, path);
                    }
                    else
                    {
                        sheet.serializeFormScriptableObject(type, asset, true);
                    }
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }

}
