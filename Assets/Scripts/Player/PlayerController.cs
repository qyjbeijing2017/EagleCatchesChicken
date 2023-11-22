using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerSkill))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerBuff))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    public CharacterScriptableObject PlayerConfig;

    public GlobalScriptableObject globalConfig
    {
        get
        {
            return NetworkController.singleton.GlobalConfig;
        }
    }

    public static PlayerController my
    {
        get
        {
            var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                if (player.isLocalPlayer)
                {
                    return player;
                }
            }
            return null;
        }
    }

    [SyncVar]
    PlayerIdentity m_Identity;
    public PlayerIdentity identity
    {
        get
        {
            return m_Identity;
        }
        set
        {
            gameObject.layer = (int)value;
            m_Identity = value;
        }
    }

    void Start()
    {
        identity = PlayerConfig.Identity;
    }



}
