using System.Collections;
using UnityEngine;

/// <summary>
/// Controls enemy behavior: movement towards a target position, health management, and shooting activation.
/// Handles collisions with player bullets and player, and manages death and bonus spawn logic.
/// </summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>Current life points of the enemy.</summary>
    private int _life;

    /// <summary>Score awarded to the player upon enemy death.</summary>
    [SerializeField] private int _score;

    /// <summary>Initial life points of the enemy when spawned or enabled.</summary>
    [SerializeField] private int _startLife;

    /// <summary>Movement speed of the enemy towards its target position.</summary>
    [SerializeField] private float _speed = 10;

    /// <summary>Reference to the player shooting script to access player stats and info.</summary>
    private Shoot_script _player;

    /// <summary>Type identifier of this enemy (used for pooling and logic).</summary>
    public Type _type;

    /// <summary>Target position the enemy moves towards before starting to shoot.</summary>
    [SerializeField] private Vector3 _targetPosition = new();

    /// <summary>Cached transform component for performance optimization.</summary>
    private Transform _transform;

    /// <summary>Reference to the shooting component managing enemy projectiles.</summary>
    private ShootEnemies _shoot;

    /// <summary>Flag to ensure death logic executes only once per enemy death.</summary>
    private bool _isTouch = false;

    /// <summary>Array of tags representing possible bonus effects that can spawn on enemy death.</summary>
    private string[] _tagEffectTab = new string[4] { "LifeBonus", "BulletBonus", "Pattern1Bonus", "Pattern2Bonus" };

    /// <summary>
    /// Initialization: finds player shooting script, caches transform and shooting components.
    /// </summary>
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Shoot_script>();
        _transform = gameObject.GetComponent<Transform>();
        _shoot = GetComponent<ShootEnemies>();
    }

    /// <summary>
    /// Called when the enemy becomes active. Resets death flag and starts moving towards the target.
    /// </summary>
    private void OnEnable()
    {
        _isTouch = false;
        StartCoroutine(MoveToTarget());
    }

    /// <summary>
    /// Called when the enemy is disabled. Resets life and disables shooting.
    /// </summary>
    private void OnDisable()
    {
        _life = _startLife;
        _shoot.CanFire = false;
    }

    /// <summary>
    /// Coroutine that moves the enemy towards the target position smoothly.
    /// Once the target is reached, activates the enemy shooting behavior.
    /// </summary>
    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(_transform.position, _targetPosition) > 0.1f)
        {
            _transform.position += _speed * Time.deltaTime * (_targetPosition - _transform.position).normalized;
            yield return null;
        }
        _shoot.CanFire = true;
    }

    /// <summary>
    /// Handles collisions with bullets and player.
    /// On bullet collision, decreases life and checks death.
    /// On player collision (if not a boss), kills enemy immediately.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            _life--;
            VerifLife();

            // Return bullet to pool
            PoolController.Instance.Suppr(collision.gameObject, collision.gameObject.GetComponent<Bullet_script>().GetHisType);
        }
        else if (collision.gameObject.CompareTag("Player") && _type != Type.Boss)
        {
            _life = 0;
            VerifLife();
        }
    }

    /// <summary>
    /// Checks if the enemy is dead (life <= 0) and triggers death logic only once.
    /// Handles score addition, spawning effects, and returning enemy to the pool.
    /// </summary>
    private void VerifLife()
    {
        if (_life <= 0 && !_isTouch)
        {
            _isTouch = true;
            EnemiesSpawner.Instance.NotifyEnemyDeath();
            Debug.Log("Death Of :: " + gameObject);

            // Return enemy to pool and spawn explosion effect
            PoolController.Instance.Suppr(gameObject, _type);
            PoolController.Instance.GetNew(Type.Explosion, _transform.position);

            // Add score to player
            EnemiesSpawner.Instance.AddScore(_score);

            // Possibly spawn bonus effect on death
            SpawnEffect();
        }
    }

    /// <summary>
    /// Randomly decides to spawn a bonus effect upon enemy death.
    /// Prevents spawning certain bonuses if player conditions are already high.
    /// </summary>
    private void SpawnEffect()
    {
        int rand = Random.Range(0, 101);

        // 10% chance to spawn a bonus
        if (rand < 10)
        {
            rand = Random.Range(0, _tagEffectTab.Length);

            // Prevent LifeBonus spawn if player life is already high
            if (rand == 0 && EnemiesSpawner.Instance.PlayerObj.life > 5) { return; }

            // Prevent BulletBonus spawn if player bullet count is high
            if (rand == 1 && _player.GetNumberOfBullet > 5) { return; }

            PoolController.Instance.GetNew(Type.Bonus, _transform.position, _tagEffectTab[rand]);
        }
    }

    /// <summary>
    /// Gets the enemy's type.
    /// </summary>
    public Type GetHisType => _type;

    /// <summary>
    /// Sets the target position for the enemy to move toward.
    /// </summary>
    public Vector3 SetTargetPosition { set => _targetPosition = value; }
}
