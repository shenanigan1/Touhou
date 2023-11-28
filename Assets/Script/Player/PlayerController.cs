using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerObj _playerObj;

    private SpriteRenderer _spriteRenderer;

    private Transform _transform;

    private Rigidbody2D _rb;

    private Color _color = Color.white;

    public GameObject MovementRect;

    public TextMeshProUGUI _lifeTxt;
    public TextMeshProUGUI _gameTimerTxt;

    private Shoot_script _shootScript;

    private Vector2 _movement = new Vector2();
    private Vector2 _LeftUp = new Vector2();
    private Vector2 _RightDown = new Vector2();

    private Vector3 _playerSpace = new Vector3();

    private int _startLife = 3;

    [SerializeField] private float _speed = 10;
    private float InvincibleTime = 2;
    [SerializeField]private float InvincibleTimeAspect = 0.2f;
    private float _gameTimerOffset = 0;

    private bool _fire = false;
    private bool _invicible = false;
    private bool _transparent = false;

    private void Awake()
    {
        _playerObj.life = _startLife;
        _shootScript = GetComponent<Shoot_script>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _LeftUp = MovementRect.transform.GetChild(0).position;
        _RightDown = MovementRect.transform.GetChild(1).position;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameTimerOffset = Time.time;
    }

    private void Update()
    {
        _lifeTxt.text = "Life : " + _playerObj.life;
        _gameTimerTxt.text = "Timer : " + string.Format("{0:0.0}",Time.time - _gameTimerOffset);
    }

    void FixedUpdate()
    {
        VerifMovement();
        _rb.AddForce(_movement * _speed);
    }

    private void VerifMovement()
    {
        Debug.Log(_movement);
        if ((_movement.x > 0 && _transform.position.x > _RightDown.x) || (_movement.x < 0 && _transform.position.x < _LeftUp.x))
        {
            ReplacePlayerInX();
            _movement.x = 0;
        }
        else if ((_transform.position.x > _RightDown.x) || (_transform.position.x < _LeftUp.x))
            ReplacePlayerInX();


        if ((_movement.y < 0 && _transform.position.y < _RightDown.y) || (_movement.y > 0 && _transform.position.y > _LeftUp.y))
        {
            ReplacePlayerInY();
            _movement.y = 0;
        }
        else if((/*_movement.y < 0 &&*/ _transform.position.y < _RightDown.y) || (/*_movement.y > 0 &&*/ _transform.position.y > _LeftUp.y))
            ReplacePlayerInY();
    }

    private void ReplacePlayerInX()
    {
        _playerSpace.Set(_transform.position.x > _RightDown.x ? _RightDown.x : _LeftUp.x, _transform.position.y, 0);
        _transform.position = _playerSpace;
        _rb.velocity = new Vector2(0, _rb.velocity.y);

    }

    private void ReplacePlayerInY()
    {
        _playerSpace.Set(_transform.position.x, _transform.position.y < _RightDown.y ? _RightDown.y : _LeftUp.y, 0);
        _transform.position = _playerSpace;
        _rb.velocity = new Vector2(_rb.velocity.x, 0);

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.started && !_invicible)
        {
            _fire = !_fire;
            _shootScript.IsFire = _fire;
            Debug.Log(_fire);
        }
 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.CompareTag("EnemiesBullet") || collision.CompareTag("Enemy")) && !_invicible)
        {
            _playerObj.life--;
            StartCoroutine(Invincible());
            IsDead();
        }
    }

    private void IsDead()
    {
        if (_playerObj.life <= 0)
        {
            _playerObj.timer = Time.time - _gameTimerOffset;
            gameObject.SetActive(false);
            SceneManager.LoadScene(2);
        }
    }
    private IEnumerator Invincible()
    {
        _invicible = true;
        _fire = false;
        _shootScript.IsFire = _fire;
        StartCoroutine(InvincibleAspect());
        yield return new WaitForSeconds(InvincibleTime);
        _invicible = false;
    }

    private IEnumerator InvincibleAspect()
    {
        while(_invicible) 
        {
            yield return new WaitForSeconds(InvincibleTimeAspect);
            _color.a = _transparent ? 1 : 0.1f;
            _transparent = !_transparent;
            _spriteRenderer.color = _color;
        }
        _color.a = 1;
        _transparent = false;
        _spriteRenderer.color = _color;
    }

    public Vector2 GetMovement() => _movement;
}
