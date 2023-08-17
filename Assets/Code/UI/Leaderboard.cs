using System;
using System.Collections;
using System.Collections.Generic;
using Tools.ScrollComponent;
using TSG.Model;
using UnityEngine;

public class Leaderboard : MonoBehaviour, ILeaderboardDataProvider
{
    [SerializeField] private LeaderboardItem leaderboardItemPrefab;
    [SerializeField] private ScrollComponent scroll;
    private IListModel<LeaderboardEntryModel> leaderboardModel = new LeaderboardModel();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        leaderboardItemPrefab.Init(this);
        scroll.InitWith(leaderboardItemPrefab, leaderboardModel.NumItems);
    }

    public LeaderboardEntryModel GetDataByID(int index)
    {
        return leaderboardModel.GetItem(index);
    }
}
