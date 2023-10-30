using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    CharacterListScriptableObject m_CharacterList;

    ActorController m_ActorController;

    public ActorController myActor
    {
        get
        {
            return m_ActorController;
        }
    }

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

    public IEnumerator LoadPlayer(string name, LoadingBase loading = null)
    {
        Debug.Log($"LoadPlayer {name}");
        if(m_ActorController != null)
            Destroy(m_ActorController.gameObject);
        if(loading != null)
            loading.maxValue += 100;
        var lastPercent = 0.0f;
        var handler = Addressables.LoadAsset<GameObject>($"Assets/Prefabs/Characters/{name}.prefab");
        while (!handler.IsDone)
        {
            var percent = handler.PercentComplete;
            if(loading != null)
                loading.Tick($"Loading {name}... {percent * 100}%", (int)((percent - lastPercent) * 100));
            lastPercent = percent;
            yield return null;
        }

        var prefab = handler.Result;
        m_ActorController = Instantiate(prefab).GetComponent<ActorController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PlayerController Start");
        StartCoroutine(LoadPlayer("BlackBoss"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
