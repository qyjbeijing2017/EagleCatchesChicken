using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Item : Skill
{
    [Header("Item")]
    [SerializeField]
    private bool EnableItemOnStart = false;

    [Server]
    void EnableItem()
    {
        Exec(PlayerId);
    }

    [Server]
    void DestroyItem()
    {
        NetworkServer.Destroy(gameObject);
    }

    override public void Stop()
    {
        var playerID = PlayerId;
        base.Stop();
        PlayerId = playerID;
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        if (isServer)
        {
            PlayerId = GetComponentInParent<Player>().PlayerId;
            if (EnableItemOnStart)
            {
                EnableItem();
            }
        }

    }
}
