using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GlobalScriptableObject", order = 1)]
public class GlobalScriptableObject : ScriptableObject
{
    [Header("Network")]
    public string EntryPoint = "http://test";
    public int Port = 8080;
}
