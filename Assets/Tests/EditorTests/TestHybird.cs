using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestHybird
{
    [Test]
    public void InitHybird()
    {
        // Arrange


    }

    [Test]
    public void BuildHotFix()
    {
        // Arrange
        var targetUnderTest = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
        var targetPath = $"{Application.dataPath}/HotFix/{targetUnderTest}";
        var hotfixNameUnderTest = new List<string> {
            "ECCTestHybird",
        };

        // Act
        ECCEditor.BuildHotFix();
        
        // Assert
        Assert.IsTrue(System.IO.Directory.Exists(targetPath));
        foreach (var name in hotfixNameUnderTest)
        {
            Assert.IsTrue(System.IO.File.Exists($"{targetPath}/{name}.dll"));
            Assert.IsTrue(System.IO.File.Exists($"{targetPath}/{name}.pdb") == Debug.isDebugBuild);
        }
        
    }

}
