using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Responsible for spawning enemies in waves and managing wave progression.
/// </summary>
public class EnemiesSpawner : MonoBehaviour
{
    public PlayerObj PlayerObj;

    public static EnemiesSpawner Instance { get; private set; }

    [Header("Spawn Areas")]
    [SerializeField] private GameObject spawnRect;
    [SerializeField] private GameObject fightRect;

    [Header("Enemy Spawn Curves")]
    [SerializeField] private AnimationCurve numberOfEnemy1PerWave;
    [SerializeField] private AnimationCurve numberOfEnemy2PerWave;
    [SerializeField] private AnimationCurve numberOfEnemy3PerWave;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Wave Settings")]
    [SerializeField] private float timeInWave = 10f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int maxEnemiesSpawnAtOnce = 10;

    // Internal tracking
    private readonly int[] enemiesRemainingPerType = new int[3];
    private List<int> enemiesCanBeSpawned = new List<int>();
    private int activeEnemyCount = 0;
    private int enemiesToSpawnThisBatch = 0;
    private bool isSpawningNewWave = false;
    private float waveTimer;

    private Vector3 upLeft;
    private Vector3 downRight;
    private Vector3 randomPosition;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Singleton safety

        ResetWaveData();
    }

    private void ResetWaveData()
    {
        PlayerObj.wave = 0;
        PlayerObj.score = 0;
        activeEnemyCount = 0;
        isSpawningNewWave = false;
        ClearEnemyCounters();
    }

    private void ClearEnemyCounters()
    {
        for (int i = 0; i < enemiesRemainingPerType.Length; i++)
            enemiesRemainingPerType[i] = 0;
        enemiesCanBeSpawned.Clear();
    }

    private void Update()
    {
        UpdateUI();

        // When all active enemies and queued enemies are zero and not spawning, start a new wave
        if (activeEnemyCount <= 0 && TotalEnemiesRemaining() <= 0 && !isSpawningNewWave)
        {
            StartCoroutine(NewWaveCoroutine());
        }
        else if (activeEnemyCount <= 1)
        {
            ContinueWave();
        }
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score : {PlayerObj.score}";
        waveText.text = $"Wave : {PlayerObj.wave}";
    }

    /// <summary>
    /// Starts a new wave with updated enemy counts based on the animation curves.
    /// </summary>
    private IEnumerator NewWaveCoroutine()
    {
        isSpawningNewWave = true;

        yield return new WaitForSecondsRealtime(timeBetweenWaves);

        PlayerObj.wave++;

        ClearEnemyCounters();

        // Calculate enemies per type for this wave
        enemiesRemainingPerType[0] = Mathf.CeilToInt(numberOfEnemy1PerWave.Evaluate(PlayerObj.wave));
        enemiesRemainingPerType[1] = Mathf.CeilToInt(numberOfEnemy2PerWave.Evaluate(PlayerObj.wave));
        enemiesRemainingPerType[2] = Mathf.CeilToInt(numberOfEnemy3PerWave.Evaluate(PlayerObj.wave));

        // Register types that can be spawned (with at least one enemy)
        for (int i = 0; i < enemiesRemainingPerType.Length; i++)
        {
            if (enemiesRemainingPerType[i] > 0)
                enemiesCanBeSpawned.Add(i);
        }

        isSpawningNewWave = false;
    }

    /// <summary>
    /// Continues spawning enemies during the current wave in batches.
    /// </summary>
    private void ContinueWave()
    {
        enemiesToSpawnThisBatch = 0;

        int totalEnemiesLeft = TotalEnemiesRemaining();

        if (totalEnemiesLeft <= 0)
            return;

        if (totalEnemiesLeft >= maxEnemiesSpawnAtOnce)
        {
            enemiesToSpawnThisBatch = maxEnemiesSpawnAtOnce;
        }
        else
        {
            enemiesToSpawnThisBatch = totalEnemiesLeft;
        }

        DecrementEnemiesRemaining(enemiesToSpawnThisBatch);

        activeEnemyCount += enemiesToSpawnThisBatch;

        for (int i = 0; i < enemiesToSpawnThisBatch; i++)
        {
            SpawnEnemy();
        }

        waveTimer = Time.time + timeInWave;
    }

    private int TotalEnemiesRemaining()
    {
        int total = 0;
        foreach (var count in enemiesRemainingPerType)
            total += count;
        return total;
    }

    /// <summary>
    /// Decreases enemies remaining for the wave by given count.
    /// Distributes decreases proportionally or simply by priority.
    /// </summary>
    private void DecrementEnemiesRemaining(int count)
    {
        // Simple distribution: reduce from each type until count exhausted
        while (count > 0 && enemiesCanBeSpawned.Count > 0)
        {
            int randomIndex = Random.Range(0, enemiesCanBeSpawned.Count);
            int enemyTypeIndex = enemiesCanBeSpawned[randomIndex];

            if (enemiesRemainingPerType[enemyTypeIndex] > 0)
            {
                enemiesRemainingPerType[enemyTypeIndex]--;
                count--;

                if (enemiesRemainingPerType[enemyTypeIndex] <= 0)
                {
                    enemiesCanBeSpawned.RemoveAt(randomIndex);
                }
            }
            else
            {
                enemiesCanBeSpawned.RemoveAt(randomIndex);
            }
        }
    }

    /// <summary>
    /// Spawns a single enemy at a random spawn position.
    /// </summary>
    private void SpawnEnemy()
    {
        GetRandomPositionInRect(spawnRect);

        GameObject enemyGO = PoolController.Instance.GetNew(GetRandomEnemyType(), randomPosition);
        EnemyController enemy = enemyGO?.GetComponent<EnemyController>();

        if (enemy != null)
        {
            GetRandomPositionInRect(fightRect);

            if (enemy._type != Type.Boss)
                enemy.SetTargetPosition = randomPosition;
            else
                enemy.SetTargetPosition = new Vector3(-2.5f, 3.8f, 0);
        }
        else
        {
            // Failed to spawn enemy, decrement active count
            activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
        }
    }

    /// <summary>
    /// Picks a random enemy type from the available enemies to spawn.
    /// </summary>
    /// <returns>The Type enum for the enemy.</returns>
    private Type GetRandomEnemyType()
    {
        if (enemiesCanBeSpawned.Count == 0)
            return Type.Ennemies1; // Default fallback

        int randomIndex = Random.Range(0, enemiesCanBeSpawned.Count);
        int enemyTypeIndex = enemiesCanBeSpawned[randomIndex];

        return GetEnemyTypeFromIndex(enemyTypeIndex);
    }

    private static Type GetEnemyTypeFromIndex(int index)
    {
        return index switch
        {
            0 => Type.Ennemies1,
            1 => Type.Ennemies2,
            2 => Type.Boss,
            _ => Type.Ennemies1,
        };
    }

    /// <summary>
    /// Gets a random position inside the rectangle defined by rect's first two children (corners).
    /// </summary>
    private void GetRandomPositionInRect(GameObject rect)
    {
        upLeft = rect.transform.GetChild(0).position;
        downRight = rect.transform.GetChild(1).position;

        randomPosition.x = Random.Range(upLeft.x, downRight.x);
        randomPosition.y = Random.Range(downRight.y, upLeft.y);
        randomPosition.z = 0;
    }

    /// <summary>
    /// Should be called by enemy when it dies to update active enemy count.
    /// </summary>
    public void NotifyEnemyDeath()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    /// <summary>
    /// Adds score to player.
    /// </summary>
    public void AddScore(int score)
    {
        PlayerObj.score += score;
    }
}
