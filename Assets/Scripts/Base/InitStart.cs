using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.StartCoroutine(Init());
    }

    IEnumerator Init() {
        yield return GameManager.instance.LoadAllMetadataForAOTAssembly();
        yield return GameManager.instance.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
