using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum for different poolable object types.
/// </summary>
public enum Type
{
    Ennemies1, Ennemies2, Ball, CurveBall, Ennemies3, Explosion, Boss, Bonus
}

/// <summary>
/// Manages object pooling for various prefab types to improve performance.
/// </summary>
public class PoolController : MonoBehaviour
{
    public static PoolController Instance;

    // Dictionary to hold queues of pooled objects by type
    private Dictionary<Type, Queue<GameObject>> _pools = new();

    // Prefabs to instantiate and pool (set in inspector)
    public List<GameObject> _prefabs = new();

    // Number of instances to create per prefab (set in inspector)
    public List<int> _numberOfPrefabInstance = new();

    private void Awake()
    {
        // Singleton pattern enforcement
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes pools by instantiating objects and adding them to their respective queues.
    /// </summary>
    private void Initialize()
    {
        // Initialize queues for all Types
        foreach (Type type in Enum.GetValues(typeof(Type)))
        {
            _pools[type] = new Queue<GameObject>();
        }

        // Validate lists are of same length
        if (_prefabs.Count != _numberOfPrefabInstance.Count)
        {
            throw new Exception("Prefab and NumberOfPrefabInstance lists must be of equal length.");
        }

        // Instantiate and enqueue the requested number of instances per prefab
        for (int i = 0; i < _prefabs.Count; i++)
        {
            GameObject prefab = _prefabs[i];
            int count = _numberOfPrefabInstance[i];

            for (int j = 0; j < count; j++)
            {
                GameObject go = Instantiate(prefab);
                go.SetActive(false);

                // Determine the pool type from attached scripts
                if (go.TryGetComponent<Bullet_script>(out Bullet_script bullet))
                {
                    _pools[bullet.GetHisType].Enqueue(go);
                }
                else if (go.TryGetComponent<EnemyController>(out EnemyController enemy))
                {
                    _pools[enemy.GetHisType].Enqueue(go);
                }
                else if (go.TryGetComponent<ExplosionScript>(out ExplosionScript _))
                {
                    _pools[Type.Explosion].Enqueue(go);
                }
                else
                {
                    Debug.LogWarning($"Prefab '{prefab.name}' does not have a recognized component to determine pool type.");
                }
            }
        }
    }

    /// <summary>
    /// Retrieves an object from the pool of the specified type and sets its position.
    /// Returns null if no object is available.
    /// </summary>
    public GameObject GetNew(Type type, Vector3 position)
    {
        if (_pools.TryGetValue(type, out Queue<GameObject> pool) && pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
            go.transform.position = position;
            go.SetActive(true);
            return go;
        }
        else
        {
            Debug.LogWarning($"Pool for type '{type}' is empty or does not exist.");
            return null;
        }
    }

    /// <summary>
    /// Retrieves an object from the pool of the specified type, sets its position and tag.
    /// </summary>
    public GameObject GetNew(Type type, Vector3 position, string tag)
    {
        GameObject go = GetNew(type, position);
        if (go != null)
        {
            go.tag = tag;
        }
        return go;
    }

    /// <summary>
    /// Returns the specified GameObject back to its pool and deactivates it.
    /// </summary>
    public void Suppr(GameObject go, Type type)
    {
        if (go == null) return;

        go.SetActive(false);

        if (_pools.TryGetValue(type, out Queue<GameObject> pool))
        {
            pool.Enqueue(go);
        }
        else
        {
            Debug.LogWarning($"Trying to return GameObject to a non-existent pool of type '{type}'.");
            Destroy(go); // Optionally destroy if no pool exists
        }
    }
}
