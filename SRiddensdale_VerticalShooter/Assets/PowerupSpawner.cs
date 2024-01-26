using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField]
    private CollectablePowerup[] _powerups;

    [SerializeField, Min(0)]
    private float _minSpawnTime = 3.0f;
    [SerializeField, Min(0)]
    private float _maxSpawnTime = 8.0f;

    [SerializeField]
    private Vector2 _randomSpawnX;

    private CollectablePowerup activePowerup;

    private float timer = 0.0f;
    private float nextSpawnTimestamp;

    private void Start()
    {
        nextSpawnTimestamp = timer + Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    void Update()
    {
        // call spawn when timestamp reached
        if(timer > nextSpawnTimestamp) Spawn();

        // start counting after both are null
        if(GameManager.instance.ActivePlayer.ActivePowerup == null && activePowerup == null) timer += Time.deltaTime;
    }

    // spawns a powerup
    private void Spawn()
    {
        activePowerup = Instantiate(_powerups[Random.Range(0, _powerups.Length)], new Vector2(transform.position.x + Random.Range(_randomSpawnX.x, _randomSpawnX.y), transform.position.y), Quaternion.identity);

        nextSpawnTimestamp = timer + Random.Range(_minSpawnTime, _maxSpawnTime);
    }
}
