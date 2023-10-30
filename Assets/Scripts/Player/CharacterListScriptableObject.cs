using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerListScriptableObject", order = 1)]
public class CharacterListScriptableObject : ScriptableObject
{
    public List<CharacterScriptableObject> MomList = new List<CharacterScriptableObject>();
    public List<CharacterScriptableObject> BabyList = new List<CharacterScriptableObject>();
    public List<CharacterScriptableObject> EagleList = new List<CharacterScriptableObject>();
}