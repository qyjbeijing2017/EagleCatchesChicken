using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Source : NetworkBehaviour
{
    [SerializeField]
    int MaxHealth = 100;

    public Transform HealthBarAnchor;

    [HideInInspector]
    [SyncVar]
    int Health = 100;

    public event System.Action<int, int> OnHealthChanged;
    public event System.Action OnHealthZero;

    public int health { get { return Health; } }
    public int maxHealth { get { return MaxHealth; } }
    public float healthPercent { get { return (float)Health / (float)MaxHealth; } }

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            Health = MaxHealth;
        }
    }


    public void TakeDamage(int damage)
    {
        if (isServer)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                Destroy(gameObject);
            }
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
