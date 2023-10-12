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

        // Act
        ECCEditor.BuildHotFix();

        // Assert

    }

}
