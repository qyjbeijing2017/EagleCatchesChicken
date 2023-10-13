using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestHotfix
{

    [UnityTest]
    public IEnumerator LoadHotFix()
    {
        // Arrange
        var modelNameUnderTest = "ECCTestHybird";
        var gameManagerUnderTest = GameManager.instance;

        // Act
        yield return gameManagerUnderTest.StartCoroutine(gameManagerUnderTest.LoadScript(modelNameUnderTest));
        var assUnderTest = gameManagerUnderTest.GetAssembly(modelNameUnderTest);
        var typeUnderTest = assUnderTest.GetType("TestHybird");
        var methodUnderTest = typeUnderTest.GetMethod("Run");
        methodUnderTest.Invoke(null, null);
        gameManagerUnderTest.ReleaseAssembly(modelNameUnderTest);

        // Assert
        LogAssert.Expect(LogType.Log, "TestHybird Run");
        Assert.AreEqual(gameManagerUnderTest.GetAssembly(modelNameUnderTest), null);
 
    }
}
