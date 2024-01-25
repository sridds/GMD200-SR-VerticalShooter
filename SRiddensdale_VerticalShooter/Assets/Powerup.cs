using UnityEngine;

public abstract class Powerup : ScriptableObject
{
    [SerializeField]
    internal float _powerupDuration = 10.0f;

    internal bool powerupActive;
    internal float lifetime;

    public delegate void PowerupExpire();
    public PowerupExpire OnPowerupExpire;

    /// <summary>
    /// Abstract for other powerups to override it
    /// </summary>
    internal virtual void Collect()
    {
        powerupActive = true;

        // set powerup of player
        FindObjectOfType<Player>().SetPowerup(this);
    }
    /// <summary>
    /// This can be overridden but not recommended
    /// </summary>
    public virtual void Tick()
    {
        if (!powerupActive) return;

        lifetime += Time.deltaTime;
        if (lifetime > _powerupDuration) Expire();
    }

    /// <summary>
    /// Other classes that inherit must fill out this class
    /// </summary>
    internal virtual void Expire() => OnPowerupExpire?.Invoke();
}
