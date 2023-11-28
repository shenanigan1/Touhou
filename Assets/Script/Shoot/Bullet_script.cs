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

    public Vector3 direction = new Vector3(0,1,0);
    public AnimationCurve XOffsetPosition;
    public AnimationCurve speed;

    private Vector2 _mapLimit = new Vector2(12,8);

    public float time = 0;
    public float baseX = 0;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        time = Time.time;
        baseX = _transform.position.x;

        if(CompareTag("Bullet"))
        {
            spriteRenderer.sprite = sprites[0];
        }
        else
            spriteRenderer.sprite = sprites[1];
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
        if(Mathf.Abs(_transform.position.x) > _mapLimit.x || Mathf.Abs(_transform.position.y) > _mapLimit.y) 
        {
            PoolController.Instance.Suppr(gameObject, _type);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        PoolController.Instance.Suppr(gameObject, _type);
    }

    public Type GetHisType => _type;
    public Vector3 SetDirection { set => direction = value; }
}
