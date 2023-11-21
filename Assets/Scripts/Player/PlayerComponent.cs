using Mirror;

public abstract class PlayerComponent : NetworkBehaviour
{
    private PlayerController m_PlayerController;
    public CharacterScriptableObject character
    {
        get
        {
            return m_PlayerController.Character;
        }
    }
    public GlobalScriptableObject global
    {
        get
        {
            return m_PlayerController.global;
        }
    }
    public NetworkController network
    {
        get
        {
            return NetworkController.singleton;
        }
    }

    public PlayerIdentity identity
    {
        get
        {
            return m_PlayerController.identity;
        }
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if(isLocalPlayer)
        m_PlayerController = GetComponent<PlayerController>();
    }
}
