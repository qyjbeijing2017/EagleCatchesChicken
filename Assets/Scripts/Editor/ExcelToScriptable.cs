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

    static IRow FindRowFromFirstCell(ISheet sheet, string value)
    {
        for (int i = 0; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null) continue;
            if (row.GetCell(0) != null && row.GetCell(0).StringCellValue == value)
            {
                return row;
            }
        }
        return null;
    }

    static IRow AddRowWithFirstCell(ISheet sheet, string value)
    {
        var row = sheet.CreateRow(sheet.LastRowNum + 1);
        var cell = row.CreateCell(0);
        cell.SetCellValue(value);
        return row;
    }

    static IRow EnsureRowWithFirstCell(ISheet sheet, string value)
    {
        var row = FindRowFromFirstCell(sheet, value);
        if (row == null)
        {
            row = AddRowWithFirstCell(sheet, value);
        }
        return row;
    }

    static bool IsCellNull(System.Reflection.FieldInfo field, ICell cell)
    {
        if (cell == null)
        {
            return true;
        }
        if (cell.CellType == CellType.Blank)
        {
            return true;
        }
        if (field.FieldType.Name == "String")
        {
            return String.IsNullOrEmpty(cell.StringCellValue);
        }
        else if (field.FieldType.Name == "Int32")
        {
            return cell.NumericCellValue == 0;
        }
        else if (field.FieldType.Name == "Single")
        {
            return cell.NumericCellValue == 0;
        }
        else if (field.FieldType.Name == "Boolean")
        {
            return cell.NumericCellValue == 0;
        }
        else if (field.FieldType.Name == "Vector2")
        {
            return cell.NumericCellValue == 0;
        }
        return false;
    }

    static void SetCell(System.Reflection.FieldInfo field, ScriptableObject obj, ICell cell, bool Cover)
    {
        if (field.FieldType.Name == "String")
        {
            if (Cover || cell.StringCellValue == "")
            {
                cell.SetCellValue((string)field.GetValue(obj));
            }
        }
        else if (field.FieldType.Name == "Int32")
        {
            if (Cover || cell.NumericCellValue == 0)
            {
                cell.SetCellValue((int)field.GetValue(obj));
            }
        }
        else if (field.FieldType.Name == "Single")
        {
            if (Cover || cell.NumericCellValue == 0)
            {
                cell.SetCellValue((float)field.GetValue(obj));
            }
        }
        else if (field.FieldType.Name == "Boolean")
        {
            if (Cover || cell.NumericCellValue == 0)
            {
                cell.SetCellValue((bool)field.GetValue(obj));
            }
        }
        else if (field.FieldType.Name == "Vector2")
        {
            if (Cover || cell.NumericCellValue == 0)
            {
                var vector = (Vector2)field.GetValue(obj);
                cell.SetCellValue(vector.x);
            }
        }
    }

    static void SetField(System.Reflection.FieldInfo field, ScriptableObject obj, ICell cell, bool Cover)
    {
        if (field.FieldType.Name == "String")
        {
            if (Cover || String.IsNullOrEmpty((string)field.GetValue(obj)))
            {
                field.SetValue(obj, cell.StringCellValue);
            }
        }
        else if (field.FieldType.Name == "Int32")
        {
            if (Cover || (int)field.GetValue(obj) == 0)
            {
                field.SetValue(obj, (int)cell.NumericCellValue);
            }
        }
        else if (field.FieldType.Name == "Single")
        {
            if (Cover || (float)field.GetValue(obj) == 0)
            {
                field.SetValue(obj, (float)cell.NumericCellValue);
            }
        }
        else if (field.FieldType.Name == "Boolean")
        {
            if (Cover || (bool)field.GetValue(obj) == false)
            {
                field.SetValue(obj, cell.NumericCellValue == 1);
            }
        }
        else if (field.FieldType.Name == "Vector2")
        {
            if (Cover || ((Vector2)field.GetValue(obj)).magnitude == 0)
            {
                field.SetValue(obj, new Vector2((float)cell.NumericCellValue, (float)cell.NumericCellValue));
            }
        }
    }

    static void CreateGlobalTable()
    {
        using (var fs = new FileStream($"{s_Designer}/global.xlsx", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            var workbook = fs.Length == 0 ? new XSSFWorkbook() : new XSSFWorkbook(fs);
            var sheet = EnsureSheet(workbook, "global");
            var fields = typeof(GlobalScriptableObject).GetFields();
            var defaultObj = new GlobalScriptableObject();

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var row = EnsureRowWithFirstCell(sheet, field.Name);
                var cell = EnsureCell(row, 1);
                SetCell(field, defaultObj, cell, false);
            }
            workbook.Write(fs);
            fs.Close();
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

        Debug.Log("Create Excels Done!");

    }

}
