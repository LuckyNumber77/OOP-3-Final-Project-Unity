using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int balance = 1000;
    public List<Card> hand = new List<Card>(); // Add this line

    public PlayerData(string name)
    {
        playerName = name;
    }
}
