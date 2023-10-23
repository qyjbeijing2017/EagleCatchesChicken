using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class TestLoading
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator LoadingScene()
    {
        yield return GameManager.instance.LoadScene("Loading");

        // Arrange
        var loadingUnderTest = GameObject.FindObjectOfType<Loading>();
        var textComponentUnderTest = GameObject.FindObjectOfType<TextMeshProUGUI>();
        var sliderComponentUnderTest = GameObject.FindObjectOfType<Slider>();
        loadingUnderTest.maxValue = 100f;
        var valueUnderTest = 50f;
        var textUnderTest = "Test Loading";

        // Act
        loadingUnderTest.Tick(textUnderTest, valueUnderTest);

        // Assert
        Assert.AreEqual(textComponentUnderTest.text, textUnderTest);
        Assert.AreEqual(sliderComponentUnderTest.value, valueUnderTest);


        yield return null;

        // Arrange2
        var valueUnderTest2 = 20f;
        var textUnderTest2 = "Test Loading2";

        // Act2
        loadingUnderTest.Tick(textUnderTest2, valueUnderTest2);

        // Assert2
        Assert.AreEqual(textComponentUnderTest.text, textUnderTest2);
        Assert.AreEqual(sliderComponentUnderTest.value, valueUnderTest2 + valueUnderTest);
    }

    [UnityTest]
    public IEnumerator LoadingTestScene()
    {
        // Arrange
        var sceneNameUnderTest = "TestHybird";

        // Act
        yield return GameManager.instance.LoadScene(sceneNameUnderTest);

        // Assert
        Assert.AreEqual(sceneNameUnderTest, SceneManager.GetActiveScene().name);
        LogAssert.Expect(LogType.Log, "TestHybird ExtraLoading");
        LogAssert.Expect(LogType.Log, "TestHybird Start");
    }
}
