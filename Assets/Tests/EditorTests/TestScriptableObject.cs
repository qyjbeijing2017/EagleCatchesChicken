using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptableObject : ScriptableObject
{
    public int intValue = 888;
    public string stringValue = "default value";
    public float floatValue = 1.5f;
    public bool boolValue = true;
    public List<int> intList = new List<int>() { 1, 2, 3 };
    public List<string> stringList = new List<string>() { "string1", "string2", "string3" };
    public List<float> floatList = new List<float>() { 1.1f, 2.2f, 3.3f };
    public List<bool> boolList = new List<bool>() { true, false, true };

}
