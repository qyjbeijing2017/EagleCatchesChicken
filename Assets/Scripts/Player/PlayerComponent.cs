using Mirror;

public abstract class PlayerComponent : NetworkBehaviour
{
    private PlayerController m_PlayerController;
    public PlayerController player => m_PlayerController;
    public CharacterScriptableObject playerConfig
    {
        get
        {
            return m_PlayerController.PlayerConfig;
        }
    }
    public GlobalScriptableObject globalConfig
    {
        get
        {
            return m_PlayerController.globalConfig;
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
        m_PlayerController = GetComponent<PlayerController>();
    }
}
