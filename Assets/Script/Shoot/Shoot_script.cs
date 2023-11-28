using UnityEngine;

public class Shoot_script : MonoBehaviour
{
    public Type _type;

    [SerializeField] private Vector3 _direction = new Vector3(0, 1, 0);

    [SerializeField] private float _timeBeforeShoot = 0.1f;
    [SerializeField] private float _shootTimer = 0;

    private Bullet_script _bullet;

    private bool _fire = false;

    // Update is called once per frame
    void Update()
    {
        if(_fire)
        {
            FirePattern1();
        }
    }

    private void FirePattern1()
    {
        if(_shootTimer < Time.time)
        {
            _shootTimer = Time.time + _timeBeforeShoot;
            _bullet =  PoolController.Instance.GetNew(_type, gameObject.transform.position, "Bullet").GetComponent<Bullet_script>();
            _bullet.SetDirection = _direction;
        }

    }

    public Type GetHisType => _type;
    public bool IsFire { set => _fire = value; }
}
