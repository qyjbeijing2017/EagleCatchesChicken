using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerSkill : PlayerComponent
{
    [SerializeField]
    public readonly SyncList<bool> RunningSkills = new SyncList<bool>();
    public readonly SyncList<float> SkillCoolDowns = new SyncList<float>();
    public readonly SyncList<bool> SkillPreparing = new SyncList<bool>();

    public bool IsReady(int index)
    {
        if (SkillCoolDowns[index] <= 0 && !RunningSkills[index])
        {
            return true;
        }
        return false;
    }

    public bool isAnySkillRunning => RunningSkills.Contains(true);

    public bool isForceMoving
    {
        get
        {
            for (int i = 0; i < playerConfig.Skills.Count; i++)
            {
                var skill = playerConfig.Skills[i];
                if (skill.ForceMove && RunningSkills[i])
                {
                    return true;
                }
            }
            return isAnySkillRunning || isKnockedBack;
        }
    }


    PlayerInputAction m_InputActions;

    public event Action<int> OnSkill;

    private PlayerHealth m_PlayerHealth;
    private PlayerMove m_PlayerMove;

    [SyncVar]
    [SerializeField]
    private float m_IsKnockedBackTime;
    public bool isKnockedBack => m_IsKnockedBackTime > 0;
    void Start()
    {
        if (isLocalPlayer)
        {
            m_InputActions = new PlayerInputAction();
            m_InputActions.Skill.Enable();
            var actions = m_InputActions.Skill.Get().actions;
            for (int i = 0; i < actions.Count; i++)
            {
                var index = i;
                actions[i].performed += (context) =>
                {
                    if (IsReady(index))
                    {
                        SkillExec(index);
                    }
                };
            }
        }
        if (isServer)
        {
            for (int i = 0; i < playerConfig.Skills.Count; i++)
            {
                RunningSkills.Add(false);
                SkillCoolDowns.Add(0);
                SkillPreparing.Add(false);
            }

        }
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_PlayerMove = GetComponent<PlayerMove>();
    }

    void SkillExec(int skillNo) {
        if (!IsReady(skillNo)) return;
        OnSkill?.Invoke(skillNo);
        StartCoroutine(SkillForceMoving(skillNo));
        CmdSkillExec(skillNo);
    }

    [Command]
    void CmdSkillExec(int skillNo)
    {
        StartCoroutine(SkillRunning(skillNo));
    }

    [Server]
    IEnumerator SkillRunning(int skillNo)
    {
        RunningSkills[skillNo] = true;
        var skill = playerConfig.Skills[skillNo];
        var attackEvents = skill.AttackEvents;
        attackEvents.Sort((a, b) => a.time.CompareTo(b.time));
        var lastTime = 0.0f;
        for (var i = 0; i < attackEvents.Count; ++i)
        {
            var attackEvent = attackEvents[i];
            yield return new WaitForSeconds(attackEvent.time - lastTime);
            var attack = attackEvent.attack;
            yield return StartCoroutine(AttackHandler(attack));
            lastTime = attackEvent.time;
        }

        yield return new WaitForSeconds(skill.Duration - lastTime);
        RunningSkills[skillNo] = false;
        SkillCoolDowns[skillNo] = skill.CoolDown;
    }

    private bool isAutoSkillRunning
    {
        get
        {
            for (int i = 0; i < playerConfig.Skills.Count; i++)
            {
                if(RunningSkills[i] && playerConfig.Skills[i].AutoAttack) {
                    Debug.Log("auto Attack");
                    return true;
                }
            }
            return false;
        }
    }

    [ClientRpc]
    void RpcSkillExec(int skillNo)
    {
        StartCoroutine(SkillRunning(skillNo));
    }

    IEnumerator SkillForceMoving(int skillNo)
    {
        var skill = playerConfig.Skills[skillNo];
        if (!skill.ForceMove) yield break;

        var time = Time.time;
        while(!RunningSkills[skillNo])
        {
            yield return null;
            if(Time.time - time > 1f) {
                yield break;
            }
        }


        var currentTime = 0.0f;
        while (RunningSkills[skillNo])
        {
            if (isKnockedBack || isAutoSkillRunning) {

                yield return null;
                continue;
            }
            var rotateY = skill.DashRotationY.Evaluate(currentTime);
            var speed = skill.DashSpeed.Evaluate(currentTime);
            transform.rotation *= Quaternion.AngleAxis(rotateY, Vector3.up);
            m_PlayerMove.AddMovePosition(transform.forward * speed * Time.deltaTime);
            yield return null;
            currentTime += Time.deltaTime;
        }

    }

    void Update()
    {
        if (!isServer) return;
        if (isKnockedBack)
        {
            Debug.Log("knockback");
            m_IsKnockedBackTime -= Time.deltaTime;
        }

        for (int i = 0; i < playerConfig.Skills.Count; i++)
        {
            var skill = playerConfig.Skills[i];

            if(SkillCoolDowns[i] > 0) {
                SkillCoolDowns[i] -= Time.deltaTime;
            }

            if (skill.AutoAttack && IsReady(i) || RunningSkills[i])
            {
                SkillPreparing[i] = SomeOneInRange(skill.AutoPrepareRange, skill.AutoTargetLayer).Count > 0;
                if (SkillPreparing[i])
                {
                    if (SomeOneInRange(skill.AutoAttackRange, skill.AutoTargetLayer).Count > 0)
                        SkillExec(i);
                }
            }
            else
            {
                SkillPreparing[i] = false;
            }
        }
    }

    List<PlayerHealth> SomeOneInRange(AttackRangeScriptableObject range, LayerMask mask)
    {
        if (range == null) return new List<PlayerHealth>();
        var result = new List<PlayerHealth>();
        var dir = Quaternion.AngleAxis(range.OffsetAngle, Vector3.up) * transform.forward;
        switch (range.AttackRangeType)
        {
            case AttackRangeType.Circle:
                var circleColliders = Physics.OverlapSphere(transform.position, range.FarDistance, mask);
                foreach (var collider in circleColliders)
                {
                    var playerHealth = collider.GetComponent<PlayerHealth>();
                    if ((collider.transform.position - transform.position).magnitude < range.NearDistance) continue;
                    if (playerHealth != null)
                    {
                        result.Add(playerHealth);
                    }
                }
                break;
            case AttackRangeType.Sector:
                var sectorColliders = Physics.OverlapSphere(transform.position, range.FarDistance, mask);
                foreach (var collider in sectorColliders)
                {
                    var playerHealth = collider.GetComponent<PlayerHealth>();
                    var vectorToTarget = collider.transform.position - transform.position;
                    var angle = Vector3.Angle(dir, vectorToTarget);
                    if (angle > range.SectorAngle / 2) continue;
                    if (playerHealth != null)
                    {
                        result.Add(playerHealth);
                    }
                }
                break;
            case AttackRangeType.Rectangle:
                var RectangleColliders = Physics.OverlapBox(
                    transform.position + dir * (range.NearDistance + range.FarDistance) / 2,
                    new Vector3(range.Width, 1, range.FarDistance - range.NearDistance),
                    Quaternion.LookRotation(dir),
                    mask
                );
                foreach (var collider in RectangleColliders)
                {
                    var playerHealth = collider.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        result.Add(playerHealth);
                    }
                }
                break;
        }
        return result;
    }

    // magic is name:damageNo, like Fire:0
    [Server]
    void OnAttack(string magic)
    {
        var magicSplit = magic.Split(':');
        var skillNo = int.Parse(magicSplit[0]);
        var attackNo = int.Parse(magicSplit[1]);
        var skill = playerConfig.Skills[skillNo];
        var attack = skill.AttackEvents[attackNo].attack;
        StartCoroutine(AttackHandler(attack));

    }

    IEnumerator AttackHandler(AttackScriptableObject attack)
    {
        if (attack.ForMyself)
        {
            m_PlayerHealth.BeAttacked(attack, player);
        }
        else
        {
            var targets = SomeOneInRange(attack.AttackRange, attack.TargetLayer);
            foreach (var target in targets)
            {
                target.BeAttacked(attack, player);
            }
            if (targets.Count > 0)
            {
                m_IsKnockedBackTime = attack.KnockbackDuration;
                yield return new WaitForSeconds(attack.KnockbackDuration);
            }
        }

        if (attack.ShootBullet != null)
        {
            var bulletConfig = attack.ShootBullet.GetComponent<Bullet>().BulletConfig;
            var rotation = Quaternion.AngleAxis(bulletConfig.OffsetAngle, Vector3.up) * transform.rotation;
            var position = transform.position + rotation * bulletConfig.OffsetPosition;
            var bulletObj = Instantiate(attack.ShootBullet, position, rotation);
            NetworkServer.Spawn(bulletObj);
            bulletObj.GetComponent<Bullet>().Owner = player;
        }
    }
}
