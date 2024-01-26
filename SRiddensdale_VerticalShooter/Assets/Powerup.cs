using UnityEngine;

public abstract class Powerup : ScriptableObject
{
    [SerializeField]
    protected float _powerupDuration = 10.0f;
    protected float lifetime = 0.0f;

    public delegate void PowerupExpire();
    public PowerupExpire OnPowerupExpire;

    /// <summary>
    /// Abstract for other powerups to override it
    /// </summary>
    public virtual void Collect()
    {
        lifetime = 0.0f;

        // set powerup of player
        FindObjectOfType<Player>().SetPowerup(this);
    }
    /// <summary>
    /// This can be overridden but not recommended
    /// </summary>
    public virtual void Tick()
    {
        lifetime += Time.deltaTime;

        // expire
        if (lifetime > _powerupDuration) Expire();
    }

    /// <summary>
    /// Other classes that inherit must fill out this class
    /// </summary>
    protected virtual void Expire() => OnPowerupExpire?.Invoke();
}
