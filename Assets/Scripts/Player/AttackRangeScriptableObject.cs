using UnityEngine;

public enum AttackRangeType
{
    Circle,
    Sector,
    Rectangle,
    Shoot,
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/AttackRangeScriptableObject", order = 1)]
#endif
public class AttackRangeScriptableObject : ScriptableObject
{
    public AttackRangeType AttackRangeType;
    public float NearDistance;
    public float FarDistance;
    public float Angle;
    public float Width;
    public float AngleOffset;
}
