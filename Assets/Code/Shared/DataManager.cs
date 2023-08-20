using TSG.Utils;
using UnityEngine;

public class DataManager : IManager
{
    public int HighScore { get; private set; }

    public DataManager()
    {
        HighScore = PlayerPrefs.GetInt("HighScore");
    }

    public void SaveHighScore(int score)
    {
        if (score > HighScore)
        {
            HighScore = score;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }
}
