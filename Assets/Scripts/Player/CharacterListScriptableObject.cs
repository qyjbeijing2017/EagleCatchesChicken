using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerListScriptableObject", order = 1)]
#endif
public class CharacterListScriptableObject : ScriptableObject
{
    public List<CharacterScriptableObject> MomList = new List<CharacterScriptableObject>();
    public List<CharacterScriptableObject> BabyList = new List<CharacterScriptableObject>();
    public List<CharacterScriptableObject> EagleList = new List<CharacterScriptableObject>();
}