using System.Collections.Generic;
using UnityEngine;

public enum PlayerIdentity
{
    Eagle = 7,
    Mom = 8,
    Baby = 9,
}

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerListScriptableObject", order = 1)]
[XLSXLocal]
public class CharacterScriptableObject : ScriptableObject
{
    public string CharacterName = "Character Name";
    public string Description = "Character Description";

    public PlayerIdentity Identity = PlayerIdentity.Eagle;
    public int MaxHealth = 100;
    public float MoveSpeed = 6.0f;
    public float Mass = 50.0f;
    public List<float> JumpSpeeds = new List<float> { 8.0f };
}
