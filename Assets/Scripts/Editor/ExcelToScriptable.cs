using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System;
using NUnit.Framework;

public class ExcelToScriptable : Editor
{
    static string s_Workspace = $"{Application.dataPath}/Configuration";
    static string s_Designer = $"{s_Workspace}/Designer";
    static string s_Runtime = $"{s_Workspace}/Runtime";

    static ISheet EnsureSheet(IWorkbook workbook, string name)
    {
        var sheet = workbook.GetSheet(name);
        if (sheet == null)
        {
            sheet = workbook.CreateSheet(name);
        }
        return sheet;
    }

    static IRow EnsureRow(ISheet sheet, int index)
    {
        var row = sheet.GetRow(index);
        if (row == null)
        {
            row = sheet.CreateRow(index);
        }
        return row;
    }

    static ICell EnsureCell(IRow row, int index)
    {
        var cell = row.GetCell(index);
        if (cell == null)
        {
            cell = row.CreateCell(index);
        }
        return cell;
    }

    static ICell HasCell(IRow row, string value)
    {
        for (int i = 0; i < row.LastCellNum; i++)
        {
            var cell = row.GetCell(i);
            if (cell.StringCellValue == value)
            {
                return cell;
            }
        }
        return null;
    }

    static ICell FindRowFromFirstCell(ISheet sheet, string value)
    {
        for (int i = 0; i < sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row.GetCell(0) != null && row.GetCell(0).StringCellValue == value)
            {
                return row.GetCell(0);
            }
        }
        return null;
    }

    static ICell AddRowWithFirstCell(ISheet sheet, string value)
    {
        var row = sheet.CreateRow(sheet.LastRowNum + 1);
        var cell = row.CreateCell(0);
        cell.SetCellValue(value);
        return cell;
    }

    static void CreateGlobalTable()
    {
        using (var fs = new FileStream($"{s_Designer}/global.xlsx", FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {

            var workbook = fs.Length == 0 ? new XSSFWorkbook() : new XSSFWorkbook(fs);
            var sheet = EnsureSheet(workbook, "global");
            var fields = typeof(GlobalScriptableObject).GetFields();
            var defaultObj = new GlobalScriptableObject();

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var raw = FindRowFromFirstCell(sheet, field.Name);
                if (raw == null)
                {
                    raw = AddRowWithFirstCell(sheet, field.Name);
                }

                var cell = EnsureCell(raw.Row, 1);
                if (cell.CellType == CellType.Blank)
                {
                    cell.SetCellValue(field.GetValue(defaultObj).ToString());
                }
            }
        }
    }


    [MenuItem("ECC/Configuration/Create Excels")]
    public static void CreateExcels()
    {
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }
        CreateGlobalTable();

    }

}
