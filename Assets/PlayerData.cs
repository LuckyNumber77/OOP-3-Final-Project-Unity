[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int balance = 1000;

    public PlayerData(string name)
    {
        playerName = name;
    }
}
