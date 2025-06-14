using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages enemy shooting behavior with multiple shooting patterns.
/// Selects and fires projectiles based on predefined patterns and timing.
/// </summary>
public class ShootEnemies : MonoBehaviour
{
    /// <summary>
    /// Different shooting patterns available for enemies.
    /// </summary>
    public enum Pattern
    {
        Circle, Target, Lazer, Tentacule, Curve
    }

    /// <summary>
    /// Represents a single shooting configuration including pattern, projectile type, timing, and quantities.
    /// </summary>
    [System.Serializable]
    private struct Shoot
    {
        [Header("Shooting pattern type")]
        public Pattern pattern;

        [Header("Bullet type to use for this shoot")]
        public Type type;

        [Header("Wait time before starting a new wave of bullets")]
        public float waitTime;

        [Header("Time delay between each projectile fired in a wave")]
        public float timeBeforeShoot;

        [Header("Number of shooting waves to fire")]
        public int numberOfShootWave;

        [Header("Number of projectiles fired per wave")]
        public int numberOfProjectilePerWave;
    }

    [SerializeField] private List<Shoot> shootPatterns = new();

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject warning;
    [SerializeField] private GameObject lazer;

    private Shoot currentShoot;

    private Transform _transform;

    private int indexPattern = 0;
    private int waveIndex = 0;

    // Timers for managing firing cooldowns and delays
    private float waitTimer = 0f;
    private float shootTimer = 0f;

    private bool canFire = false;

    /// <summary>
    /// Initialization: finds player reference and sets initial pattern.
    /// </summary>
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _transform = transform;
        GetRandomPattern();
    }

    /// <summary>
    /// Handles timing and firing waves of projectiles each frame.
    /// </summary>
    private void Update()
    {
        // If current pattern completed all waves, prepare for next pattern after wait time
        if (waveIndex >= currentShoot.numberOfShootWave)
        {
            waitTimer = Time.time + currentShoot.waitTime;
            waveIndex = 0;
            GetRandomPattern();
        }
        // If wait time passed and firing is enabled
        else if (waitTimer < Time.time && canFire)
        {
            // Fire projectiles based on pattern timing
            if (shootTimer < Time.time)
            {
                shootTimer = Time.time + currentShoot.timeBeforeShoot;
                FireCurrentPattern();
                waveIndex++;
            }
        }
    }

    /// <summary>
    /// Selects a random shooting pattern from the list or defaults to the first if only one exists.
    /// </summary>
    private void GetRandomPattern()
    {
        if (shootPatterns.Count > 1)
            indexPattern = Random.Range(0, shootPatterns.Count);
        else
            indexPattern = 0;

        currentShoot = shootPatterns[indexPattern];
    }

    /// <summary>
    /// Fires projectiles according to the current shooting pattern.
    /// </summary>
    private void FireCurrentPattern()
    {
        switch (currentShoot.pattern)
        {
            case Pattern.Circle:
                FirePatternCircle();
                break;
            case Pattern.Target:
                FirePatternTarget();
                break;
            case Pattern.Lazer:
                FirePatternLazer();
                break;
            case Pattern.Curve:
                FirePatternCurve();
                break;
                // Future implementation for Tentacule pattern can go here
        }
    }

    /// <summary>
    /// Fires projectiles evenly distributed in a circle around the shooter.
    /// </summary>
    private void FirePatternCircle()
    {
        int projectiles = currentShoot.numberOfProjectilePerWave;

        for (int i = 0; i < projectiles; i++)
        {
            var bulletGO = PoolController.Instance.GetNew(currentShoot.type, _transform.position, "EnemiesBullet");
            var bullet = bulletGO.GetComponent<Bullet_script>();

            // Calculate rotation angle for even spread
            float angle = (360f / projectiles) * i;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Set bullet direction to 'up' after rotation (direction facing)
            bullet.SetDirection = bullet.transform.up;
        }
    }

    /// <summary>
    /// Fires projectiles aimed directly at the player’s current position.
    /// </summary>
    private void FirePatternTarget()
    {
        int projectiles = currentShoot.numberOfProjectilePerWave;
        Vector3 playerPos = player.transform.position;

        for (int i = 0; i < projectiles; i++)
        {
            var bulletGO = PoolController.Instance.GetNew(currentShoot.type, _transform.position, "EnemiesBullet");
            var bullet = bulletGO.GetComponent<Bullet_script>();

            // Calculate normalized direction vector towards the player
            Vector3 direction = (playerPos - bullet.transform.position).normalized;
            bullet.SetDirection = direction;
        }
    }

    /// <summary>
    /// Fires projectiles in a curved pattern spread horizontally with a downward trajectory.
    /// </summary>
    private void FirePatternCurve()
    {
        int projectiles = currentShoot.numberOfProjectilePerWave;
        float offset = 12f / projectiles;

        for (int i = 0; i <= projectiles; i++)
        {
            // Position bullets spread across a horizontal line centered near shooter
            Vector3 pos = new Vector3((_transform.position.x - 6f) + offset * i, _transform.position.y, 0);
            var bulletGO = PoolController.Instance.GetNew(currentShoot.type, pos, "EnemiesBullet");
            var bullet = bulletGO.GetComponent<Bullet_script>();

            // Set bullets to move straight down
            bullet.SetDirection = Vector3.down;
        }
    }

    /// <summary>
    /// Fires a laser by enabling the laser gameobject for a set duration.
    /// </summary>
    private void FirePatternLazer()
    {
        StartCoroutine(LazerFire());
    }

    /// <summary>
    /// Coroutine to handle the laser active duration and cooldown.
    /// </summary>
    private IEnumerator LazerFire()
    {
        lazer.SetActive(true);
        yield return new WaitForSeconds(currentShoot.waitTime);
        lazer.SetActive(false);
    }

    /// <summary>
    /// Gets the bullet type of the current shooting pattern.
    /// </summary>
    public Type CurrentType => currentShoot.type;

    /// <summary>
    /// Enables or disables firing. When enabled, resets wait timer to start shooting after delay.
    /// </summary>
    public bool CanFire
    {
        set
        {
            canFire = value;
            waitTimer = Time.time + currentShoot.waitTime;
        }
    }
}
