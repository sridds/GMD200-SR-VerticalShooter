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

    [Header("VFX")]
    [SerializeField]
    private int _lowHealth;

    [SerializeField]
    private ParticleSystem _playerLowHealthParticles;

    [Header("Death Animation")]
    [SerializeField]
    private GameObject _spritesHolder;

    [SerializeField]
    private ParticleSystem _playerDeathParticle;

    [Header("SFX")]
    [SerializeField]
    private AudioData _powerupExpireSound;

    [SerializeField]
    private AudioData _deathLaugh;

    public Health PlayerHealth { get { return _playerHealth; } }
    public Powerup ActivePowerup { get; private set; }
    public Spawner PlayerGun { get { return _playerGun; } }

    private void Start()
    {
        _playerMovement.OnDashStart += DashIFrames;
        _playerHealth.OnHealthUpdate += HealthUpdate;
        _playerHealth.OnHealthDepleted += Dead;
    }

    private void OnDisable()
    {
        _playerMovement.OnDashStart -= DashIFrames;
        _playerHealth.OnHealthUpdate -= HealthUpdate;
        _playerHealth.OnHealthDepleted -= Dead;
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
        if (ActivePowerup != null) ActivePowerup.Tick();
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
            AudioHandler.instance.FadeToPitch(0.15f, 0.8f, true);

            Invoke(nameof(SetTimeScaleBack), 0.25f);
        }

        // play low health particles
        if (newHealth < _lowHealth) _playerLowHealthParticles.Play();
        else _playerLowHealthParticles.Stop();
    }

    private void Dead() => StartCoroutine(GameOver());

    private IEnumerator GameOver()
    {
        GameManager.instance.CallGameOver();
        _spritesHolder.SetActive(false);
        _playerDeathParticle.Play();

        CameraShake.instance.Shake(1.3f, 1f);
        GameManager.instance.SetTimeScale(0f, 5f);
        AudioHandler.instance.FadeToPitch(5f, 0f, true);

        yield return new WaitForSecondsRealtime(1.5f);
        AudioHandler.instance.ProcessAudioData(_deathLaugh);
        yield return new WaitForSecondsRealtime(5);
        GameManager.instance.RestartLevel();
    }

    /// <summary>
    /// Sets the current active powerup of the player
    /// </summary>
    /// <param name="powerup"></param>
    public void SetPowerup(Powerup powerup)
    {
        ActivePowerup = powerup;

        // subscribe to powerup events
        powerup.OnPowerupExpire += PowerupExpire;
    }

    /// <summary>
    /// This is called when the current active powerup expires
    /// </summary>
    private void PowerupExpire()
    {
        ActivePowerup.OnPowerupExpire -= PowerupExpire;
        AudioHandler.instance.ProcessAudioData(_powerupExpireSound);

        ActivePowerup = null;
    }

    private void SetTimeScaleBack()
    {
        GameManager.instance.SetTimeScale(1f, 0.5f);
        AudioHandler.instance.FadeToPitch(0.5f, 1f);
        //AudioHandler.instance
    }

    private void DashIFrames() => _playerHealth.CallIFrames(15, _playerMovement.DashTime / (float)15);
}
