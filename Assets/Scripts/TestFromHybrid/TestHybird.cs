using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHybird : MonoBehaviour
{
    readonly public static float ExtraLoadingValue = 50f;
    public static IEnumerator ExtraLoading()
    {
        Debug.Log("TestHybird ExtraLoading");
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
