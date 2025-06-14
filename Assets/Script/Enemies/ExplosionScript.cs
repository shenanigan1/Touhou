using UnityEngine;

/// <summary>
/// Handles the explosion animation lifecycle,
/// returns the explosion GameObject to the pool when the animation ends.
/// </summary>
public class ExplosionScript : MonoBehaviour
{
    /// <summary>
    /// Called at the end of the explosion animation (e.g., via Animation Event).
    /// Returns this GameObject to the pool for reuse.
    /// </summary>
    public void EndAnimation()
    {
        if (PoolController.Instance != null)
        {
            PoolController.Instance.Suppr(gameObject, Type.Explosion);
        }
        else
        {
            Debug.LogWarning("PoolController instance not found. Cannot return explosion to pool.");
            // Alternatively, destroy if no pool manager is available
            // Destroy(gameObject);
        }
    }
}
