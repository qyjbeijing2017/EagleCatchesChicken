using UnityEngine;

public enum AttackRangeType
{
    Circle,
    Sector,
    Rectangle,
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "AttackRange_", menuName = "ScriptableObjects/AttackRangeScriptableObject", order = 1)]
#endif
[XLSXLocal]
public class AttackRangeScriptableObject : ScriptableObject
{
    public AttackRangeType AttackRangeType;
    public float NearDistance;
    public float FarDistance;
    public float SectorAngle;
    public float Width;
    public float OffsetAngle;
}
