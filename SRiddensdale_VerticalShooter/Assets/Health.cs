using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable, IHealable
{
    [Header("Preferences")]
    [SerializeField, Min(1)]
    private int _maxHealth;

    private int health;

    // events
    public delegate void HealthUpdate(int oldHealth, int newHealth);
    public HealthUpdate OnHealthUpdate;

    private void Start()
    {
        health = _maxHealth;
    }

    /// <summary>
    /// Takes damage and calls an event
    /// </summary>
    /// <param name="damageAmount"></param>
    public void TakeDamage(int damageAmount)
    {
        // get the old and new health
        int oldHealth = health;

        health -= damageAmount;
        int newHealth = health;

        // call events
        OnHealthUpdate?.Invoke(oldHealth, newHealth);
    }

    /// <summary>
    /// Takes health and calls an update event
    /// </summary>
    /// <param name="health"></param>
    public void Heal(int healAmount)
    {
        // get the old and new health
        int oldHealth = health;

        health += health;
        int newHealth = health;

        // call events
        OnHealthUpdate?.Invoke(oldHealth, newHealth);
    }
}


public interface IDamagable
{
    void TakeDamage(int damageAmount);
}

public interface IHealable
{
    void Heal(int healAmount);
}
