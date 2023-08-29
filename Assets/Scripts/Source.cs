using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Source : NetworkBehaviour
{
    [SerializeField]
    int MaxHealth = 100;
    public Transform HealthBarAnchor;
    public event System.Action<int, int> OnHealthChanged;
    public event System.Action OnHealthZero;


    [Header("Debug")]
    [SyncVar]
    int Health = 100;

    public float healthPercent {
        get {
            return (float)Health / MaxHealth;
        }
    }

    BuffManager PlayerBuffManager;

    // Start is called before the first frame update
    void Start()
    {
        if(isServer) {
            Health = MaxHealth;
            PlayerBuffManager = GetComponent<BuffManager>();
        }
    }


    public void TakeDamage(int damage)
    {
        if(isServer) {
            Health -= damage + PlayerBuffManager.damageTaken;
            if(Health <= 0) {
                Health = 0;
                Destroy(gameObject);
            }
            if(Health > MaxHealth) {
                Health = MaxHealth;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
