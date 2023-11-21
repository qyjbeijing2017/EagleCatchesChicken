using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerSkill))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    public CharacterScriptableObject Character;
    public GlobalScriptableObject global
    {
        get
        {
            return NetworkController.singleton.global;
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

}
