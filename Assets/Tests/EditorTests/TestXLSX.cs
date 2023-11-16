using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System;

public class TestXLSX
{
    static string s_Workspace = $"{Application.dataPath}/ConfigurationTest";
    static string s_WorkspaceRelative = $"Assets/ConfigurationTest";
    static string s_TestFilePath = $"{s_Workspace}/Test.xlsx";
    static string s_sheetName = "test_sheet";

    [SetUp]
    public void SetUp()
    {
        if (!Directory.Exists(s_Workspace))
        {
            Directory.CreateDirectory(s_Workspace);
        }

        var files = Directory.GetFiles(s_Workspace);
        foreach (var file in files)
        {
            File.Delete(file);
        }
        AssetDatabase.Refresh();
    }

    [Test]
    public void FileExport()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = ScriptableObject.CreateInstance<TestScriptableObject>();


        // Act
        sheetUnderTest.SerializeFormScriptableObject(scriptableObjectUnderTest);
        xlsxUnderTest.Save();

        // Assert
        Assert.IsTrue(File.Exists(s_TestFilePath));
    }

    [Test]
    public void SimpleWriteAndReadFromScriptableObject()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = ScriptableObject.CreateInstance<TestScriptableObject>();
        scriptableObjectUnderTest.stringValue = "test_string";
        scriptableObjectUnderTest.intValue = 1;
        scriptableObjectUnderTest.floatValue = 1.5f;
        scriptableObjectUnderTest.boolValue = true;
        scriptableObjectUnderTest.intList = new List<int>() { 4, 5, 6 };
        scriptableObjectUnderTest.stringList = new List<string>() { "string4", "string5", "string6" };
        scriptableObjectUnderTest.floatList = new List<float>() { 1.2f, 2.3f, 3.4f };
        scriptableObjectUnderTest.boolList = new List<bool>() { false, true, true };

        // Act
        sheetUnderTest.SerializeFormScriptableObject(scriptableObjectUnderTest);
        xlsxUnderTest.Save();

        var scriptableObjectCheck = ScriptableObject.CreateInstance<TestScriptableObject>();
        var xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];
        sheetCheck.DeserializeToScriptableObject(scriptableObjectCheck);

        // Assert
        Assert.AreEqual(scriptableObjectUnderTest.stringValue, scriptableObjectCheck.stringValue);
        Assert.AreEqual(scriptableObjectUnderTest.intValue, scriptableObjectCheck.intValue);
        Assert.AreEqual(scriptableObjectUnderTest.floatValue, scriptableObjectCheck.floatValue);
        Assert.AreEqual(scriptableObjectUnderTest.boolValue, scriptableObjectCheck.boolValue);
        Assert.AreEqual(scriptableObjectUnderTest.intList.Count, scriptableObjectCheck.intList.Count);
        Assert.AreEqual(scriptableObjectUnderTest.intList[0], scriptableObjectCheck.intList[0]);
        Assert.AreEqual(scriptableObjectUnderTest.intList[1], scriptableObjectCheck.intList[1]);
        Assert.AreEqual(scriptableObjectUnderTest.intList[2], scriptableObjectCheck.intList[2]);
        Assert.AreEqual(scriptableObjectUnderTest.stringList.Count, scriptableObjectCheck.stringList.Count);
        Assert.AreEqual(scriptableObjectUnderTest.stringList[0], scriptableObjectCheck.stringList[0]);
        Assert.AreEqual(scriptableObjectUnderTest.stringList[1], scriptableObjectCheck.stringList[1]);
        Assert.AreEqual(scriptableObjectUnderTest.stringList[2], scriptableObjectCheck.stringList[2]);
        Assert.AreEqual(scriptableObjectUnderTest.floatList.Count, scriptableObjectCheck.floatList.Count);
        Assert.AreEqual(scriptableObjectUnderTest.floatList[0], scriptableObjectCheck.floatList[0]);
        Assert.AreEqual(scriptableObjectUnderTest.floatList[1], scriptableObjectCheck.floatList[1]);
        Assert.AreEqual(scriptableObjectUnderTest.floatList[2], scriptableObjectCheck.floatList[2]);
        Assert.AreEqual(scriptableObjectUnderTest.boolList.Count, scriptableObjectCheck.boolList.Count);
        Assert.AreEqual(scriptableObjectUnderTest.boolList[0], scriptableObjectCheck.boolList[0]);
        Assert.AreEqual(scriptableObjectUnderTest.boolList[1], scriptableObjectCheck.boolList[1]);
        Assert.AreEqual(scriptableObjectUnderTest.boolList[2], scriptableObjectCheck.boolList[2]);
    }

    [Test]
    public void WriteGlobal()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectCheck = ScriptableObject.CreateInstance<TestScriptableObject>();

        // Act
        sheetUnderTest.sheetType = XLSXSheetType.Global;
        sheetUnderTest["intValue"] = 1;
        sheetUnderTest["stringValue"] = "test value";
        sheetUnderTest["floatValue"] = 1.8f;
        sheetUnderTest["boolValue"] = false;
        sheetUnderTest["intList"] = new List<int>() { 7, 8 };
        sheetUnderTest["stringList"] = new List<string>() { "string7", "string8" };
        sheetUnderTest["floatList"] = new List<float>() { 1.7f, 2.8f, 3.9f, 4.0f };
        sheetUnderTest["boolList"] = new List<bool>() { false };
        xlsxUnderTest.Save();

        // Assert
        var xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];
        sheetCheck.DeserializeToScriptableObject(scriptableObjectCheck);
        Assert.AreEqual(1, scriptableObjectCheck.intValue);
        Assert.AreEqual("test value", scriptableObjectCheck.stringValue);
        Assert.AreEqual(1.8f, scriptableObjectCheck.floatValue);
        Assert.AreEqual(false, scriptableObjectCheck.boolValue);
        Assert.AreEqual(2, scriptableObjectCheck.intList.Count);
        Assert.AreEqual(7, scriptableObjectCheck.intList[0]);
        Assert.AreEqual(8, scriptableObjectCheck.intList[1]);
        Assert.AreEqual(2, scriptableObjectCheck.stringList.Count);
        Assert.AreEqual("string7", scriptableObjectCheck.stringList[0]);
        Assert.AreEqual("string8", scriptableObjectCheck.stringList[1]);
        Assert.AreEqual(4, scriptableObjectCheck.floatList.Count);
        Assert.AreEqual(1.7f, scriptableObjectCheck.floatList[0]);
        Assert.AreEqual(2.8f, scriptableObjectCheck.floatList[1]);
        Assert.AreEqual(3.9f, scriptableObjectCheck.floatList[2]);
        Assert.AreEqual(4.0f, scriptableObjectCheck.floatList[3]);
        Assert.AreEqual(1, scriptableObjectCheck.boolList.Count);
        Assert.AreEqual(false, scriptableObjectCheck.boolList[0]);
    }

    [Test]
    public void ReadGlobal()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = ScriptableObject.CreateInstance<TestScriptableObject>();
        scriptableObjectUnderTest.stringValue = "test_read";
        scriptableObjectUnderTest.intValue = 6;
        scriptableObjectUnderTest.floatValue = 4.5f;
        scriptableObjectUnderTest.boolValue = true;
        scriptableObjectUnderTest.intList = new List<int>() { 4, 5 };
        scriptableObjectUnderTest.stringList = new List<string>() { "string4" };
        scriptableObjectUnderTest.floatList = new List<float>() { 1.2f, 2.3f };
        scriptableObjectUnderTest.boolList = new List<bool>() { false, true };
        sheetUnderTest.SerializeFormScriptableObject(scriptableObjectUnderTest);
        xlsxUnderTest.Save();

        // Act
        var xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];

        // Assert
        Assert.AreEqual("test_read", (string)XLSX.StringToType(typeof(string), (string)sheetCheck["stringValue"]));
        Assert.AreEqual(6, (int)XLSX.StringToType(typeof(int), (string)sheetCheck["intValue"]));
        Assert.AreEqual(4.5f, (float)XLSX.StringToType(typeof(float), (string)sheetCheck["floatValue"]));
        Assert.AreEqual(true, (bool)XLSX.StringToType(typeof(bool), (string)sheetCheck["boolValue"]));
        Assert.AreEqual(2, ((List<int>)XLSX.StringToType(typeof(List<int>), (string)sheetCheck["intList"])).Count);
        Assert.AreEqual(4, ((List<int>)XLSX.StringToType(typeof(List<int>), (string)sheetCheck["intList"]))[0]);
        Assert.AreEqual(5, ((List<int>)XLSX.StringToType(typeof(List<int>), (string)sheetCheck["intList"]))[1]);
        Assert.AreEqual(1, ((List<string>)XLSX.StringToType(typeof(List<string>), (string)sheetCheck["stringList"])).Count);
        Assert.AreEqual("string4", ((List<string>)XLSX.StringToType(typeof(List<string>), (string)sheetCheck["stringList"]))[0]);
        Assert.AreEqual(2, ((List<float>)XLSX.StringToType(typeof(List<float>), (string)sheetCheck["floatList"])).Count);
        Assert.AreEqual(1.2f, ((List<float>)XLSX.StringToType(typeof(List<float>), (string)sheetCheck["floatList"]))[0]);
        Assert.AreEqual(2.3f, ((List<float>)XLSX.StringToType(typeof(List<float>), (string)sheetCheck["floatList"]))[1]);
        Assert.AreEqual(2, ((List<bool>)XLSX.StringToType(typeof(List<bool>), (string)sheetCheck["boolList"])).Count);
        Assert.AreEqual(false, ((List<bool>)XLSX.StringToType(typeof(List<bool>), (string)sheetCheck["boolList"]))[0]);
        Assert.AreEqual(true, ((List<bool>)XLSX.StringToType(typeof(List<bool>), (string)sheetCheck["boolList"]))[1]);

        Assert.AreEqual((int)XLSX.StringToType(typeof(int), (string)sheetCheck["intValue"]), sheetCheck.GetValue<int>("intValue"));
        Assert.AreEqual((string)XLSX.StringToType(typeof(string), (string)sheetCheck["stringValue"]), sheetCheck.GetValue<string>("stringValue"));
        Assert.AreEqual((float)XLSX.StringToType(typeof(float), (string)sheetCheck["floatValue"]), sheetCheck.GetValue<float>("floatValue"));
        Assert.AreEqual((bool)XLSX.StringToType(typeof(bool), (string)sheetCheck["boolValue"]), sheetCheck.GetValue<bool>("boolValue"));
    }

    [Test]
    public void WriteWithString()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectCheck = ScriptableObject.CreateInstance<TestScriptableObject>();

        // Act
        sheetUnderTest.sheetType = XLSXSheetType.Global;
        sheetUnderTest["intValue"] = "3";
        sheetUnderTest["stringValue"] = "test value2";
        sheetUnderTest["floatValue"] = "1.9";
        sheetUnderTest["boolValue"] = "false";
        sheetUnderTest["intList"] = "7|8|9";
        sheetUnderTest["stringList"] = "string7";
        sheetUnderTest["floatList"] = "1.7|2.8|3.9|4.0|5.2";
        sheetUnderTest["boolList"] = "false|true";
        xlsxUnderTest.Save();

        // Assert
        var xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];
        sheetCheck.DeserializeToScriptableObject(scriptableObjectCheck);
        Assert.AreEqual(3, scriptableObjectCheck.intValue);
        Assert.AreEqual("test value2", scriptableObjectCheck.stringValue);
        Assert.AreEqual(1.9f, scriptableObjectCheck.floatValue);
        Assert.AreEqual(false, scriptableObjectCheck.boolValue);
        Assert.AreEqual(3, scriptableObjectCheck.intList.Count);
        Assert.AreEqual(7, scriptableObjectCheck.intList[0]);
        Assert.AreEqual(8, scriptableObjectCheck.intList[1]);
        Assert.AreEqual(9, scriptableObjectCheck.intList[2]);
        Assert.AreEqual(1, scriptableObjectCheck.stringList.Count);
        Assert.AreEqual("string7", scriptableObjectCheck.stringList[0]);
        Assert.AreEqual(5, scriptableObjectCheck.floatList.Count);
        Assert.AreEqual(1.7f, scriptableObjectCheck.floatList[0]);
        Assert.AreEqual(2.8f, scriptableObjectCheck.floatList[1]);
        Assert.AreEqual(3.9f, scriptableObjectCheck.floatList[2]);
        Assert.AreEqual(4.0f, scriptableObjectCheck.floatList[3]);
        Assert.AreEqual(5.2f, scriptableObjectCheck.floatList[4]);
        Assert.AreEqual(2, scriptableObjectCheck.boolList.Count);
        Assert.AreEqual(false, scriptableObjectCheck.boolList[0]);
        Assert.AreEqual(true, scriptableObjectCheck.boolList[1]);
    }

    [Test]
    public void ReadEachString()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        var scriptableObjectUnderTest = ScriptableObject.CreateInstance<TestScriptableObject>();
        scriptableObjectUnderTest.stringValue = "test_read";
        scriptableObjectUnderTest.intValue = 6;
        scriptableObjectUnderTest.floatValue = 4.5f;
        scriptableObjectUnderTest.boolValue = true;
        scriptableObjectUnderTest.intList = new List<int>() { 4, 5 };
        scriptableObjectUnderTest.stringList = new List<string>() { "string4" };
        scriptableObjectUnderTest.floatList = new List<float>() { 1.2f, 2.3f };
        scriptableObjectUnderTest.boolList = new List<bool>() { false, true };
        sheetUnderTest.SerializeFormScriptableObject(scriptableObjectUnderTest);
        xlsxUnderTest.Save();

        // Act
        var xlsxCheck = new XLSX(s_TestFilePath);
        var sheetCheck = xlsxCheck[s_sheetName];


        // Assert
        Assert.AreEqual("test_read", sheetCheck["stringValue"]);
        Assert.AreEqual("6", sheetCheck["intValue"]);
        Assert.AreEqual("4.5", sheetCheck["floatValue"]);
        Assert.AreEqual("True", sheetCheck["boolValue"]);
        Assert.AreEqual("4|5", sheetCheck["intList"]);
        Assert.AreEqual("string4", sheetCheck["stringList"]);
        Assert.AreEqual("1.2|2.3", sheetCheck["floatList"]);
        Assert.AreEqual("False|True", sheetCheck["boolList"]);
    }

    [Test]
    public void CreateSheetByType()
    {
        // Arrange
        var xlsxUnderTest = new XLSX(s_TestFilePath);

        // Act
        xlsxUnderTest.CreateSheets(new List<Type>(){
            typeof(TestLocalScriptableObject),
            typeof(Test2ScriptableObject),
        });

        Assert.IsNotNull(xlsxUnderTest.GetSheet("TestLocal"));
        Assert.IsNotNull(xlsxUnderTest.GetSheet("Test2"));
    }

    [Test]
    public void ReferenceScriptObject()
    {
        // Arrange
        var scriptableObjectUnderTest = ScriptableObject.CreateInstance<Test2ScriptableObject>();
        var testLocalScriptableObject = ScriptableObject.CreateInstance<TestLocalScriptableObject>();
        testLocalScriptableObject.intValue = 1;
        // name must be set to 'ObjectNameWithoutScriptableObject' + '_' + 'key'
        testLocalScriptableObject.name = $"{typeof(TestLocalScriptableObject).Name.Replace("ScriptableObject", "")}_test";
        scriptableObjectUnderTest.objectList.Add(testLocalScriptableObject);


        // must save LocalScriptableObject To AssetDatabase before serialize Test2ScriptableObject
        AssetDatabase.CreateAsset(testLocalScriptableObject, $"{s_WorkspaceRelative}/{testLocalScriptableObject.name}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(scriptableObjectUnderTest);

        // Act
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        var sheetUnderTest = xlsxUnderTest[s_sheetName];
        sheetUnderTest.SerializeFormScriptableObject(scriptableObjectUnderTest);

        // Assert
        // objectList will be serialized as the key of LocalScriptableObject
        Assert.AreEqual("test", sheetUnderTest["objectList"]);
        // We didn't serialize TestLocalScriptableObject, so it should be null
        Assert.IsNull(xlsxUnderTest.GetSheet("TestLocal"));
    }


    [Test]
    public void SerializeAll()
    {
        // Arrange
        var scriptableObjectUnderTest = ScriptableObject.CreateInstance<Test2ScriptableObject>();

        var testLocalScriptableObject1 = ScriptableObject.CreateInstance<TestLocalScriptableObject>();
        testLocalScriptableObject1.intValue = 1;

        testLocalScriptableObject1.name = $"{typeof(TestLocalScriptableObject).Name.Replace("ScriptableObject", "")}_localTestOne";

        var testLocalScriptableObject2 = ScriptableObject.CreateInstance<TestLocalScriptableObject>();
        testLocalScriptableObject2.intValue = 2;
        testLocalScriptableObject2.name = "TestLocal_localTestTwo";

        scriptableObjectUnderTest.objectList = new List<TestLocalScriptableObject>() { testLocalScriptableObject1, testLocalScriptableObject2 };

        // attention to scriptableObjectUnderTest, it must be created after testLocalScriptableObject1 and testLocalScriptableObject2
        AssetDatabase.CreateAsset(testLocalScriptableObject1, $"{s_WorkspaceRelative}/{testLocalScriptableObject1.name}.asset");
        AssetDatabase.CreateAsset(testLocalScriptableObject2, $"{s_WorkspaceRelative}/{testLocalScriptableObject2.name}.asset");
        AssetDatabase.CreateAsset(scriptableObjectUnderTest, $"{s_WorkspaceRelative}/{typeof(Test2ScriptableObject).Name.Replace("ScriptableObject", "")}.asset");


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(scriptableObjectUnderTest);
        EditorUtility.SetDirty(testLocalScriptableObject1);
        EditorUtility.SetDirty(testLocalScriptableObject2);

        // Act
        var xlsxUnderTest = new XLSX(s_TestFilePath);
        xlsxUnderTest.SerializeAll(new List<Type>(){
            typeof(TestLocalScriptableObject),
            typeof(Test2ScriptableObject),
        }, s_WorkspaceRelative);
        xlsxUnderTest.Save();

        // Assert
        var xlsxCheck = new XLSX(s_TestFilePath);
        Assert.NotNull(xlsxCheck.GetSheet("Test2"));
        Assert.NotNull(xlsxCheck.GetSheet("TestLocal"));
        Assert.NotNull(xlsxCheck.GetSheet("TestLocal").GetRow("localTestOne"));
        Assert.NotNull(xlsxCheck.GetSheet("TestLocal").GetRow("localTestTwo"));
        Assert.AreEqual("1", xlsxCheck.GetSheet("TestLocal").GetRow("localTestOne")["intValue"]);
        Assert.AreEqual("2", xlsxCheck.GetSheet("TestLocal").GetRow("localTestTwo")["intValue"]);
        Assert.AreEqual("localTestOne|localTestTwo", xlsxCheck.GetSheet("Test2")["objectList"]);
    }

    [Test]
    public void DeserializeAll()
    {
        // Arrange
        var scriptableObjectUnderTest = ScriptableObject.CreateInstance<Test2ScriptableObject>();

        var testLocalScriptableObject1 = ScriptableObject.CreateInstance<TestLocalScriptableObject>();
        testLocalScriptableObject1.intValue = 1;

        testLocalScriptableObject1.name = $"{typeof(TestLocalScriptableObject).Name.Replace("ScriptableObject", "")}_localTestOne";

        var testLocalScriptableObject2 = ScriptableObject.CreateInstance<TestLocalScriptableObject>();
        testLocalScriptableObject2.intValue = 2;
        testLocalScriptableObject2.name = "TestLocal_localTestTwo";

        scriptableObjectUnderTest.objectList = new List<TestLocalScriptableObject>() { testLocalScriptableObject1, testLocalScriptableObject2 };

        // attention to scriptableObjectUnderTest, it must be created after testLocalScriptableObject1 and testLocalScriptableObject2
        AssetDatabase.CreateAsset(testLocalScriptableObject1, $"{s_WorkspaceRelative}/{testLocalScriptableObject1.name}.asset");
        AssetDatabase.CreateAsset(testLocalScriptableObject2, $"{s_WorkspaceRelative}/{testLocalScriptableObject2.name}.asset");
        AssetDatabase.CreateAsset(scriptableObjectUnderTest, $"{s_WorkspaceRelative}/{typeof(Test2ScriptableObject).Name.Replace("ScriptableObject", "")}.asset");


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(scriptableObjectUnderTest);
        EditorUtility.SetDirty(testLocalScriptableObject1);
        EditorUtility.SetDirty(testLocalScriptableObject2);

        var xlsxUnderTest = new XLSX(s_TestFilePath);
        xlsxUnderTest.SerializeAll(new List<Type>(){
            typeof(TestLocalScriptableObject),
            typeof(Test2ScriptableObject),
        }, s_WorkspaceRelative);
        xlsxUnderTest.Save();

        // Act
        var xlsxDeserialize = new XLSX(s_TestFilePath);
        xlsxDeserialize.GetSheet("Test2")["objectList"] = "localTestOne";
        xlsxDeserialize.GetSheet("TestLocal").GetRow("localTestOne")["intValue"] = "3";
        xlsxDeserialize.DeserializeAll(new List<Type>(){
            typeof(TestLocalScriptableObject),
            typeof(Test2ScriptableObject),
        }, s_WorkspaceRelative);

        // Assert
        var test2Check =  AssetDatabase.LoadAssetAtPath<Test2ScriptableObject>($"{s_WorkspaceRelative}/Test2.asset");
        var testLocalCheck =  AssetDatabase.LoadAssetAtPath<TestLocalScriptableObject>($"{s_WorkspaceRelative}/TestLocal_localTestOne.asset");
        Assert.AreEqual(3, testLocalCheck.intValue);
        Assert.AreEqual(1, test2Check.objectList.Count);
        Assert.AreEqual(3, test2Check.objectList[0].intValue);
        Assert.AreEqual(testLocalCheck, test2Check.objectList[0]);
    }


    [TearDown]
    public void TearDown()
    {
        var files = Directory.GetFiles(s_Workspace);
        foreach (var file in files)
        {
            File.Delete(file);
        }
        AssetDatabase.DeleteAsset(s_WorkspaceRelative);
        AssetDatabase.Refresh();
    }

}
