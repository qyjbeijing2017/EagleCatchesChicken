using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHybird : MonoBehaviour
{
    readonly public static float ExtraLoadingValue = 50f;
    public static IEnumerator ExtraLoading(LoadingBase loading)
    {
        Debug.Log("TestHybird ExtraLoading");
        var tickTime = 0;
        while (tickTime < 10)
        {
            loading.Tick("ExtraLoading...", ExtraLoadingValue / 10);
            tickTime++;
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    public static void Run()
    {
        Debug.Log("TestHybird Run");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TestHybird Start");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
