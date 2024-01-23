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

    private void Start()
    {
        _playerMovement.OnDashStart += DashIFrames;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Z)) {
            CallFire();
        }
    }

    private void CallFire()
    {
        _playerGun.Fire();
    }

    private void DashIFrames() => _playerHealth.CallIFrames(15, _playerMovement.DashTime / (float)15);
}
