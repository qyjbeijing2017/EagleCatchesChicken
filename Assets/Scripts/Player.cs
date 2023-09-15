using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Experimental;
using System.IO.Enumeration;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkRigidbody))]
[RequireComponent(typeof(Move))]
[RequireComponent(typeof(Source))]
[RequireComponent(typeof(BuffManager))]
[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(AnimatorManager))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SkillManager))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    public int PlayerId;
    // Start is called before the first frame update
    void Start()
    {
        if(isClientOnly) {
            ECCNetworkManager.instance.PlayerList.Add(this);
            ECCNetworkManager.instance.PlayerList.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Reset()
    {
        var player = this;
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        player.transform.localScale = Vector3.one;

        var networkTransform = player.GetComponent<NetworkTransform>();
        networkTransform.syncDirection = SyncDirection.ClientToServer;
        var networkAnimator = player.GetComponent<NetworkAnimator>();
        networkAnimator.clientAuthority = true;
        var animator = player.GetComponent<Animator>();
        networkAnimator.animator = animator;

        var networkRigidbody = player.GetComponent<NetworkRigidbody>();

        var rigdibody = player.GetComponent<Rigidbody>();
        rigdibody.freezeRotation = true;

        var source = player.GetComponent<Source>();
        if (source.HealthBarAnchor == null)
        {
            var healthBarAnchor = player.transform.Find("HealthBarAnchor");
            if (healthBarAnchor == null)
            {
                healthBarAnchor = new GameObject("HealthBarAnchor").transform;
                healthBarAnchor.transform.SetParent(player.transform);
                healthBarAnchor.transform.localPosition = new Vector3(0f, 2f, 0f);
                healthBarAnchor.transform.localRotation = Quaternion.identity;
                healthBarAnchor.transform.localScale = Vector3.one;
            }
            source.HealthBarAnchor = healthBarAnchor;
        }

        var jumpManager = player.GetComponentInChildren<JumpManager>();
        if (jumpManager == null)
        {
            var jumpManagerObject = new GameObject("JumpManager");
            jumpManagerObject.transform.SetParent(player.transform);
            jumpManagerObject.transform.localPosition = Vector3.zero;
            jumpManagerObject.transform.localRotation = Quaternion.identity;
            jumpManagerObject.transform.localScale = Vector3.one;
            jumpManager = jumpManagerObject.AddComponent<JumpManager>();

            var jumpColider = jumpManagerObject.GetComponent<SphereCollider>();
            jumpColider.center = new Vector3(0f, 0.0f, 0f);
            jumpColider.radius = 0.3f;
        }

        var skillManager = player.GetComponent<SkillManager>();        
        var skillsNode =player.transform.Find("Skills");
        if(skillsNode != null)
        {
            DestroyImmediate(skillsNode.gameObject);
        }

        var skills = new GameObject("Skills");
        skills.transform.SetParent(player.transform);
        skills.transform.localPosition = Vector3.zero;
        skills.transform.localRotation = Quaternion.identity;
        skills.transform.localScale = Vector3.one;

        var attackObject = new GameObject("Attack");
        attackObject.transform.SetParent(skills.transform);
        attackObject.transform.localPosition = Vector3.zero;
        attackObject.transform.localRotation = Quaternion.identity;
        attackObject.transform.localScale = Vector3.one;
        var attack = attackObject.AddComponent<Attack>();
        skillManager.Attack = attack;

        var damageTypeNames = System.Enum.GetNames(typeof(DamageType));
        var InputActions = new PlayerInputAction();
        var actions = InputActions.Skill.Get().actions;
        skillManager.Skills = new List<SkillIdentity>(actions.Count * damageTypeNames.Length);
        foreach (string damageTypeName in damageTypeNames)
        {
            var typeSkillsObject = new GameObject(damageTypeName);
            typeSkillsObject.transform.SetParent(skills.transform);
            typeSkillsObject.transform.localPosition = Vector3.zero;
            typeSkillsObject.transform.localRotation = Quaternion.identity;
            typeSkillsObject.transform.localScale = Vector3.one;

            foreach (var action in actions)
            {
                var skillObject = new GameObject(action.name);
                skillObject.transform.SetParent(typeSkillsObject.transform);
                skillObject.transform.localPosition = Vector3.zero;
                skillObject.transform.localRotation = Quaternion.identity;
                skillObject.transform.localScale = Vector3.one;

                var skill = skillObject.AddComponent<Skill>();
                skillManager.Skills.Add(new SkillIdentity()
                {
                    name = $"{damageTypeName}\t{action.name}",
                    id = $"{action.name}",
                    skill = skill,
                });
            }
        }
    }
}
