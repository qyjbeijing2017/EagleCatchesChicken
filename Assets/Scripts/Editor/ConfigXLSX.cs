using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Codice.Client.BaseCommands;

public enum ConfigType
{
    Null,
    Global,
    Local,
}

public class ConfigRowClass
{
    private ISheet m_Sheet;
    private IRow m_Row;

    public IRow iRow
    {
        get
        {
            return m_Row;
        }
    }

    public string name
    {
        get
        {
            var cell = m_Row.GetCell(0);
            return cell.ToString();
        }
        set
        {
            var cell = m_Row.GetCell(0);
            if (cell == null)
            {
                cell = m_Row.CreateCell(0);
            }
            cell.SetCellValue(value);
        }
    }

    public List<string> keys
    {
        get
        {
            var keys = new List<string>();
            var headerRow = m_Sheet.GetRow(0);
            if (headerRow == null)
            {
                return keys;
            }
            int cellCount = headerRow.LastCellNum;
            for (int i = 1; i < cellCount; i++)
            {
                var cell = m_Row.GetCell(i);
                if (cell == null) continue;
                keys.Add(cell.ToString());
            }
            return keys;
        }
    }

    public void SetIRow(IRow row, ISheet sheet = null)
    {
        m_Row = row;
        m_Sheet = sheet;
    }

    public ConfigRowClass(IRow row = null, ISheet sheet = null)
    {
        m_Row = row;
        m_Sheet = sheet;
    }

    private ICell GetKeyCell(string key)
    {
        if (m_Sheet == null)
        {
            Debug.LogError("sheet is null");
            return null;
        }
        var headerRow = m_Sheet.GetRow(0);
        if (headerRow == null)
        {
            headerRow = m_Sheet.CreateRow(0);
        }
        int cellCount = headerRow.LastCellNum;
        for (int i = 0; i < cellCount; i++)
        {
            var cell = headerRow.GetCell(i);
            if (cell != null && cell.StringCellValue == key)
            {
                return cell;
            }
        }
        return null;
    }

    private ICell CreateKeyCell(string key)
    {
        if (m_Sheet == null)
        {
            Debug.LogError("sheet is null");
            return null;
        }
        var headerRow = m_Sheet.GetRow(0);
        if (headerRow == null)
        {
            headerRow = m_Sheet.CreateRow(0);
        }
        int cellCount = headerRow.LastCellNum;
        for (int i = 0; i < cellCount; i++)
        {
            var cell = headerRow.GetCell(i);
            if (cell != null && cell.StringCellValue == key)
            {
                return cell;
            }
        }
        return headerRow.CreateCell(cellCount);
    }

    public string GetValue(string key)
    {
        var keyCell = GetKeyCell(key);
        if (keyCell == null) return null;
        if (m_Row == null)
        {
            Debug.LogError("row is null");
            return null;
        }
        var valueCell = m_Row.GetCell(keyCell.ColumnIndex);
        if (valueCell == null) return null;
        return valueCell.ToString();
    }

    public void SetValue(string key, string value)
    {
        var keyCell = CreateKeyCell(key);
        if (m_Row == null)
        {
            Debug.LogError("row is null");
            return;
        }

        var valueCell = m_Row.GetCell(keyCell.ColumnIndex);
        if (valueCell == null)
        {
            valueCell = m_Row.CreateCell(keyCell.ColumnIndex);
        }
        valueCell.SetCellValue(value);
    }
}

public class ConfigSheet
{
    private ISheet m_Sheet;

    public ISheet iSheet
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
        set
        {
            m_Sheet.Workbook.SetSheetName(m_Sheet.Workbook.GetSheetIndex(m_Sheet), value);
        }
    }

    public ConfigType sheetType
    {
        get
        {
            if (m_Sheet == null)
            {
                Debug.LogError("sheet is null");
                return ConfigType.Global;
            }
            var row0 = m_Sheet.GetRow(0);
            if (row0 == null)
            {
                row0 = m_Sheet.CreateRow(0);
            }
            var cell0 = row0.GetCell(0);
            if (cell0 == null)
            {
                cell0 = row0.CreateCell(0);
            }
            if (cell0.StringCellValue == "Global")
            {
                return ConfigType.Global;
            }
            else if (cell0.StringCellValue == "Local")
            {
                return ConfigType.Local;
            }
            else
            {
                return ConfigType.Null;
            }
        }
        set
        {
            if (m_Sheet == null)
            {
                Debug.LogError("sheet is null");
                return;
            }
            var row0 = m_Sheet.GetRow(0);
            if (row0 == null)
            {
                row0 = m_Sheet.CreateRow(0);
            }
            var cell0 = row0.GetCell(0);
            if (cell0 == null)
            {
                cell0 = row0.CreateCell(0);
            }
            if (value == ConfigType.Global)
            {
                cell0.SetCellValue("Global");
            }
            else if (value == ConfigType.Local)
            {
                cell0.SetCellValue("Local");
            }
        }
    }

    private ICell valueCell
    {
        get
        {
            if (sheetType != ConfigType.Global) return null;

            var headerRow = m_Sheet.GetRow(0);
            if (headerRow != null)
            {
                headerRow = m_Sheet.CreateRow(0);
            }
            int cellCount = headerRow.LastCellNum;
            for (int i = 0; i < cellCount; i++)
            {
                var cell = headerRow.GetCell(i);
                if (cell != null && cell.StringCellValue == "Value")
                {
                    return cell;
                }
            }
            return headerRow.CreateCell(cellCount);
        }
    }

    public List<string> keys
    {
        get
        {
            var rowCount = m_Sheet.LastRowNum;
            var keys = new List<string>();
            for (int i = 1; i <= rowCount; i++)
            {
                var row = m_Sheet.GetRow(i);
                if (row == null) continue;
                var cell = row.GetCell(0);
                if (cell == null) continue;
                keys.Add(cell.ToString());
            }
            return keys;
        }
    }

    public string GetValue(string key)
    {
        if (m_Sheet == null)
        {
            Debug.LogError("sheet is null");
            return null;
        }
        if (sheetType != ConfigType.Global)
        {
            Debug.LogError("GetValue only support Global sheet");
            return null;
        }
        int rowCount = m_Sheet.LastRowNum;
        int colunmIndex = valueCell.ColumnIndex;
        for (int i = 1; i <= rowCount; i++)
        {
            var row = m_Sheet.GetRow(i);
            if (row == null) continue;
            var cell = row.GetCell(0);
            if (cell == null) continue;
            if (cell.ToString() == key)
            {
                var valueCell = row.GetCell(colunmIndex);
                if (valueCell == null)
                {
                    return null;
                }
                return valueCell.ToString();
            }
        }
        return null;
    }

    public void SetValue(string key, string value)
    {
        if (m_Sheet == null)
        {
            Debug.LogError("sheet is null");
            return;
        }
        if (sheetType != ConfigType.Global)
        {
            Debug.LogError("SetValue only support Global sheet");
            return;
        }
        int rowCount = m_Sheet.LastRowNum;
        int colunmIndex = valueCell.ColumnIndex;
        for (int i = 1; i <= rowCount; i++)
        {
            var row = m_Sheet.GetRow(i);
            if (row == null) continue;
            var cell = row.GetCell(0);
            if (cell == null) continue;
            if (cell.ToString() == key)
            {
                var valueCell = row.GetCell(colunmIndex);
                if (valueCell == null)
                {
                    valueCell = row.CreateCell(colunmIndex);
                }
                valueCell.SetCellValue(value);
                return;
            }
        }
        var newRow = m_Sheet.CreateRow(rowCount + 1);
        var newKeyCell = newRow.CreateCell(0);
        newKeyCell.SetCellValue(key);
        var newValueCell = newRow.CreateCell(colunmIndex);
        newValueCell.SetCellValue(value);
    }

    private ConfigRowClass m_activeRow = new ConfigRowClass();

    public ConfigRowClass GetRow(string key)
    {
        if (m_Sheet == null)
        {
            Debug.LogError("sheet is null");
            return null;
        }
        if (sheetType != ConfigType.Local)
        {
            Debug.LogError("GetRow only support Local sheet");
            return null;
        }
        var iRow = m_activeRow.iRow;
        int rowCount = m_Sheet.LastRowNum;
        for (int i = 1; i <= rowCount; i++)
        {
            var row = m_Sheet.GetRow(i);
            if (row == null) continue;
            m_activeRow.SetIRow(row, m_Sheet);
            if (m_activeRow.name == key)
            {
                return m_activeRow;
            }
        }
        m_activeRow.SetIRow(iRow, m_Sheet);
        return null;

    }

    public ConfigRowClass CreateRow(string key)
    {
        if (m_Sheet == null)
        {
            Debug.LogError("sheet is null");
            return null;
        }
        if (sheetType != ConfigType.Local)
        {
            Debug.LogError("CreateRow only support Classes sheet");
            return null;
        }
        int rowCount = m_Sheet.LastRowNum;
        for (int i = 1; i <= rowCount; i++)
        {
            var row = m_Sheet.GetRow(i);
            if (row == null) continue;
            m_activeRow.SetIRow(row, m_Sheet);
            if (m_activeRow.name == key)
            {
                return m_activeRow;
            }
        }
        var newRow = m_Sheet.CreateRow(rowCount + 1);
        var newKeyCell = newRow.CreateCell(0);
        newKeyCell.SetCellValue(key);
        m_activeRow.SetIRow(newRow, m_Sheet);
        return m_activeRow;
    }

    public ConfigSheet(ISheet sheet = null)
    {
        m_Sheet = sheet;
    }

    public void SetISheet(ISheet sheet)
    {
        m_Sheet = sheet;
    }
}

public class ConfigXLSX
{
    public static void Decompose(string value, out string name, out List<string> tags)
    {
        var keys = value
        .Replace("\n", "")
        .Replace("\r", "")
        .Replace("\t", "")
        .Replace(" ", "")
        .Split('#');
        name = keys[0];
        tags = new List<string>();
        for (int i = 1; i < keys.Length; i++)
        {
            tags.Add(keys[i]);
        }
    }
    public static string TypeToString(Type t, object instance)
    {
        if (t == typeof(Vector2))
        {
            var value = (Vector2)instance;
            return $"{value.x},{value.y}";
        }
        else if (t == typeof(Vector3))
        {
            var value = (Vector3)instance;
            return $"{value.x},{value.y},{value.z}";
        }
        else if (t == typeof(Vector4))
        {
            var value = (Vector4)instance;
            return $"{value.x},{value.y},{value.z},{value.w}";
        }
        else if (t == typeof(Quaternion))
        {
            var value = (Quaternion)instance;
            return $"{value.x},{value.y},{value.z},{value.w}";
        }
        else if (t == typeof(Color))
        {
            var value = (Color)instance;
            return $"{value.r},{value.g},{value.b},{value.a}";
        }
        else if (t == typeof(Color32))
        {
            var value = (Color32)instance;
            return $"{value.r},{value.g},{value.b},{value.a}";
        }
        else if (typeof(LocalScriptableObject).IsAssignableFrom(t))
        {
            return ((LocalScriptableObject)instance).key;
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)instance;
            var str = "";
            for (int i = 0; i < list.Count; i++)
            {
                str += TypeToString(list[i].GetType(), list[i]);
                if (i < list.Count - 1)
                {
                    str += "|";
                }
            }
            return str;
        }
        else if (typeof(Enum).IsAssignableFrom(t))
        {
            return ((Enum)instance).ToString();
        }


        return JsonUtility.ToJson(instance);
    }

    public static object StringToType(Type t, string value, string configFolder = "Assets/Configurations")
    {
        if (t == typeof(Vector2))
        {
            var values = value.Split(',');
            return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        }
        else if (t == typeof(Vector3))
        {
            var values = value.Split(',');
            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        else if (t == typeof(Vector4))
        {
            var values = value.Split(',');
            return new Vector4(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Quaternion))
        {
            var values = value.Split(',');
            return new Quaternion(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Color))
        {
            var values = value.Split(',');
            return new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Color32))
        {
            var values = value.Split(',');
            return new Color32(byte.Parse(values[0]), byte.Parse(values[1]), byte.Parse(values[2]), byte.Parse(values[3]));
        }
        else if (typeof(LocalScriptableObject).IsAssignableFrom(t))
        {
            return AssetDatabase.LoadAssetAtPath($"{configFolder}/{t.Name}_{value}.asset", t);
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)Activator.CreateInstance(t);
            var values = value.Split('|');
            for (int i = 0; i < values.Length; i++)
            {
                list.Add(StringToType(t.GetGenericArguments()[0], values[i]));
            }
            return list;
        }
        else if (typeof(Enum).IsAssignableFrom(t))
        {
            return Enum.Parse(t, value);
        }
        else if (t == typeof(string))
        {
            return value;
        }
        else if (t == typeof(int))
        {
            return int.Parse(value);
        }
        else if (t == typeof(float))
        {
            return float.Parse(value);
        }
        else if (t == typeof(bool))
        {
            return bool.Parse(value);
        }
        else if (t == typeof(long))
        {
            return long.Parse(value);
        }

        return JsonUtility.FromJson(value, t);
    }

    private string m_Path;
    private IWorkbook m_Workbook;

    public ConfigXLSX(string path)
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

    private ConfigSheet m_activeSheet = new ConfigSheet();

    public ConfigSheet activeSheet
    {
        get
        {
            return m_activeSheet;
        }
    }

    public List<string> sheetNames
    {
        get
        {
            var sheetNames = new List<string>();
            int sheetCount = m_Workbook.NumberOfSheets;
            for (int i = 0; i < sheetCount; i++)
            {
                var sheet = m_Workbook.GetSheetAt(i);
                sheetNames.Add(sheet.SheetName);
            }
            return sheetNames;
        }
    }

    public ConfigSheet GetSheet(string name)
    {
        var iSheet = m_activeSheet.iSheet;
        int sheetCount = m_Workbook.NumberOfSheets;
        for (int i = 0; i < sheetCount; i++)
        {
            var sheet = m_Workbook.GetSheetAt(i);
            m_activeSheet.SetISheet(sheet);
            if (m_activeSheet.name == name)
            {
                return m_activeSheet;
            }
        }
        m_activeSheet.SetISheet(iSheet);
        return null;
    }

    public ConfigSheet CreateSheet(string name)
    {
        int sheetCount = m_Workbook.NumberOfSheets;
        for (int i = 0; i < sheetCount; i++)
        {
            var sheet = m_Workbook.GetSheetAt(i);
            m_activeSheet.SetISheet(sheet);
            if (m_activeSheet.name == name)
            {
                return m_activeSheet;
            }
        }
        var newSheet = m_Workbook.CreateSheet(name);
        m_activeSheet.SetISheet(newSheet);
        return m_activeSheet;
    }

    public void SerializeSheet(List<Type> globalTypes, List<Type> localTypes, string configFolder)
    {
        foreach (var localType in localTypes)
        {

        }

        foreach (var globalType in globalTypes)
        {
            
        }
    }

    public void DeserializeSheet<T>(T instance, string configFolder)
    {

    }
}
