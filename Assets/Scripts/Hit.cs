using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : Damage
{
    [Header("Hit")]
    [SerializeField]
    float Duration = 0.3f;

    public override void Exec()
    {
        base.Exec();
        StartCoroutine(HitCoroutine());
    }

    IEnumerator HitCoroutine()
    {
        yield return new WaitForSeconds(Duration);
        Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
