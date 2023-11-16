using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;


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
            throw new Exception("sheet is null");
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
            throw new Exception("sheet is null");
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
        var keyCell = headerRow.CreateCell(cellCount);
        keyCell.SetCellValue(key);
        return keyCell;
    }

    public string GetValue(string key)
    {
        var keyCell = GetKeyCell(key);
        if (keyCell == null) return "";
        if (m_Row == null)
        {
            Debug.LogError("row is null");
            throw new Exception("row is null");
        }
        var valueCell = m_Row.GetCell(keyCell.ColumnIndex);
        if (valueCell == null) return "";
        return valueCell.ToString();
    }

    public void SetValue(string key, string value)
    {
        var keyCell = CreateKeyCell(key);
        if (m_Row == null)
        {
            Debug.LogError("row is null");
            throw new Exception("row is null");
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
                throw new Exception("sheet is null");
            }
            var row0 = m_Sheet.GetRow(0);
            if (row0 == null)
            {
                return ConfigType.Null;
            }
            var cell0 = row0.GetCell(0);
            if (cell0 == null)
            {
                return ConfigType.Null;
            }
            if (cell0.ToString() == "Global")
            {
                return ConfigType.Global;
            }
            else if (cell0.ToString() == "Local")
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
                throw new Exception("sheet is null");
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
            else
            {
                cell0.SetCellValue("");
            }
        }
    }

    private ICell valueCell
    {
        get
        {
            if (sheetType != ConfigType.Global) return null;

            var headerRow = m_Sheet.GetRow(0);
            if (headerRow == null)
            {
                headerRow = m_Sheet.CreateRow(0);
            }
            int cellCount = Mathf.Max(headerRow.LastCellNum, 1);
            for (int i = 1; i < cellCount; i++)
            {
                var cell = headerRow.GetCell(i);
                if (cell != null && cell.ToString() == "Value")
                {
                    return cell;
                }
            }
            var valueCell = headerRow.CreateCell(cellCount);
            valueCell.SetCellValue("Value");
            return valueCell;
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
            throw new Exception("sheet is null");
        }
        if (sheetType != ConfigType.Global)
        {
            Debug.LogError("GetValue only support Global sheet");
            throw new Exception("GetValue only support Global sheet");
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
                    return "";
                }
                return valueCell.ToString();
            }
        }
        return "";
    }

    public void SetValue(string key, string value)
    {
        if (m_Sheet == null)
        {
            Debug.LogError("sheet is null");
            throw new Exception("sheet is null");
        }
        if (sheetType != ConfigType.Global)
        {
            Debug.LogError("SetValue only support Global sheet");
            throw new Exception("SetValue only support Global sheet");
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
            throw new Exception("sheet is null");
        }
        if (sheetType != ConfigType.Local)
        {
            Debug.LogError("GetRow only support Local sheet");
            throw new Exception("GetRow only support Local sheet");
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
            throw new Exception("sheet is null");
        }
        if (sheetType != ConfigType.Local)
        {
            Debug.LogError("CreateRow only support Classes sheet");
            throw new Exception("CreateRow only support Classes sheet");
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

    public void CreateKey(string key)
    {
        if (sheetType == ConfigType.Local)
        {
            var headerRow = m_Sheet.GetRow(0);
            if (headerRow == null)
            {
                headerRow = m_Sheet.CreateRow(0);
            }
            var cellCount = headerRow.LastCellNum;
            for (int i = 0; i < cellCount; i++)
            {
                var cell = headerRow.GetCell(i);
                if (cell != null && cell.ToString() == key)
                {
                    return;
                }
            }
            headerRow.CreateCell(cellCount).SetCellValue(key);
        }
        else
        {
            var rowCount = m_Sheet.LastRowNum;
            for (int i = 1; i <= rowCount; i++)
            {
                var row = m_Sheet.GetRow(i);
                if (row == null) continue;
                var cell = row.GetCell(0);
                if (cell != null && cell.ToString() == key)
                {
                    return;
                }
            }
            var newRow = m_Sheet.CreateRow(rowCount + 1);
            newRow.CreateCell(0).SetCellValue(key);
        }
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
        Debug.Log(t.Name);
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
        else if (typeof(ScriptableObject).IsAssignableFrom(t))
        {
            return ((ScriptableObject)instance).name.Split('_')[1];
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)instance;
            var str = "";
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                if(value != null)
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
        else if (t == typeof(string))
        {
            return (string)instance;
        }
        else if (t == typeof(int))
        {
            return ((int)instance).ToString();
        }
        else if (t == typeof(float))
        {
            return ((float)instance).ToString();
        }
        else if (t == typeof(bool))
        {
            return ((bool)instance).ToString();
        }
        else if (t == typeof(long))
        {
            return ((long)instance).ToString();
        }
        else if (instance == null)
        {
            return "";
        }

        Debug.LogError($"TypeToString not support type {t.Name}");
        return JsonUtility.ToJson(instance);
    }

    public static object StringToType(Type t, string value, string configFolder)
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
        else if (typeof(ScriptableObject).IsAssignableFrom(t))
        {
            return AssetDatabase.LoadAssetAtPath($"{configFolder}/{t.Name.Replace("ScriptableObject", "")}_{value}.asset", t);
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)Activator.CreateInstance(t);
            var values = value.Split('|');
            if(values.Length == 1 && values[0] == "")
            {
                return list;
            }
            for (int i = 0; i < values.Length; i++)
            {
                list.Add(StringToType(t.GetGenericArguments()[0], values[i], configFolder));
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
        else if (value == "")
        {
            return null;
        }

        Debug.LogError($"StringToType not support type {t.Name}");
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
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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

    private bool IsGlobal(Type type)
    {
        return type.GetTypeInfo().GetCustomAttribute<LocalScriptableObjectAttribute>() == null;
    }

    public void SerializeGlobal(Type type, object instance)
    {
        var sheet = CreateSheet(type.Name.Replace("ScriptableObject", ""));
        sheet.sheetType = ConfigType.Global;
        var fields = type.GetFields();
        foreach (var filed in fields)
        {
            var key = filed.Name;
            var valueString = TypeToString(filed.FieldType, filed.GetValue(instance));
            sheet.SetValue(key, valueString);
        }
    }

    public void SerializeLocal(Type type, object instance)
    {
        var sheet = CreateSheet(type.Name.Replace("ScriptableObject", ""));
        sheet.sheetType = ConfigType.Local;
        var row = sheet.CreateRow(((ScriptableObject)instance).name.Split('_')[1]);
        var fields = type.GetFields();
        foreach (var filed in fields)
        {
            var key = filed.Name;
            if (key == "key") continue;
            var valueString = TypeToString(filed.FieldType, filed.GetValue(instance));
            row.SetValue(key, valueString);
        }
    }

    public void SerializeAll(List<Type> types, string configFolder)
    {

        var filesPath = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { configFolder });
        foreach (var filePath in filesPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(filePath));
            var names = fileName.Split('_');
            var typeName = names[0];

            var type = types.Find(t => t.Name == typeName || t.Name == $"{typeName}ScriptableObject");
            if (type == null)
            {
                Debug.LogWarning($"ScriptableObject {fileName} is not exist");
                continue;
            }
            var isGlobal = IsGlobal(type);

            if (isGlobal)
            {
                SerializeGlobal(type, AssetDatabase.LoadAssetAtPath($"{configFolder}/{fileName}.asset", type));
            }
            else
            {
                SerializeLocal(type, AssetDatabase.LoadAssetAtPath($"{configFolder}/{fileName}.asset", type));
            }
        }
    }

    public void DeserializeGlobal(Type type, string configFolder)
    {
        var sheet = GetSheet(type.Name.Replace("ScriptableObject", ""));
        if (sheet == null)
        {
            Debug.LogWarning($"Sheet {type.Name} is not exist");
            return;
        }
        var instance = AssetDatabase.LoadAssetAtPath($"{configFolder}/{type.Name.Replace("ScriptableObject", "")}.asset", type);
        if (instance == null)
        {
            instance = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(instance, $"{configFolder}/{type.Name.Replace("ScriptableObject", "")}.asset");
        }
        var fields = type.GetFields();
        foreach (var filed in fields)
        {
            var key = filed.Name;
            var valueString = sheet.GetValue(key);
            if (valueString == null) continue;
            filed.SetValue(instance, StringToType(filed.FieldType, valueString, configFolder));
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(instance);
    }

    public void DeserializeLocal(Type type, string configFolder)
    {
        var sheet = GetSheet(type.Name.Replace("ScriptableObject", ""));
        if (sheet == null)
        {
            Debug.LogWarning($"Sheet {type.Name} is not exist");
            return;
        }
        var fields = type.GetFields();
        var rows = sheet.keys;
        foreach (var row in rows)
        {
            var instance = AssetDatabase.LoadAssetAtPath($"{configFolder}/{type.Name.Replace("ScriptableObject", "")}_{row}.asset", type);
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(instance, $"{configFolder}/{type.Name.Replace("ScriptableObject", "")}_{row}.asset");
            }
            var rowClass = sheet.GetRow(row);
            foreach (var filed in fields)
            {
                var key = filed.Name;
                if (key == "key") continue;
                var valueString = rowClass.GetValue(key);
                if (valueString == null) continue;
                filed.SetValue(instance, StringToType(filed.FieldType, valueString, configFolder));
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(instance);
        }
    }

    public void DeserializeAll(List<Type> types, string configFolder)
    {
        foreach (var type in types)
        {
            var isGlobal = IsGlobal(type);

            if (isGlobal)
            {
                DeserializeGlobal(type, configFolder);
            }
            else
            {
                DeserializeLocal(type, configFolder);
            }
        }
    }

    public void CreateSheets(List<Type> types)
    {
        foreach (var type in types)
        {
            var sheet = CreateSheet(type.Name.Replace("ScriptableObject", ""));
            var isGlobal = IsGlobal(type);
            if (isGlobal)
            {
                sheet.sheetType = ConfigType.Global;
            }
            else
            {
                sheet.sheetType = ConfigType.Local;
            }
            var defaultInstance = ScriptableObject.CreateInstance(type);
            foreach (var field in type.GetFields())
            {
                sheet.CreateKey(field.Name);
                if (isGlobal)
                {
                    if (sheet.GetValue(field.Name) == "")
                        sheet.SetValue(field.Name, TypeToString(field.FieldType, field.GetValue(defaultInstance)));
                }
                else
                {
                    var row = sheet.CreateRow("default");
                    if (row.GetValue(field.Name) == "")
                        row.SetValue(field.Name, TypeToString(field.FieldType, field.GetValue(defaultInstance)));
                }
            }
        }
    }
}
