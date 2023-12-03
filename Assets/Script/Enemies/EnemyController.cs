using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int _life;
    [SerializeField] private int _score;
    [SerializeField] private int _startLife;
    [SerializeField] private float _speed = 10;

    private Shoot_script _player;
    public Type _type;
    [SerializeField]private Vector3 _targetPosition = new();
    private Transform _transform;

    private ShootEnnemies _shoot;
    private bool _isTouch = false;

    private string[] _tagEffectTab = new string[4] { "LifeBonus", "BulletBonus", "Pattern1Bonus", "Pattern2Bonus" };

    // Start is called before the first frame update
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Shoot_script>();
        _transform = gameObject.GetComponent<Transform>();
        _shoot = GetComponent<ShootEnnemies>();
    }   

    // Update is called once per frame
    private void OnEnable()
    {
        _isTouch = false;
        StartCoroutine(MoveToTarget());

    }

    private void OnDisable()
    {
        _life = _startLife;
        _shoot.CanFire = false;
    }

    private IEnumerator MoveToTarget() 
    {
        while(Vector3.Distance(_transform.position, _targetPosition) > 0.1f)
        {
            _transform.position += _speed * Time.deltaTime * (_targetPosition - _transform.position).normalized;
            yield return null;
        }
        _shoot.CanFire = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            _life--;
            VerifLife();
            PoolController.Instance.Suppr(collision.gameObject, collision.gameObject.GetComponent<Bullet_script>().GetHisType);
        }
        else if (collision.gameObject.CompareTag("Player") && _type != Type.Boss)
        {
            _life = 0;
            VerifLife();
        }
    }

    private void VerifLife()
    {
        if(_life <= 0 && !_isTouch) 
        {
            _isTouch = true;
            EnemiesSpawner.Instance.EnemyDead();
            Debug.Log("Death Of :: " + gameObject);
            PoolController.Instance.Suppr(gameObject, _type);
            PoolController.Instance.GetNew(Type.Explosion, _transform.position);
            EnemiesSpawner.Instance.AddScore(_score);
            SpawnEffect();
        }
    }

    private void SpawnEffect()
    {
        int rand = Random.Range(0, 101);
        if(rand < 10) 
        {
            rand = Random.Range(0, _tagEffectTab.Length);
            if(rand == 0 && EnemiesSpawner.Instance._playerObj.life > 5) { return; }
            if(rand == 1 && _player.GetNumberOfBullet > 5 ) { return; }
            PoolController.Instance.GetNew(Type.Bonus, _transform.position, _tagEffectTab[rand]);
        }
    }

    public Type GetHisType => _type;
    public Vector3 SetTargetPosition {set => _targetPosition = value; }
}
