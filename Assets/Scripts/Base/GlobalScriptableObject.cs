using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global", menuName = "ScriptableObjects/GlobalScriptableObject", order = 1)]
public class GlobalScriptableObject : ScriptableObject
{
    [Header("Network")]
    public string EntryPoint = "http://test";
    public int Port = 8080;

    public float Gravity = 9.8f;

    public float AttackRange = 50f;
}
