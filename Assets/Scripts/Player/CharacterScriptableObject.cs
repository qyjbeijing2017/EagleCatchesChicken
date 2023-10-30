using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerListScriptableObject", order = 1)]
public class CharacterScriptableObject : ScriptableObject
{
    public string CharacterName = "Character Name";
    public string Description = "Character Description";
    public float MoveSpeed = 6.0f;
    public float Mass = 50.0f;
    public List<float> JumpSpeeds = new List<float> { 8.0f };
}