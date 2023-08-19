using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tools.ScrollComponent;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : ScrollItem
{
    [SerializeField] private TMP_Text indexText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Leaderboard dataProvider;

    private string Index { set => indexText.text = value; }
    private string Name { set => nameText.text = value; }
    private string Score { set => scoreText.text = value; }
    
    public void Init(Leaderboard dataProvider)
    {
        this.dataProvider = dataProvider;
    }
    public override void Refresh(int index)
    {
        var data = dataProvider.GetDataByID(index);
        if (data != null)
        {
            index++;
            Index = index.ToString();
            Name = data.Name;
            Score = data.Score.ToString();
        }
        else
        {
            Index = string.Empty;
            Name = string.Empty;
            Score = string.Empty;
        }
    }

    public override void OnClick(int index)
    {
        
    }

    public override void OnDrag(int index)
    {
        
    }

    public override void OnGrab(int index)
    {
        
    }
}
