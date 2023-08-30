using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Skill
{
    [Header("Attack")]
    [SerializeField]
    LayerMask Target;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        if(isServer) {
            PlayerId = GetComponentInParent<Player>().PlayerId;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
