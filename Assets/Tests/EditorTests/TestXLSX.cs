using NUnit.Framework;
using UnityEngine;
using System.IO;

public class TestScriptableObject : ScriptableObject
{
    public int intValue = 888;
    public string stringValue = "default value";
    public float floatValue = 1.5f;
    public bool boolValue = true;
}

public class TestScriptableObject2 : ScriptableObject
{
    public int intValue_2;
    public string stringValue_2;
    public float floatValue_2;
    public bool boolValue_2;
}

public class TestXLSX
{
    static string s_Workspace = $"{Application.dataPath}/Configuration";
    static string s_TestFilePath = $"{s_Workspace}/Test.xlsx";
    static string s_sheetName = "test_sheet";

    [SetUp]
    public void SetUp()
    {
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }
    }

    [Test]
    public void FileExport()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = new TestScriptableObject()
        {
            stringValue = "test_string",
            intValue = 1,
            floatValue = 1.5f,
            boolValue = true
        };

        // Act
        sheetUnderTest.serializeFormScriptableObject(scriptableObjectUnderTest);
        xlsxUnderTest.Save();

        // Assert
        Assert.IsTrue(File.Exists(s_TestFilePath));
    }

    [Test]
    public void Serialize()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = new TestScriptableObject()
        {
            stringValue = "test_string",
            intValue = 1,
            floatValue = 1.5f,
            boolValue = true
        };


        // Act
        sheetUnderTest.serializeFormScriptableObject(scriptableObjectUnderTest);
        xlsxUnderTest.Save();
        XLSX xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];
        var scriptableObjectCheck = new TestScriptableObject();
        sheetCheck.serializeToScriptableObject(scriptableObjectCheck);

        // Assert
        Assert.AreNotEqual(xlsxUnderTest, xlsxCheck);
        Assert.AreEqual(scriptableObjectUnderTest.stringValue, scriptableObjectCheck.stringValue);
        Assert.AreEqual(scriptableObjectUnderTest.intValue, scriptableObjectCheck.intValue);
        Assert.AreEqual(scriptableObjectUnderTest.floatValue, scriptableObjectCheck.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest.boolValue, scriptableObjectCheck.boolValue);
    }

    [Test]
    public void FileRead()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = new TestScriptableObject()
        {
            stringValue = "test_string",
            intValue = 1,
            floatValue = 1.5f,
            boolValue = true
        };
        sheetUnderTest.serializeFormScriptableObject(scriptableObjectUnderTest);
        xlsxUnderTest.Save();

        var xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];

        // Act
        var stringRawUnderTest = sheetCheck["stringValue"];
        var intRawUnderTest = sheetCheck["intValue"];
        var floatRawUnderTest = sheetCheck["floatValue"];
        var boolRawUnderTest = sheetCheck["boolValue"];

        var stringValueUnderTest = (string)stringRawUnderTest["value"];
        var intValueUnderTest = (int)intRawUnderTest["value"];
        var floatValueUnderTest = (float)floatRawUnderTest["value"];
        var boolValueUnderTest = (bool)boolRawUnderTest["value"];


        // Assert
        Assert.AreNotEqual(xlsxUnderTest, xlsxCheck);
        Assert.AreEqual(scriptableObjectUnderTest.stringValue, stringValueUnderTest);
        Assert.AreEqual(scriptableObjectUnderTest.intValue, intValueUnderTest);
        Assert.AreEqual(scriptableObjectUnderTest.floatValue, floatValueUnderTest);
        Assert.AreEqual(scriptableObjectUnderTest.boolValue, boolValueUnderTest);
    }

    [Test]
    public void FileWrite()
    {
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = new TestScriptableObject()
        {
            stringValue = "test_string",
            intValue = 1,
            floatValue = 1.5f,
            boolValue = true
        };

        // Act
        var stringValueRaw = sheetUnderTest["stringValue"];
        var intValueRaw = sheetUnderTest["intValue"];
        var floatValueRaw = sheetUnderTest["floatValue"];
        var boolValueRaw = sheetUnderTest["boolValue"];

        stringValueRaw["value"] = scriptableObjectUnderTest.stringValue;
        intValueRaw["value"] = scriptableObjectUnderTest.intValue;
        floatValueRaw["value"] = scriptableObjectUnderTest.floatValue;
        boolValueRaw["value"] = scriptableObjectUnderTest.boolValue;


        // Assert
        var scriptableObjectCheck = new TestScriptableObject();
        sheetUnderTest.serializeToScriptableObject(scriptableObjectCheck);

        Assert.AreEqual(scriptableObjectUnderTest.stringValue, scriptableObjectCheck.stringValue);
        Assert.AreEqual(scriptableObjectUnderTest.intValue, scriptableObjectCheck.intValue);
        Assert.AreEqual(scriptableObjectUnderTest.floatValue, scriptableObjectCheck.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest.boolValue, scriptableObjectCheck.boolValue);
    }

    [Test]
    public void RowScriptbleObject()
    {

        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest1 = new TestScriptableObject()
        {
            stringValue = "test_string",
            intValue = 1,
            floatValue = 1.5f,
            boolValue = true
        };
        var scriptableObjectUnderTest2 = new TestScriptableObject()
        {
            stringValue = "test_string2",
            intValue = 2,
            floatValue = 1.8f,
            boolValue = false
        };
        var scriptableObjectUnderTest3 = new TestScriptableObject()
        {
            stringValue = "test_string3",
            intValue = 3,
            floatValue = 1.9f,
            boolValue = true
        };


        // Act
        sheetUnderTest["test_row1"].serializeFormScriptableObject(scriptableObjectUnderTest1);
        sheetUnderTest["test_row2"].serializeFormScriptableObject(scriptableObjectUnderTest2);
        sheetUnderTest["test_row3"].serializeFormScriptableObject(scriptableObjectUnderTest3);
        xlsxUnderTest.Save();

        // Assert
        var xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];
        var scriptableObjectCheck1 = new TestScriptableObject();
        var scriptableObjectCheck2 = new TestScriptableObject();
        var scriptableObjectCheck3 = new TestScriptableObject();

        sheetCheck["test_row1"].serializeToScriptableObject(scriptableObjectCheck1);
        sheetCheck["test_row2"].serializeToScriptableObject(scriptableObjectCheck2);
        sheetCheck["test_row3"].serializeToScriptableObject(scriptableObjectCheck3);

        Assert.AreEqual(scriptableObjectUnderTest1.stringValue, scriptableObjectCheck1.stringValue);
        Assert.AreEqual(scriptableObjectUnderTest1.intValue, scriptableObjectCheck1.intValue);
        Assert.AreEqual(scriptableObjectUnderTest1.floatValue, scriptableObjectCheck1.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest1.boolValue, scriptableObjectCheck1.boolValue);

        Assert.AreEqual(scriptableObjectUnderTest2.stringValue, scriptableObjectCheck2.stringValue);
        Assert.AreEqual(scriptableObjectUnderTest2.intValue, scriptableObjectCheck2.intValue);
        Assert.AreEqual(scriptableObjectUnderTest2.floatValue, scriptableObjectCheck2.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest2.boolValue, scriptableObjectCheck2.boolValue);

        Assert.AreEqual(scriptableObjectUnderTest3.stringValue, scriptableObjectCheck3.stringValue);
        Assert.AreEqual(scriptableObjectUnderTest3.intValue, scriptableObjectCheck3.intValue);
        Assert.AreEqual(scriptableObjectUnderTest3.floatValue, scriptableObjectCheck3.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest3.boolValue, scriptableObjectCheck3.boolValue);
    }

    [Test]
    public void DifferentObjectInOneTable()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest1 = new TestScriptableObject()
        {
            stringValue = "test_string",
            intValue = 1,
            floatValue = 1.5f,
            boolValue = true
        };
        var scriptableObjectUnderTest2 = new TestScriptableObject2()
        {
            stringValue_2 = "test_string2",
            intValue_2 = 2,
            floatValue_2 = 1.8f,
            boolValue_2 = false
        };

        // Act
        sheetUnderTest["test_row1"].serializeFormScriptableObject(scriptableObjectUnderTest1);
        sheetUnderTest["test_row2"].serializeFormScriptableObject(scriptableObjectUnderTest2);
        xlsxUnderTest.Save();

        // Assert
        var xlsx = new XLSX(s_TestFilePath);
        var sheet = xlsx[s_sheetName];
        var scriptableObjectCheck1 = new TestScriptableObject();
        var scriptableObjectCheck2 = new TestScriptableObject2();

        sheet["test_row1"].serializeToScriptableObject(scriptableObjectCheck1);
        sheet["test_row2"].serializeToScriptableObject(scriptableObjectCheck2);

        Assert.AreEqual(scriptableObjectUnderTest1.stringValue, scriptableObjectCheck1.stringValue);
        Assert.AreEqual(scriptableObjectUnderTest1.intValue, scriptableObjectCheck1.intValue);
        Assert.AreEqual(scriptableObjectUnderTest1.floatValue, scriptableObjectCheck1.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest1.boolValue, scriptableObjectCheck1.boolValue);

        Assert.AreEqual(scriptableObjectUnderTest2.stringValue_2, scriptableObjectCheck2.stringValue_2);
        Assert.AreEqual(scriptableObjectUnderTest2.intValue_2, scriptableObjectCheck2.intValue_2);
        Assert.AreEqual(scriptableObjectUnderTest2.floatValue_2, scriptableObjectCheck2.floatValue_2);
        Assert.AreEqual(scriptableObjectUnderTest2.boolValue_2, scriptableObjectCheck2.boolValue_2);
    }


    [Test]
    public void Uncover()
    {
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = new TestScriptableObject()
        {
            stringValue = "test_string",
            intValue = 1,
            floatValue = 1.5f,
            boolValue = true
        };
        var rawUnderTest = sheetUnderTest["test_row"];

        // Act
        rawUnderTest["stringValue"] = "raw_string";
        rawUnderTest["intValue"] = 2;
        rawUnderTest.serializeFormScriptableObject(scriptableObjectUnderTest, false);

        // Assert
        var scriptableObjectCheck = new TestScriptableObject();
        sheetUnderTest["test_row"].serializeToScriptableObject(scriptableObjectCheck);

        Assert.AreEqual("raw_string", scriptableObjectCheck.stringValue);
        Assert.AreEqual(2, scriptableObjectCheck.intValue);
        Assert.AreEqual(scriptableObjectUnderTest.floatValue, scriptableObjectCheck.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest.boolValue, scriptableObjectCheck.boolValue);
    }


    [Test]
    public void UnknownObject()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var typeUnderTest = typeof(TestScriptableObject);

        // Act
        var instance = typeUnderTest.Assembly.CreateInstance(typeUnderTest.FullName);
        sheetUnderTest.serializeFormScriptableObject(typeUnderTest, instance, false);


        // Assert
        var scriptableObjectCheck = new TestScriptableObject(){
            stringValue = "other Value",
            intValue = 111,
            floatValue = 1.0f,
            boolValue = false
        };
        sheetUnderTest.serializeToScriptableObject(scriptableObjectCheck);

        Assert.AreEqual("default value", scriptableObjectCheck.stringValue);
        Assert.AreEqual(888, scriptableObjectCheck.intValue);
        Assert.AreEqual(1.5f, scriptableObjectCheck.floatValue);
        Assert.AreEqual(true, scriptableObjectCheck.boolValue);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(s_TestFilePath))
        {
            File.Delete(s_TestFilePath);
        }
    }

}
