using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "CharacterList", menuName = "ScriptableObjects/PlayerListScriptableObject", order = 1)]
#endif
public class CharacterListScriptableObject : ScriptableObject
{
    public List<CharacterScriptableObject> Characters = new List<CharacterScriptableObject>();
}