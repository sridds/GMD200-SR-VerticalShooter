using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletPowerup", menuName = "Powerups/Bullet Powerup")]
public class BulletPowerup : Powerup
{
    public enum ModifierType
    {
        FireRate,
        IncreaseBullets
    }

    [SerializeField]
    private ModifierType _modifier;

    [ShowIf(nameof(_modifier), ModifierType.FireRate)]
    [SerializeField]
    private float _timeBetweenBursts;
    [ShowIf(nameof(_modifier), ModifierType.FireRate)]
    [SerializeField]
    private bool _multiply;

    [ShowIf(nameof(_modifier), ModifierType.IncreaseBullets)]
    [SerializeField, Min(0)]
    private int _burstCount;
    [ShowIf(nameof(_modifier), ModifierType.IncreaseBullets)]
    [SerializeField, Min(0)]
    public int _projectilesPerBurst;

    Player p;

    public override void Collect()
    {
        base.Collect();

        if(p == null) p = FindObjectOfType<Player>();

        switch (_modifier)
        {
            case ModifierType.FireRate:
                float rate = _multiply ? p.PlayerGun.timeBetweenBursts * _timeBetweenBursts : _timeBetweenBursts;
                p.PlayerGun.SetRatePowerup(rate);
                break;
            case ModifierType.IncreaseBullets:
                p.PlayerGun.SetBurstPowerup(_burstCount, _projectilesPerBurst);
                break;
        }
    }

    protected override void Expire()
    {
        switch (_modifier)
        {
            case ModifierType.FireRate:
                p.PlayerGun.ResetRatePowerup();
                break;
            case ModifierType.IncreaseBullets:
                p.PlayerGun.ResetBurstPowerup();
                break;
        }

        base.Expire();
    }
}