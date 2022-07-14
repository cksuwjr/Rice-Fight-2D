using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float MaxHealth = 100f;
    [SerializeField] float NowHealth;
    // Start is called before the first frame update
    void Start()
    {
        NowHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        NowHealth -= damage;

        if(NowHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Áê±Ý!!");
    }
}
