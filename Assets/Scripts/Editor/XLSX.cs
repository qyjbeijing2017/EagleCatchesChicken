using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Functions;


public struct XLSXCell
{
    ICell m_Cell;
    private CellType m_cellType;
    private string m_stringValue;
    private double m_numberValue;
    private bool m_boolValue;
    private DateTime m_dateValue;

    public XLSXCell(ICell cell)
    {
        m_Cell = cell;
        m_cellType = CellType.Blank;
        m_stringValue = null;
        m_numberValue = 0;
        m_boolValue = false;
        m_dateValue = DateTime.MinValue;
    }

    public ICell cell
    {
        get
        {
            return m_Cell;
        }
        set
        {
            m_Cell = value;
            switch (m_cellType)
            {
                case CellType.String:
                    m_Cell.SetCellValue(m_stringValue);
                    break;
                case CellType.Numeric:
                    m_Cell.SetCellValue(m_numberValue);
                    break;
                case CellType.Boolean:
                    m_Cell.SetCellValue(m_boolValue);
                    break;
                default:
                    break;
            }
        }
    }

    public static implicit operator string(XLSXCell cell)
    {
        return cell.m_Cell == null ? cell.m_stringValue : cell.m_Cell.StringCellValue;
    }

    public static implicit operator double(XLSXCell cell)
    {
        return cell.m_Cell == null ? cell.m_numberValue : cell.m_Cell.NumericCellValue;
    }

    public static implicit operator bool(XLSXCell cell)
    {
        return cell.m_Cell == null ? cell.m_boolValue : cell.m_Cell.BooleanCellValue;
    }


    public static implicit operator XLSXCell(string value)
    {
        return new XLSXCell(null)
        {
            m_cellType = CellType.String,
            m_stringValue = value
        };
    }

    public static implicit operator XLSXCell(double value)
    {
        return new XLSXCell(null)
        {
            m_cellType = CellType.Numeric,
            m_numberValue = value
        };
    }

    public static implicit operator XLSXCell(bool value)
    {
        return new XLSXCell(null)
        {
            m_cellType = CellType.Boolean,
            m_boolValue = value
        };
    }


    public void setValue(string value)
    {
        if (m_Cell == null)
        {
            m_stringValue = value;
            m_cellType = CellType.String;
        }
        else
        {
            m_Cell.SetCellValue(value);
        }
    }

    public void setValue(double value)
    {
        if (m_Cell == null)
        {
            m_numberValue = value;
            m_cellType = CellType.Numeric;
        }
        else
        {
            m_Cell.SetCellValue(value);
        }
    }

    public void setValue(bool value)
    {
        if (m_Cell == null)
        {
            m_boolValue = value;
            m_cellType = CellType.Boolean;
        }
        else
        {
            m_Cell.SetCellValue(value);
        }
    }

    public void setValue(System.Reflection.FieldInfo fieldInfo, object obj)
    {
        if (fieldInfo.FieldType == typeof(string))
        {
            setValue((string)fieldInfo.GetValue(obj));
        }
        else if (fieldInfo.FieldType == typeof(double))
        {
            setValue((double)fieldInfo.GetValue(obj));
        }
        else if (fieldInfo.FieldType == typeof(int))
        {
            setValue((int)fieldInfo.GetValue(obj));
        }
        else if (fieldInfo.FieldType == typeof(float))
        {
            setValue((float)fieldInfo.GetValue(obj));
        }
        else if (fieldInfo.FieldType == typeof(long))
        {
            setValue((long)fieldInfo.GetValue(obj));
        }
        else if (fieldInfo.FieldType == typeof(bool))
        {
            setValue((bool)fieldInfo.GetValue(obj));
        }
        else
        {
            Debug.LogError($"XLSXCell.setValue: {fieldInfo.Name} type is not supported!");
        }
    }

    public void exportToObject(System.Reflection.FieldInfo fieldInfo, object obj)
    {
        if (fieldInfo.FieldType == typeof(string))
        {
            fieldInfo.SetValue(obj, (string)this);
        }
        else if (fieldInfo.FieldType == typeof(double))
        {
            fieldInfo.SetValue(obj, (double)this);
        }
        else if (fieldInfo.FieldType == typeof(int))
        {
            fieldInfo.SetValue(obj, (int)this);
        }
        else if (fieldInfo.FieldType == typeof(float))
        {
            fieldInfo.SetValue(obj, (float)this);
        }
        else if (fieldInfo.FieldType == typeof(long))
        {
            fieldInfo.SetValue(obj, (long)this);
        }
        else if (fieldInfo.FieldType == typeof(bool))
        {
            fieldInfo.SetValue(obj, (bool)this);
        }
        else
        {
            Debug.LogError($"XLSXCell.setValue: {fieldInfo.Name} type is not supported!");
        }
    }

    public bool IsEmpty()
    {
        if (m_Cell != null)
        {
            return m_Cell.CellType == CellType.Blank;
        }
        else
        {
            return m_cellType == CellType.Blank;
        }
    }

    public void Clear()
    {
        if (m_Cell != null)
        {
            m_Cell.SetCellValue(string.Empty);
        }
        else
        {
            m_cellType = CellType.Blank;
            m_stringValue = null;
            m_numberValue = 0;
            m_boolValue = false;
            m_dateValue = DateTime.MinValue;
        }
    }
}

public struct XLSXRow
{
    XLSXTitles titles;
    IRow m_Row;

    public string name
    {
        get
        {
            var cell = m_Row.GetCell(0);
            if (cell == null) return string.Empty;
            return cell.StringCellValue;
        }
    }

    public IRow row
    {
        get
        {
            return m_Row;
        }
    }


    public XLSXRow(IRow row, XLSXTitles titles)
    {
        m_Row = row;
        this.titles = titles;
    }

    public XLSXCell this[string index]
    {
        get
        {
            var cell = m_Row.GetCell(titles[index]);
            if (cell == null)
            {
                cell = m_Row.CreateCell(titles[index]);
            }
            return new XLSXCell(cell);
        }
        set
        {
            var cell = m_Row.GetCell(titles[index]);
            if (cell == null)
            {
                cell = m_Row.CreateCell(titles[index]);
            }
            value.cell = cell;
        }

    }

    public void serializeFormScriptableObject<T>(T obj, bool cover = true) where T : ScriptableObject
    {
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            var cell = this[field.Name];
            if (cell.IsEmpty() || cover)
            {
                cell.setValue(field, obj);
            }
        }
    }

    public void serializeToScriptableObject<T>(T obj, bool cover = true) where T : ScriptableObject
    {
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            var cell = this[field.Name];
            if (!cell.IsEmpty() || cover)
            {
                cell.exportToObject(field, obj);
            }
        }
    }

    public void serializeFormScriptableObject(Type type, object obj, bool cover = true)
    {
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var cell = this[field.Name];
            if (cell.IsEmpty() || cover)
            {
                cell.setValue(field, obj);
            }
        }
    }

    public void serializeToScriptableObject(Type type, object obj, bool cover = true)
    {
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var cell = this[field.Name];
            if (!cell.IsEmpty() || cover)
            {
                cell.exportToObject(field, obj);
            }
        }
    }
}

public struct XLSXTitles
{
    private IRow m_Row;

    public IRow row
    {
        get
        {
            return m_Row;
        }
    }

    public XLSXTitles(IRow row)
    {
        m_Row = row;
    }

    public int this[string index]
    {
        get
        {
            ICell cell;
            for (int i = 1; i < m_Row.LastCellNum; i++)
            {
                cell = m_Row.GetCell(i);
                if (cell == null) continue;
                if (cell.StringCellValue == index)
                {
                    return i;
                }
            }

            cell = m_Row.CreateCell(m_Row.LastCellNum);
            cell.SetCellValue(index);
            return m_Row.LastCellNum;
        }
    }
}

public struct XLSXSheet
{
    ISheet m_Sheet;

    public ISheet sheet
    {
        get
        {
            return m_Sheet;
        }
    }

    public string name
    {
        get
        {
            return m_Sheet.SheetName;
        }
    }

    public List<XLSXRow> raws
    {
        get
        {
            var list = new List<XLSXRow>();
            for (int i = 1; i <= m_Sheet.LastRowNum; i++)
            {
                var row = m_Sheet.GetRow(i);
                if (row == null) continue;
                list.Add(new XLSXRow(row, titles));
            }
            return list;
        }
    }

    static List<Char> words = new List<Char>()
    {
        'A','B','C','D','E','F','G','H','I','J','K','L','M',
        'N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
    };
    public static int WordsToNumber(string words)
    {
        int number = 0;
        for (int i = 0; i < words.Length; i++)
        {
            number += (int)Mathf.Pow(26, i) * (words[words.Length - 1 - i] - 'A' + 1);
        }
        return number;
    }

    public XLSXCell GetCell(string index)
    {
        var colunmPattern = @"^[A-Z]+";
        var rowPattern = @"\d+$";
        var colunmIndex = XLSXSheet.WordsToNumber(Regex.Match(index, colunmPattern).Value) - 1;
        var rowIndex = int.Parse(Regex.Match(index, rowPattern).Value) - 1;
        var row = m_Sheet.GetRow(rowIndex);
        if (row == null) row = m_Sheet.CreateRow(rowIndex);
        var cell = row.GetCell(colunmIndex);
        if (cell == null) cell = row.CreateCell(colunmIndex);
        return new XLSXCell(cell);
    }

    public XLSXTitles titles;
    public XLSXSheet(ISheet sheet)
    {
        m_Sheet = sheet;
        var titlesRow = m_Sheet.GetRow(0);
        if (titlesRow == null)
        {
            titlesRow = m_Sheet.CreateRow(0);
            titlesRow.CreateCell(0);
        }
        titles = new XLSXTitles(titlesRow);
    }

    public XLSXRow this[string index]
    {
        get
        {
            for (int i = 1; i <= m_Sheet.LastRowNum; i++)
            {
                var row = m_Sheet.GetRow(i);
                if (row == null) continue;
                var cell = row.GetCell(0);
                if (cell == null) continue;
                if (cell.StringCellValue == index)
                {
                    return new XLSXRow(row, titles);
                }
            }
            var newRow = m_Sheet.CreateRow(m_Sheet.LastRowNum + 1);
            newRow.CreateCell(0).SetCellValue(index);
            return new XLSXRow(newRow, titles);
        }
    }

    public void serializeFormScriptableObject<T>(T obj, bool cover = true) where T : ScriptableObject
    {
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            var row = this[field.Name];
            var cell = row["value"];
            if (cell.IsEmpty() || cover)
            {
                cell.setValue(field, obj);
            }
        }
    }

    public void serializeToScriptableObject<T>(T obj, bool cover = true) where T : ScriptableObject
    {
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            var row = this[field.Name];
            var cell = row["value"];
            if (!cell.IsEmpty() || cover)
            {
                cell.exportToObject(field, obj);
            }
        }
    }


    public void serializeFormScriptableObject(Type type, object obj, bool cover = true)
    {
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var row = this[field.Name];
            var cell = row["value"];
            if (cell.IsEmpty() || cover)
            {
                cell.setValue(field, obj);
            }
        }
    }

    public void serializeToScriptableObject(Type type, object obj, bool cover = true)
    {
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var row = this[field.Name];
            var cell = row["value"];
            if (!cell.IsEmpty() || cover)
            {
                cell.exportToObject(field, obj);
            }
        }
    }
}

public class XLSX
{
    private string m_Path;
    private IWorkbook m_Workbook;

    public IWorkbook workbook
    {
        get
        {
            return m_Workbook;
        }
    }

    public List<XLSXSheet> sheets
    {
        get
        {
            var list = new List<XLSXSheet>();
            for (int i = 0; i < m_Workbook.NumberOfSheets; i++)
            {
                list.Add(new XLSXSheet(m_Workbook.GetSheetAt(i)));
            }
            return list;
        }
    }

    public XLSX(string path)
    {
        m_Path = path;
        if (File.Exists(path))
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                m_Workbook = new XSSFWorkbook(stream);
            }
        }
        else
        {
            m_Workbook = new XSSFWorkbook();
        }
    }

    public void Save()
    {
        FileInfo file = new FileInfo(m_Path);
        string savePath = $"{file.Directory}/~catch_{file.Name}";
        try
        {
            using (var stream = File.Open(savePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                m_Workbook.Write(stream);
            }
            if (File.Exists(m_Path))
                File.Delete(m_Path);
            File.Move(savePath, m_Path);
        }
        catch (Exception e)
        {

            if (File.Exists(savePath))
                File.Delete(savePath);
            throw e;
        }
    }

    public XLSXSheet this[string index]
    {
        get
        {
            var sheet = m_Workbook.GetSheet(index);
            if (sheet == null)
            {
                sheet = m_Workbook.CreateSheet(index);
            }
            return new XLSXSheet(sheet);
        }
    }

    public bool HasSheet(string name)
    {
        return m_Workbook.GetSheet(name) != null;
    }



}
