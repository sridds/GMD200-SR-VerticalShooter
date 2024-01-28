using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _spawnables;

    [SerializeField, Min(0)]
    private float _minSpawnTime = 3.0f;
    [SerializeField, Min(0)]
    private float _maxSpawnTime = 8.0f;

    [SerializeField]
    private Vector2 _randomSpawnX;

    [SerializeField]
    private int _allowedObjectsAtOnce;

    private float timer = 0.0f;
    private float nextSpawnTimestamp;

    List<GameObject> _activeSpawnables = new List<GameObject>();

    private void Start()
    {
        // set the initial spawn timestamp
        nextSpawnTimestamp = timer + Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    private void Update() => Tick();

    private void Tick()
    {
        // call spawn when timestamp reached
        if (timer > nextSpawnTimestamp) Spawn();

        // only tick timer when can spawn and can tick
        if (CanTick() && CanSpawn()) timer += Time.deltaTime;
    }

    protected virtual bool CanTick() => true;

    protected virtual bool CanSpawn()
    {
        // terminate null values
        if(_activeSpawnables.Count > 0) _activeSpawnables = _activeSpawnables.Where(x => x != null).ToList();

        if (_activeSpawnables.Count >= _allowedObjectsAtOnce) return false;

        return true;
    }

    protected virtual void Spawn()
    {
        // spawn the object
        _activeSpawnables.Add(Instantiate(_spawnables[Random.Range(0, _spawnables.Length)], new Vector2(transform.position.x + Random.Range(_randomSpawnX.x, _randomSpawnX.y), transform.position.y), Quaternion.identity));

        nextSpawnTimestamp = timer + Random.Range(_minSpawnTime, _maxSpawnTime);
    }
}
