using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System;
using UnityEngine;

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

    public void SetIRow(IRow row, ISheet sheet = null)
    {
        m_Row = row;
        m_Sheet = sheet;
    }

    public ConfigRowClass(IRow row  = null, ISheet sheet = null)
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

}
