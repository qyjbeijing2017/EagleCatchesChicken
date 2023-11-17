using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(MoveController))]
public class ActorController : NetworkBehaviour
{
    private static ActorController s_My;
    static public ActorController my {
        get {
            if(!s_My || !s_My.isLocalPlayer) {
                s_My = null;
                var actors = FindObjectsOfType<ActorController>();
                foreach(var actor in actors) {
                    if(actor.isLocalPlayer) {
                        s_My = actor;
                        break;
                    }
                }
            }
            return s_My;
        }
    }

    [SerializeField]
    GlobalScriptableObject m_GlobalScriptableObject;

    public GlobalScriptableObject globalScriptableObject
    {
        get
        {
            return m_GlobalScriptableObject;
        }
    }

    [SerializeField]
    CharacterScriptableObject m_CharacterScriptableObject;

    public CharacterScriptableObject characterScriptableObject
    {
        get
        {
            return m_CharacterScriptableObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDestroy()
    {
    }
}
