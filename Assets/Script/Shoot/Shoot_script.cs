using UnityEngine;

public class Shoot_script : MonoBehaviour
{
    public Type _type;

    public enum PlayerPattern
    {
        Front, SemiCircle
    };

    public PlayerPattern pattern = PlayerPattern.Front;   

    [SerializeField] private Vector3 _direction = new Vector3(0, 1, 0);

    private double _timeBeforeShoot = 0.1f;
    [SerializeField] private double _startTimeBeforeShoot = 0.1f;
    [SerializeField] private double _shootTimer = 0;

    [SerializeField] private int _numberOfBulletShoot = 1;

    [SerializeField] private Vector3 _positionOffset = Vector3.zero;

    private Bullet_script _bullet;

    private bool _fire = false;

    // Update is called once per frame
    void Update()
    {
        if(_fire)
        {
            switch(pattern)
            {
                case PlayerPattern.Front: FirePattern1(); break;

                case PlayerPattern.SemiCircle: FirePattern2(); break;
            }
        }
    }

    private void FirePattern1()
    {
        if(_shootTimer < Time.time)
        {
            for(int i = 0; i< _numberOfBulletShoot; i++)
            {
                _bullet = PoolController.Instance.GetNew(_type, (gameObject.transform.position - ((_positionOffset*_numberOfBulletShoot)/2)) + (_positionOffset*(i+0.5f)), "Bullet").GetComponent<Bullet_script>();
                _bullet.SetDirection = _direction;
            }
            _shootTimer = Time.time + _timeBeforeShoot;
        }

    }

    private void FirePattern2()
    {
        if (_shootTimer < Time.time)
        {
            for (int i = 0; i < _numberOfBulletShoot; i++)
            {
                _bullet = PoolController.Instance.GetNew(_type, (gameObject.transform.position), "Bullet").GetComponent<Bullet_script>();
                _bullet.transform.rotation = Quaternion.Euler(0, 0, - 90 + ((180 / (_numberOfBulletShoot+1)) * (i+1)));
                _bullet.SetDirection = _bullet.transform.up;
            }
            _shootTimer = Time.time + _timeBeforeShoot;
        }

    }

    public Type GetHisType => _type;

    public void AddBall(int add = 1) => _numberOfBulletShoot += add;
    public void ResetBall() => _numberOfBulletShoot = 1;
    public int GetNumberOfBullet => _numberOfBulletShoot;

    public void ResetSpeed() => _timeBeforeShoot = _startTimeBeforeShoot;
    public void SpeedIncrease() => _timeBeforeShoot *= 0.75;
    public bool IsFire { set => _fire = value; }
}
