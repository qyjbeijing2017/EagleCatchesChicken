using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Global", menuName = "ScriptableObjects/GlobalScriptableObject", order = 1)]
#endif
public class GlobalScriptableObject : ScriptableObject
{
    [Header("Network")]
    public string EntryPoint = "http://test";
    public int Port = 8080;

    public float Gravity = 9.8f;

    public float AttackRange = 50f;
    public float Drag = 0.5f;
}
