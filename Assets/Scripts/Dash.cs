using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Dash : Damage
{
    [Header("Dash")]
    [SerializeField]
    private AnimationCurve Distance;

    [SerializeField]
    private float MaxTime = 1f;

    [Server]
    public override void Exec(Skill skill)
    {
        base.Exec(skill);
        StartCoroutine(DashCoroutine());
        skill.OnStopped += Stop;
    }


    override public void Stop()
    {
        CurrentSkill.OnStopped -= Stop;
        base.Stop();
        StopAllCoroutines();
    }

    [Server]
    IEnumerator DashCoroutine()
    {
        var playerTransform = Murderer.transform;
        var startPos = playerTransform.position;
        var dir = transform.forward;
        var startTime = Time.time;
        var timePast = Time.time - startTime;
        while (timePast < MaxTime)
        {
            Debug.Log("handle Dash");
            yield return null;
            timePast = Time.time - startTime;
            Debug.Log(timePast);
            playerTransform.position = startPos + dir * Distance.Evaluate(timePast);
        }
    }

    protected override void Awake()
    {
    }
}
