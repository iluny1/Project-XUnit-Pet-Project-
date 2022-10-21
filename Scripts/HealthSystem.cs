using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    [SerializeField] private int health_Max = 4;
    [SerializeField] private Unit currentUnit;

    private int health;
    private bool isShooting;

    public EventHandler OnDead;
    public EventHandler OnHealthChange;

    public void Awake()
    {
        health = health_Max;        
    }

    public void SetIsShooting(bool isShooting)
    {
        this.isShooting = isShooting;
    }

    public void Damage(int minDamage, int maxDamage)
    {
        health -= UnityEngine.Random.Range(minDamage, maxDamage);        

        if (health < 0)
        {
            health = 0;
        }
        if (health == 0 && isShooting != true)
        {
            Die();
        }
        OnHealthChange?.Invoke(this, EventArgs.Empty);
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public int GetHealthCount()
    {
        return health;
    }

    public int GetMaxHealthCount()
    {
        return health_Max;
    }

    public void Heal(int healCount)
    {
        health += healCount;

        if (health > health_Max)
        {
            health = health_Max;
        }

        OnHealthChange?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)health / health_Max;
    }
}
