using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Spawner _playerGun;

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
}
