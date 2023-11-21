using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 RelativePosition;
    [SerializeField]
    private AnimationCurve FollowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
        RelativePosition = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var player = PlayerController.my;
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
