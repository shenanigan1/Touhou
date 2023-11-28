using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObj", menuName = "Custom/PlayerObj")]
public class PlayerObj : ScriptableObject
{
    public int score;
    public int life;
    public int wave;
    public float timer;

}
