using UnityEngine;

[CreateAssetMenu(fileName = "BulletPowerup", menuName = "Powerups/Invincible Powerup")]
public class InvinciblePowerup : Powerup
{
    private Player p;

    public override void Collect()
    {
        base.Collect();

        if (p == null) p = GameManager.instance.ActivePlayer;

        p.PlayerHealth.CallIFrames((int)(_powerupDuration / 0.1f), 0.05f);
    }

    protected override void Expire()
    {
        base.Expire();
    }
}
