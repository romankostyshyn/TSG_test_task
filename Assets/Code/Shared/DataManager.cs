using System.Collections;
using System.Collections.Generic;
using TSG.Utils;
using UnityEngine;

public class DataManager : IManager
{
    public int HighScore { get; private set; }

    public DataManager()
    {
        HighScore = PlayerPrefs.GetInt("HighScore");
        Debug.Log(HighScore);
    }

    public void SaveHighScore(int score)
    {
        if (score > HighScore)
        {
            HighScore = score;
        }
        
        Debug.Log(HighScore);
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.Save();
    }
}
