using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Threading;
using Mirror;
using UnityEngine;

public class SkillController : NetworkBehaviour
{
    Animator m_Animator;

    bool m_SkillRunning = false;

    public bool skillRunning
    {
        get
        {
            return m_SkillRunning;
        }
    }

    PlayerInputAction InputActions;

    MoveController m_MoveController;

    CharacterController m_CharacterController;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;
        m_Animator = GetComponentInChildren<Animator>();
        m_MoveController = GetComponent<MoveController>();
        m_CharacterController = GetComponent<CharacterController>();
        InputActions = new PlayerInputAction();
        InputActions.Skill.Enable();
        var actions = InputActions.Skill.Get().actions;
        for (int i = 0; i < actions.Count; i++)
        {
            var index = i;
            actions[i].performed += (context) => SkillStart(index);
        }
    }

    void Update()
    {
        var speed = new Vector2(m_MoveController.moveVelocity.x, m_MoveController.moveVelocity.z).magnitude;
        m_Animator.SetFloat("Speed", speed);
    }

    [SerializeField]
    private AnimationCurve DashDistance;

    [SerializeField]
    private float BulletSpeed = 20f;
    [SerializeField]
    private float BulletRadius = 0.5f;
    [SerializeField]
    private float BulletLifeTime = 1f;

    public void OnShoot() {
        StartCoroutine(HandleShoot());
    }

    IEnumerator HandleShoot()
    {
        var startTime = Time.time;
        var startPos = transform.position;
        var dir = transform.forward;
        var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.localScale = Vector3.one * BulletRadius;
        while (m_SkillRunning)
        {
            var timePast = Time.time - startTime;
            var distance = BulletSpeed * timePast;
            var move = dir * distance;
            var target = startPos + move;
            bullet.transform.position = target;
            yield return null;
        }
        Destroy(bullet);
    }

    void SkillStart(int skillNo)
    {
        if(skillNo >= 2) return;
        if (m_SkillRunning) return;
        m_Animator.SetTrigger("Skill" + (skillNo + 1));
        m_SkillRunning = true;
        switch (skillNo)
        {
            case 1:
                StartCoroutine(DashHandler());
                break;
            default:
                break;
        }
    }

    IEnumerator DashHandler()
    {
        var startTime = Time.time;
        var startPos = transform.position;
        var dir = transform.forward;
        while (m_SkillRunning)
        {
            var currentPos = transform.position;
            var timePast = Time.time - startTime;
            var distance = DashDistance.Evaluate(timePast);
            var move = dir * distance;
            var target = startPos + move;
            var moveTo = target - currentPos;
            moveTo.y = 0;
            m_CharacterController.Move(moveTo);
            yield return null;
        }
    }

    public void OnSkillEnd()
    {
        m_SkillRunning = false;
    }

    public void OnSkillAction(string magic)
    {
        Debug.Log(magic);
    }
}
