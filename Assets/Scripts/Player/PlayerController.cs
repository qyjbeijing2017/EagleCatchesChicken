using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoSingleton<PlayerController>
{
    [SerializeField]
    CharacterListScriptableObject m_CharacterList;

    public GlobalScriptableObject GlobalConfig;

    public CharacterScriptableObject GetCharacterScriptableObject(string name)
    {
        foreach (var character in m_CharacterList.MomList)
        {
            if (character.name == name)
            {
                return character;
            }
        }
        foreach (var character in m_CharacterList.BabyList)
        {
            if (character.name == name)
            {
                return character;
            }
        }
        foreach (var character in m_CharacterList.EagleList)
        {
            if (character.name == name)
            {
                return character;
            }
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PlayerController Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
