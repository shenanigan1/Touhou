using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public enum Type 
{
    Ennemies1, Ennemies2, Ball, LittleLine, Ennemies3, Explosion

}
public class PoolController : MonoBehaviour
{
    public static PoolController Instance;

    private Dictionary<Type, Queue<GameObject>> _pools = new();

    public List<GameObject> _prefabs = new();
    public List<int> _numberOfPrefabInstance = new();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        Initialize();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        _pools[Type.Ball] = new Queue<GameObject>();
        _pools[Type.LittleLine] = new Queue<GameObject>();
        _pools[Type.Ennemies1] = new Queue<GameObject>();
        _pools[Type.Ennemies2] = new Queue<GameObject>();
        _pools[Type.Explosion] = new Queue<GameObject>();

        if (_prefabs.Count != _numberOfPrefabInstance.Count)
        {
            throw new Exception("The two list Prefab & NumberOfPrefabInstance are not equal Please Fix It");
        }

        for(int i = 0; i < _prefabs.Count; i++) 
        {
            for(int j = 0; j < _numberOfPrefabInstance[i];  j++)
            {
                GameObject go = Instantiate<GameObject>(_prefabs[i]);
                if(go.TryGetComponent<Bullet_script>(out Bullet_script Bs))
                    _pools[Bs.GetHisType].Enqueue(go);

                else if(go.TryGetComponent<EnemyController>(out EnemyController Es))
                    _pools[Es.GetHisType].Enqueue(go);

                else if(go.TryGetComponent<ExplosionScript>(out ExplosionScript Exs))
                    _pools[Type.Explosion].Enqueue(go);
                go.SetActive(false);
                
                
            }
        }
    }


    public GameObject GetNew(Type T, Vector3 shooter_transform)
    {
        GameObject go = _pools[T].Dequeue();
        go.transform.position = shooter_transform;
        go.SetActive(true);
        return go;
    }

    public GameObject GetNew(Type T, Vector3 shooter_transform, String _tag)
    {
        GameObject go = _pools[T].Dequeue();
        go.transform.position = shooter_transform;
        go.tag = _tag;
        go.SetActive(true);
        return go;
    }

    public void Suppr(GameObject go, Type T)
    {
        go.SetActive(false);
        _pools[T].Enqueue(go);
    }
}
