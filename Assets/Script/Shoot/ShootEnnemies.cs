using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnnemies : MonoBehaviour
{
    public enum Pattern
    {
        Circle, Target, Lazer, Tentacule, Curve
    }

    [System.Serializable]
    private struct Shoot 
    {
        [Header("Get The Pattern of shoot")]
        public Pattern _pattern;
        [Header("Get the object you want use like bullet")]
        public Type _type;
        //public Vector3 _direction;
        [Header("Get the time you want wait before an other wave of ball")]
        public float _waitTime;
        [Header("Get the time you want wait between each wave")]
        public float _timeBeforeShoot;
        [Header("Get the number of wave you want")]
        public int _numberOfShootWave;
        [Header("Get the number of bullet you want in one wave")]
        public int _numberOfProjectilePerWave;

    }

    [SerializeField] private List<Shoot> shootPattern = new();

    //public Pattern _pattern;
    //public Type _type;

    //[SerializeField] private Vector3 _direction = new Vector3(0, 1, 0);

    //[SerializeField] private float _waitTime = 2f;
    [SerializeField] private float _waitTimer = 0;
    //[SerializeField] private float _timeBeforeShoot = 0.1f;
    [SerializeField] private float _shootTimer = 0;
    //[SerializeField] private int _numberOfShootWave = 1;
    //[SerializeField] private int _numberOfProjectilePerWave = 20;

    public GameObject _player;
    public GameObject _warning;
    public GameObject _lazer;

    private Shoot _currentShoot;

    private Transform _transform;

    private int _indexPattern = 0;
    private int waveIndex = 0;

    private Vector3 _Position = Vector3.zero;

    [SerializeField] private bool _canFire = false;
    private Bullet_script _bullet;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _transform = GetComponent<Transform>();
        GetRandomPattern();
    }
    // Update is called once per frame
    void Update()
    {
        if (waveIndex >= _currentShoot._numberOfShootWave)
        {
            _waitTimer = Time.time + _currentShoot._waitTime;
            waveIndex = 0;
            GetRandomPattern();
        }

        else if (_waitTimer < Time.time)
        {
            if (_shootTimer < Time.time && _canFire)
            {
                _shootTimer = Time.time + _currentShoot._timeBeforeShoot;
                switch (shootPattern[_indexPattern]._pattern)
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
                }
                waveIndex++;
            }
        }
    }

    private void GetRandomPattern() 
    {
        if (shootPattern.Count > 1)
        {
            _indexPattern = Random.Range(0, shootPattern.Count);
        }
        else
            _indexPattern = 0;

        _currentShoot = shootPattern[_indexPattern];
    }

    private void FirePatternCircle()
    {
        for(int i =0; i< _currentShoot._numberOfProjectilePerWave; i++)
        {
            _bullet = PoolController.Instance.GetNew(_currentShoot._type, _transform.position, "EnemiesBullet").GetComponent<Bullet_script>();
            _bullet.transform.rotation = Quaternion.Euler(0,0, (360/ _currentShoot._numberOfProjectilePerWave)*i);
            _bullet.SetDirection = _bullet.transform.up;
        }
    }

    private void FirePatternTarget()
    {
        for (int i = 0; i < _currentShoot._numberOfProjectilePerWave; i++)
        {
            _bullet = PoolController.Instance.GetNew(_currentShoot._type, _transform.position, "EnemiesBullet").GetComponent<Bullet_script>();
            //_bullet.transform.LookAt(_player.transform);
            //Debug.Log("Rotation ::: " + _bullet.transform.rotation.eulerAngles.x);
            //_bullet.transform.rotation =  Quaternion.Euler(0,0,_bullet.transform.rotation.eulerAngles.x);
            //Debug.Log("Rotation ::: " + (360 / _numberOfProjectilePerWave) * i + "  ////   " + _bullet.transform.up);
            _bullet.SetDirection = (_player.transform.position - _bullet.transform.position).normalized;
        }
    }

    private void FirePatternCurve()
    {
        float offset = (12 / _currentShoot._numberOfProjectilePerWave);
        for (int i = 0; i < _currentShoot._numberOfProjectilePerWave+1; i++)
        {
            _Position.Set((_transform.position.x - 6) + offset*(i), _transform.position.y, 0);
            _bullet = PoolController.Instance.GetNew(_currentShoot._type, _Position, "EnemiesBullet").GetComponent<Bullet_script>();
            _bullet.SetDirection = Vector3.down;
        }
    }

    private void FirePatternLazer()
    {
        StartCoroutine(LazerFire());
    }

    private IEnumerator LazerFire()
    {
        _lazer.SetActive(true);
        yield return new WaitForSeconds(_currentShoot._waitTime);
        _lazer.SetActive(false);
    }

    public Type GetHisType => _currentShoot._type;
    public bool CanFire 
    { set
        {
            _canFire = value;
            _waitTimer = Time.time + _currentShoot._waitTime;
        }   
    }
}
