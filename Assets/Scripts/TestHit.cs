using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHit : MonoBehaviour
{
    Hit hit;
    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        hit = GetComponent<Hit>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        hit.OnHitStart += () => {
            meshRenderer.enabled = true;
            Debug.Log("HitStart");
        };
        hit.OnHitEnd += () => {
            meshRenderer.enabled = false;
            Debug.Log("HitEnd");
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
