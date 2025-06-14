using TMPro;
using UnityEngine;

/// <summary>
/// Displays the final game results on the end menu UI.
/// </summary>
public class EndMenu : MonoBehaviour
{
    [Tooltip("Reference to the PlayerObj that holds game data")]
    public PlayerObj player;

    [Tooltip("Text UI for displaying the final calculated score")]
    public TextMeshProUGUI FinalScore;

    [Tooltip("Text UI for displaying the in-game score")]
    public TextMeshProUGUI ScoreInGame;

    [Tooltip("Text UI for displaying survival time")]
    public TextMeshProUGUI Timer;

    [Tooltip("Text UI for displaying wave number")]
    public TextMeshProUGUI Wave;

    private const int TimerScoreMultiplicator = 100;
    private const int WaveScoreMultiplicator = 50;

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError("PlayerObj reference is not assigned in EndMenu.");
            return;
        }

        int finalScore = (int)(player.timer * TimerScoreMultiplicator) + player.score + player.wave * WaveScoreMultiplicator;

        FinalScore.text = $"Score : {finalScore}";
        ScoreInGame.text = $"Score In Game : {player.score}";
        Timer.text = $"Survival Time : {player.timer:0.0} second{(player.timer > 1 ? "s" : "")}";
        Wave.text = $"Wave : {player.wave}";
    }
}
