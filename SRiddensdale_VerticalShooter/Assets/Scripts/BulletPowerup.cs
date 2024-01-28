using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletPowerup", menuName = "Powerups/Bullet Powerup")]
public class BulletPowerup : Powerup
{
    [SerializeField]
    private ScriptableSpawner _swapSpawner;

    Player p;

    public override void Collect()
    {
        base.Collect();

        if(p == null) p = FindObjectOfType<Player>();

        p.PlayerGun.SwapoutData(_swapSpawner);
    }

    protected override void Expire()
    {
        p.PlayerGun.ResetData();

        base.Expire();
    }
}