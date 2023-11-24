using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class ExcelToScriptable : Editor
{
    static string s_Workspace = $"{Application.dataPath}/Configurations";
    static string s_RelativeWorkSpace = $"Assets/Configurations";
    static string s_FilePath = $"{s_Workspace}/Designer.xlsx";

    static List<Type> s_ScriptableTypes = new List<Type>(){
        typeof(AttackRangeScriptableObject),
        typeof(BuffScriptableObject),
        typeof(AttackScriptableObject),
        typeof(SkillScriptableObject),
        typeof(CharacterScriptableObject),
        typeof(CharacterListScriptableObject),
        typeof(NetworkPrefabScriptableObject),
        typeof(GlobalScriptableObject),
    };

    [MenuItem("ECC/Configuration/CreateExcel")]
    public static void CreateExcel()
    {
        if (!Directory.Exists(s_Workspace)) Directory.CreateDirectory(s_Workspace);
        var xlsx = new XLSX(s_FilePath);
        xlsx.CreateSheets(s_ScriptableTypes);
        xlsx.Save();
        Debug.Log("Create Excel Done");
    }

    [MenuItem("ECC/Configuration/Serialize")]
    public static void Serialize()
    {
        if (EditorUtility.DisplayDialog("Overwrite Excels", "This action will overwrite all excels, are you sure?", "Yes", "No") == false) return;
        if (!Directory.Exists(s_Workspace)) Directory.CreateDirectory(s_Workspace);
        var xlsx = new XLSX(s_FilePath);
        xlsx.SerializeAll(s_ScriptableTypes, s_RelativeWorkSpace);
        xlsx.Save();
        Debug.Log("Serialize Done");
    }

    [MenuItem("ECC/Configuration/Deserialize")]
    public static void Deserialize()
    {
        var xlsx = new XLSX(s_FilePath);
        xlsx.DeserializeAll(s_ScriptableTypes, s_RelativeWorkSpace);
        Debug.Log("Deserialize Done");
    }

}
