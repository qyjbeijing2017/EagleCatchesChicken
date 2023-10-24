using UnityEngine;
using UnityEditor;
using System.IO;

public class ExcelToScriptable : Editor
{
    static string s_Workspace = $"{Application.dataPath}/Configuration";
    static string s_Designer = $"{s_Workspace}/Designer";
    static string s_Runtime = $"{s_Workspace}/Runtime";

    static void CreateGlobalTable()
    {
        var path = $"{s_Designer}/Global.xlsx";
        var xlsx = new XLSX(path);
        var sheet = xlsx["global"];
        var obj = new GlobalScriptableObject();
        sheet.serializeFormScriptableObject(obj, false);
        xlsx.Save();
    }

    [MenuItem("ECC/Configuration/Create Excels")]
    public static void CreateExcels()
    {
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }
        CreateGlobalTable();


        Debug.Log("Create Excels Done!");

    }

}
