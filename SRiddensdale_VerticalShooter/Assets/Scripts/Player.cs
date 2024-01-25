using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Spawner _playerGun;

    [SerializeField]
    private PlayerMovement _playerMovement;

    [SerializeField]
    private Health _playerHealth;

    [SerializeField]
    private Supercharge _playerSupercharge;

    public Health PlayerHealth { get { return _playerHealth; } }
    public Powerup _activePowerup { get; private set; }

    private void Start()
    {
        _playerMovement.OnDashStart += DashIFrames;
        _playerHealth.OnHealthUpdate += HealthUpdate;
    }

    private void OnDisable()
    {
        _playerMovement.OnDashStart -= DashIFrames;
        _playerHealth.OnHealthUpdate -= HealthUpdate;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z) && CanFire()) CallFire();

        // This is getting messy and I'm not proud of it, but hey, it works.
        if (_playerSupercharge.IsReleasingCharge || _playerSupercharge.IsChargingUp) {
            _playerMovement.OverrideDash = true;
            _playerMovement.ChangePlayerSpeed(new Vector2(5, 5));
        }
        else {
            _playerMovement.OverrideDash = false;
            _playerMovement.ResetPlayerSpeed();
        }

        // tick the active powerup
        if (_activePowerup != null) _activePowerup.Tick();
    }

    private bool CanFire()
    {
        if (_playerSupercharge.IsChargingUp) return false;
        if (GameManager.instance.CurrentGameState == GameManager.GameState.Paused) return false;
        if (_playerSupercharge.IsReleasingCharge) return false;

        return true;
    }

    private void CallFire() => _playerGun.Fire();


    private void HealthUpdate(int oldHealth, int newHealth)
    {
        if (oldHealth > newHealth)
        {
            // damaged
            CameraShake.instance.Shake(0.6f, 0.4f);
            GameManager.instance.SetTimeScale(0.25f, 0.25f);

            Invoke(nameof(SetTimeScaleBack), 0.25f);
        }
    }

    /// <summary>
    /// Sets the current active powerup of the player
    /// </summary>
    /// <param name="powerup"></param>
    public void SetPowerup(Powerup powerup)
    {
        _activePowerup = powerup;

        // subscribe to powerup events
        powerup.OnPowerupExpire += PowerupExpire;
    }

    /// <summary>
    /// This is called when the current active powerup expires
    /// </summary>
    private void PowerupExpire()
    {
        _activePowerup.OnPowerupExpire -= PowerupExpire;
        _activePowerup = null;
    }

    private void SetTimeScaleBack() => GameManager.instance.SetTimeScale(1f, 0.5f);

    private void DashIFrames() => _playerHealth.CallIFrames(15, _playerMovement.DashTime / (float)15);
}
