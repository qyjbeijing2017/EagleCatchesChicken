using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    private Vector3 RelativePosition;
    [SerializeField]
    private AnimationCurve FollowSpeed;

    [Header("Debug")]
    [SerializeField]
    private Player MyPlayer;

    // Start is called before the first frame update
    void Start()
    {
        RelativePosition = transform.position;
    }

    Player player
    {
        get
        {
            return ECCNetworkManager.instance.PlayerList.Find(x => x.isLocalPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var player = this.player;
        MyPlayer = player;
        if (player)
        {
            var targetPosition = player.transform.position + RelativePosition;
            var distance = Vector3.Distance(transform.position, targetPosition);
            var speed = FollowSpeed.Evaluate(distance);
            var moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }
}
