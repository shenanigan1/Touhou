using UnityEngine;

/// <summary>
/// Simple script to move the GameObject upwards at a constant speed,
/// typically used for parallax background layers.
/// </summary>
public class MoveParallax : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    void Update()
    {
        // Move upward along Y-axis at 'speed' units per second
        transform.position += Vector3.up * speed * Time.deltaTime;
    }
}
