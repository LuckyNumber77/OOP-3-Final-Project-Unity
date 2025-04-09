using UnityEngine;

[System.Serializable]
public class Card
{
    public string rank;
    public string suit;
    public int cardValue;  // Represents the card's value in the game (e.g., 10 for a 10 card)

    // Constructor
    public Card(string rank, string suit, int cardValue)
    {
        this.rank = rank;
        this.suit = suit;
        this.cardValue = cardValue;
    }
}
