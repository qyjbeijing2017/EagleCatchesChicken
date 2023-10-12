using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace MainMenu;

public class MainMenu
{
    static IEnumerator Loading(){
        int i = 0;
        while(i < 100){
            i++;
            Debug.Log("Loading... " + i + "%");
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Loading Complete!");
    }

    public static void Init()
    {
        GameManager.instance.StartCoroutine(Loading());
    }
}
