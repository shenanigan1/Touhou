using System.Collections.Generic;
using UnityEngine;

// https://assetstore.unity.com/packages/2d/textures-materials/galaxia-2d-space-shooter-sprite-pack-1-64944
// https://assetstore.unity.com/packages/tools/particles-effects/mk-space-background-parallax-maker-93514
public class Bullet_script : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();

    private SpriteRenderer spriteRenderer;

    public Type _type;

    private Transform _transform;

    private Vector3 _LeftUp;
    private Vector3 _RightDown;
    public Vector3 direction = new Vector3(0,1,0);

    public AnimationCurve XOffsetPosition;
    public AnimationCurve speed;

    public GameObject _mapLimit;

    public float time = 0;
    public float baseX = 0;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();
        _mapLimit = GameObject.Find("MapLimit");
        _LeftUp = _mapLimit.transform.GetChild(0).position;
        _RightDown = _mapLimit.transform.GetChild(1).position;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        time = Time.time;
        baseX = _transform.position.x;

        if (_type == Type.Ball || _type == Type.CurveBall)
        {
            if (CompareTag("Bullet"))
            {
                spriteRenderer.sprite = sprites[0];
            }
            else
                spriteRenderer.sprite = sprites[1];
        }
        else if(_type == Type.Bonus) 
        {
            switch (gameObject.tag)
            {
                case "LifeBonus": spriteRenderer.sprite = sprites[0]; break;
                case "BulletBonus": spriteRenderer.sprite = sprites[1]; break;
                case "Pattern1Bonus": spriteRenderer.sprite = sprites[2]; break;
                case "Pattern2Bonus": spriteRenderer.sprite = sprites[3]; break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ///Faire un bool pour dire si tu touche au X ou pas, Je sais pas trop en vrai réfléchie.
        //_transform.position += new Vector3(_transform.right.x * XOffsetPosition.Evaluate(Time.time - time), _transform.right.y * _transform.position.y, 0);

        _transform.position += (direction + _transform.TransformVector(new Vector3(XOffsetPosition.Evaluate(Time.time - time), 0, 0))).normalized * speed.Evaluate(Time.time - time)* Time.deltaTime;

        //Debug.Log(direction + " //////// " + speed.Evaluate(Time.time - time) + " /////// " + Time.deltaTime);

        IsOutOfLimit();
    }

    private void IsOutOfLimit()
    {
        if ((_transform.position.x > _RightDown.x) || (_transform.position.x < _LeftUp.x))
        {
            PoolController.Instance.Suppr(gameObject, _type);
        }

        if ((_transform.position.y < _RightDown.y) || (_transform.position.y > _LeftUp.y))
        {
            PoolController.Instance.Suppr(gameObject, _type);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_type == Type.Bonus)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<PlayerController>(out PlayerController PC) && collision.gameObject.TryGetComponent<Shoot_script>(out Shoot_script SC))
            {
                Effect(PC, SC);
                PoolController.Instance.Suppr(gameObject, _type);
            }
        }
    }

    private void Effect(PlayerController player, Shoot_script SC)
    {
        switch (gameObject.tag)
        {
            case "LifeBonus": player.AddLife(); break;
            case "BulletBonus": SC.AddBall(); break;
            case "Pattern1Bonus":
                if(SC.pattern != Shoot_script.PlayerPattern.Front)
                {
                    SC.pattern = Shoot_script.PlayerPattern.Front;
                    SC.ResetSpeed();
                }
                else
                    SC.SpeedIncrease();
                break;
            case "Pattern2Bonus":
                if (SC.pattern != Shoot_script.PlayerPattern.SemiCircle)
                {
                    SC.pattern = Shoot_script.PlayerPattern.SemiCircle;
                    SC.ResetSpeed();
                }
                else
                    SC.SpeedIncrease(); 
                break;
        }
    }

    public Type GetHisType => _type;
    public Vector3 SetDirection { set => direction = value; }
}
