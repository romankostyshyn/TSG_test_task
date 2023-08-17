using TSG.Model;

public interface ILeaderboardDataProvider
{
    LeaderboardEntryModel GetDataByID(int index);
}