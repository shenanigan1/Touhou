using UnityEngine;

/// <summary>
/// ScriptableObject that holds player data such as score, life, wave, and survival timer.
/// This asset allows sharing player state data across scenes and scripts.
/// </summary>
[CreateAssetMenu(fileName = "PlayerObj", menuName = "Custom/PlayerObj")]
public class PlayerObj : ScriptableObject
{
    [Tooltip("Current player score")]
    public int score;

    [Tooltip("Remaining player lives")]
    public int life;

    [Tooltip("Current wave number reached")]
    public int wave;

    [Tooltip("Survival time in seconds")]
    public float timer;
}
