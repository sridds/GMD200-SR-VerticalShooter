using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// WAVE SPAWNER BEHAVIOUR
/// - Spawns enemies based on rarity
/// - If an enemy is exclusive only past a defined wave, the enemy will spawn at least once during the wave defined regardless of rarity
/// - More enemies spawn over time based on a curve value and the min/max spawn amount
/// - Once all enemies are killed, a new wave starts
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveEnemy
    {
        [MinMaxSlider(1, 20)]
        public Vector2 rarity;

        public GameObject enemyPrefab;

        [Min(1)]
        public int spawnsPastWave;

        public bool hasSpawnLimit;

        /*[AllowNesting]
        [ShowIf(nameof(hasSpawnLimit))]
        public int spawnLimit;*/

        [HideInInspector]
        public GameObject activeEnemy;
    }

    [Header("Difficulty")]
    [SerializeField, Tooltip("A curve that effects how many enemies are spawned over each wave, the length of the curv being defined by maxWaveEvaluation")]
    private AnimationCurve _enemiesOverWaves;
    [SerializeField, Tooltip("Defines the amount of waves the curve evaluates over")]
    private int _maxWaveEvaluation = 100;
    [SerializeField]
    private int _minSpawnCount = 3;
    [SerializeField]
    private int _maxSpawnCount = 10;

    [Header("Spawn Conditions")]
    [SerializeField]
    private WaveEnemy[] _waveEnemies;
    [SerializeField]
    private Vector2 _minMaxSpawnX = new Vector2(-6, 6);

    [Header("Timers")]
    [SerializeField]
    private Vector2 _randomSpawnInterval = new Vector2(1.0f, 4.0f);
    [SerializeField]
    private float _waveDowntime = 4.0f;

    [Header("SFX")]
    [SerializeField]
    private AudioData _waveBeginSound;

    [SerializeField]
    private WaveIndicator _waveIndicator;

    private int currentWave = 0;
    private bool spawning = false;
    private int enemiesCount;

    private Coroutine activeWaveCoroutine = null;
    private List<WaveEnemy> enemiesActive = new List<WaveEnemy>();

    void Start()
    {
        enemiesCount = _minSpawnCount;
        activeWaveCoroutine = StartCoroutine(HandleWave());
    }

    private void Update()
    {
        if (activeWaveCoroutine == null)
        {
            // terminate null values
            if (enemiesActive.Count > 0) enemiesActive = enemiesActive.Where(x => x.activeEnemy != null).ToList();

            if(enemiesActive.Count == 0) {
                activeWaveCoroutine = StartCoroutine(HandleWave());
            }
        }
    }

    private IEnumerator HandleWave()
    {
        // wait before spawning enemies
        yield return new WaitForSeconds(_waveDowntime);

        currentWave++;

        // show text for the current wave
        _waveIndicator.CallWaveIndicator(currentWave);
        AudioHandler.instance.ProcessAudioData(_waveBeginSound);

        // wait a brief moment before spawning just to let the player settle
        spawning = true;

        // get the approximate enemy count based on curve
        UpdateEnemyCount();
        Debug.Log("Enemies this wave: " + enemiesCount);

        // spawn enemies
        for(int i = 0; i < enemiesCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(_randomSpawnInterval.x, _randomSpawnInterval.y));

            // get and spawn the enemy
            WaveEnemy enemy = GetEnemyToSpawn(i);
            Spawn(enemy);
        }


        yield return new WaitForSeconds(0.5f);

        activeWaveCoroutine = null;
    }

    private WaveEnemy GetEnemyToSpawn(int index)
    {
        // default enemy, will be returned if no enemy is found
        WaveEnemy enemy = _waveEnemies[0];

        // this list will contain all enemies available to spawn
        List<WaveEnemy> enemiesAvailable = new List<WaveEnemy>();

        // determine which enemies are available
        foreach(WaveEnemy e in _waveEnemies)
        {
            // must match wave to spawn
            if (currentWave < e.spawnsPastWave) continue;

            // must spawn enemy once on the first wave they exist
            if (currentWave == e.spawnsPastWave && index == 0) {
                enemy = e;
                return enemy;
            }

            // add this enemy to available enemies
            enemiesAvailable.Add(e);
        }

        int iterations = 0;
        bool found = false;

        // this will handle determining an enemy of rarity
        do
        {
            // roll 1 - 21
            float roll = Random.Range(1, 20);
            int matches = 0;

            foreach(WaveEnemy e in enemiesAvailable)
            {
                if(roll > e.rarity.x && roll < e.rarity.y)
                {
                    if(matches == 0) enemy = e;
                    else {
                        int choose = Random.Range(0, 2);
                        // set to random of chosen enemies
                        enemy = choose == 0 ? e : enemy;
                    }

                    matches++;
                }
            }

            if (matches > 0) found = true;

            iterations++;
            // break out if still can't get an enemy. will use default enemy
            if (iterations > 20) found = true;
        } while (!found);

        return enemy;
    }

    private void Spawn(WaveEnemy enemy)
    {
        // spawns an enemy
        enemy.activeEnemy = Instantiate(enemy.enemyPrefab, new Vector2(transform.position.x + Random.Range(_minMaxSpawnX.x, _minMaxSpawnX.y), transform.position.y), Quaternion.identity);

        // add to list of enemies
        enemiesActive.Add(enemy);
    }

    /// <summary>
    /// Retrieves the enemy count based on the enemy curve
    /// </summary>
    private void UpdateEnemyCount()
    {
        float curveValue = _enemiesOverWaves.Evaluate((float)currentWave / (float)_maxWaveEvaluation);
        enemiesCount = Mathf.FloorToInt(Mathf.Lerp(enemiesCount, _maxSpawnCount, 1 - Mathf.Pow(1 - curveValue, curveValue)));
    }
}
