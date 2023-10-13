using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.AddressableAssets;
using UnityEditor;
using UnityEditor.SceneManagement;

public class TestHybird
{
    [UnityTest]
    public IEnumerator InitHybird()
    {
        // Arrange
        var targetUnderTest = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
        var fileNameUnderTest = "ECCTestHybird";
        var handle = Addressables.LoadAssetAsync<TextAsset>($"Assets/HotFix/{targetUnderTest}/{fileNameUnderTest}.dll.bytes");
        yield return handle;
        var assUnderTest = System.Reflection.Assembly.Load(handle.Result.bytes);

        // Act
        var typeUnderTest = assUnderTest.GetType("TestHybird");
        var methodUnderTest = typeUnderTest.GetMethod("Run");
        methodUnderTest.Invoke(null, null);

        // Assert
        LogAssert.Expect(LogType.Log, "TestHybird Run");
    }

    [Test]
    public void BuildHotFix()
    {
        // Arrange
        var targetUnderTest = EditorUserBuildSettings.activeBuildTarget;
        var targetPath = $"{Application.dataPath}/HotFix/{targetUnderTest}";
        var hotfixNameUnderTest = new List<string> {
            "TestHybird",
            "Loading",
        };

        // Act
        ECCEditor.BuildHotFix();

        // Assert
        Assert.IsTrue(System.IO.Directory.Exists(targetPath));
        foreach (var name in hotfixNameUnderTest)
        {
            Assert.IsTrue(System.IO.File.Exists($"{targetPath}/ECC{name}.dll.bytes"));
            if (Debug.isDebugBuild)
                Assert.IsTrue(System.IO.File.Exists($"{targetPath}/ECC{name}.pdb.bytes"));
        }

    }
}
