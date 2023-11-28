using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int _life = 10;
    [SerializeField] private int _score = 10;
    [SerializeField] private int _startLife = 10;
    [SerializeField] private float _speed = 10;
    public Type _type;
    private Vector3 _targetPosition = new();
    private Transform _transform;

    private ShootEnnemies _shoot;
    private bool _isTouch = false;

    // Start is called before the first frame update
    void Awake()
    {
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
        else if (collision.gameObject.CompareTag("Player"))
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
            PoolController.Instance.Suppr(gameObject, _type);
            PoolController.Instance.GetNew(Type.Explosion, _transform.position);
            EnemiesSpawner.Instance.AddScore(_score);
        }
    }

    public Type GetHisType => _type;
    public Vector3 SetTargetPosition {set => _targetPosition = value; }
}
