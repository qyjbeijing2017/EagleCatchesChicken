using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : PlayerComponent
{
    [SerializeField]
    private int m_Health;


    // Start is called before the first frame update
    void Start()
    {
        m_Health = character.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
