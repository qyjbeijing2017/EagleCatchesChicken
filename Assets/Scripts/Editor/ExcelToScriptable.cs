using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExcelToScriptable : Editor
{

    [MenuItem("ECC/Configuration/Create Excel")]
    public static void CreateScriptableObject()
    {
        string workSpace = $"{Application.dataPath}/Configuration";
        string runtimePath = $"{workSpace}/Runtime";
        string designerPath = $"{workSpace}/Designer";

        var excelApp = new Microsoft.Office.Interop.Excel.Application();
        var workbook = excelApp.Workbooks.Open($"{designerPath}/global.xlsx");
        var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];


    }

}
