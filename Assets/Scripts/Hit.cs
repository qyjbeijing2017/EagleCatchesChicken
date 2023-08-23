using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hit : Damage
{
    [Header("Hit")]
    [SerializeField]
    float Duration = 0.3f;

    public event Action OnHitStart;
    public event Action OnHitEnd;

    public override void Exec()
    {
        base.Exec();
        if (OnHitStart != null)
            OnHitStart();
        StartCoroutine(HitCoroutine());
    }

    IEnumerator HitCoroutine()
    {
        yield return null;
        yield return new WaitForSeconds(Duration);
        if (OnHitEnd != null)
            OnHitEnd();
        Stop();
    }
}
