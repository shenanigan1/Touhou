using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnnemies : MonoBehaviour
{
    public enum Pattern
    {
        Circle, Target
    }

    public Pattern _pattern;
    public Type _type;

    [SerializeField] private Vector3 _direction = new Vector3(0, 1, 0);

    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private float _waitTimer = 0;
    [SerializeField] private float _timeBeforeShoot = 0.1f;
    [SerializeField] private float _shootTimer = 0;
    [SerializeField] private int _numberOfShootWave = 1;
    [SerializeField] private int _numberOfProjectilePerWave = 20;

    public GameObject _player;
    private int waveIndex = 0;

    [SerializeField] private bool _canFire = false;
    private Bullet_script _bullet;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    // Update is called once per frame
    void Update()
    {
        if (waveIndex >= _numberOfShootWave)
        {
            _waitTimer = Time.time + _waitTime;
            waveIndex = 0;
        }

        else if (_waitTimer < Time.time)
        {
            if (_shootTimer < Time.time && _canFire)
            {
                _shootTimer = Time.time + _timeBeforeShoot;
                switch (_pattern)
                {
                    case Pattern.Circle:
                        FirePatternCircle();
                        break;
                    case Pattern.Target:
                        FirePatternTarget();
                        break;
                }
                waveIndex++;
            }
        }
    }

    private void FirePatternCircle()
    {
        for(int i =0; i< _numberOfProjectilePerWave; i++)
        {
            _bullet = PoolController.Instance.GetNew(_type, gameObject.transform.position, "EnemiesBullet").GetComponent<Bullet_script>();
            _bullet.transform.rotation = Quaternion.Euler(0,0, (360/_numberOfProjectilePerWave)*i);
            Debug.Log("Rotation ::: " + (360 / _numberOfProjectilePerWave) * i + "  ////   " + _bullet.transform.up);
            _bullet.SetDirection = _bullet.transform.up;
        }
    }

    private void FirePatternTarget()
    {
        for (int i = 0; i < _numberOfProjectilePerWave; i++)
        {
            _bullet = PoolController.Instance.GetNew(_type, gameObject.transform.position, "EnemiesBullet").GetComponent<Bullet_script>();
            //_bullet.transform.LookAt(_player.transform);
            //Debug.Log("Rotation ::: " + _bullet.transform.rotation.eulerAngles.x);
            //_bullet.transform.rotation =  Quaternion.Euler(0,0,_bullet.transform.rotation.eulerAngles.x);
            //Debug.Log("Rotation ::: " + (360 / _numberOfProjectilePerWave) * i + "  ////   " + _bullet.transform.up);
            _bullet.SetDirection = (_player.transform.position - _bullet.transform.position).normalized;
        }
    }

    public Type GetHisType => _type;
    public bool CanFire 
    { set
        {
            _canFire = value;
            _waitTimer = Time.time + _waitTime;
        }   
    }
}
