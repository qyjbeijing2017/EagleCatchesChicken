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

    private HashSet<Source> HasHit = new HashSet<Source>();

    void OnTriggerEnter(Collider other)
    {
        var source = other.GetComponent<Source>();
        if (source)
        {
            if (HasHit.Contains(source))
                return;
            HasHit.Add(source);
        }
        base.OnTriggerEnter(other);
    }

    public override void Exec(Skill skill)
    {
        HasHit.Clear();
        base.Exec(skill);
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
