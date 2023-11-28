using TMPro;
using UnityEngine;

public class EndMenu : MonoBehaviour
{
    public PlayerObj player;

    public TextMeshProUGUI FinalScore;
    public TextMeshProUGUI ScoreInGame;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI Wave;

    private int TimerScoreMultiplicator = 100;
    public int WaveScoreMultiplicator = 50;

    private void Awake()
    {
        FinalScore.text = "Score : " + ((int)(player.timer) * TimerScoreMultiplicator + player.score + player.wave * WaveScoreMultiplicator);
        ScoreInGame.text = "Score In Game : " + player.score;
        Timer.text = "Survival Time : " + string.Format("{0:0.0}", player.timer) + " second";
        Wave.text = "Wave : " + player.wave;
    }
}
