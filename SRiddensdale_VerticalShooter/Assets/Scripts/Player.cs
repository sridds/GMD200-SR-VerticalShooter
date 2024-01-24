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

    public Health PlayerHealth { get { return _playerHealth; } }

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
        if (Input.GetKey(KeyCode.Z)) CallFire();
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

    private void SetTimeScaleBack() => GameManager.instance.SetTimeScale(1f, 0.5f);

    private void DashIFrames() => _playerHealth.CallIFrames(15, _playerMovement.DashTime / (float)15);
}
