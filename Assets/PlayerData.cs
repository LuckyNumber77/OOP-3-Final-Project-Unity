using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;           // Player's name
    public int balance = 1000;          // Player's balance
    public List<Card> hand = new List<Card>();  // Player's hand of cards
    public int currentBet;              // Player's current bet

    // Constructor to initialize player data
    public PlayerData(string name)
    {
        playerName = name;
        balance = 1000;  // Default balance
        currentBet = 0;  // Initial bet is zero
    }

    // Add a card to the player's hand
    public void AddCardToHand(Card card)
    {
        hand.Add(card);
    }

    // Reset the player's hand for a new round
    public void ClearHand()
    {
        hand.Clear();
    }

    // Get the current value of the player's hand (simplified for Blackjack example)
    public int GetHandValue()
    {
        int value = 0;
        int aces = 0;

        foreach (Card card in hand)
        {
            value += card.cardValue;

            if (card.rank == "Ace")  // Assuming the Card class has a "rank" property
            {
                aces++;
            }
        }

        // Adjust for Aces (Ace can be 1 or 11)
        for (int i = 0; i < aces; i++)
        {
            if (value + 10 <= 21)
            {
                value += 10;  // Make Ace count as 11 if it doesn't bust the hand
            }
        }

        return value;
    }

    // Place a bet for the player
    public void PlaceBet(int betAmount)
    {
        if (betAmount > 0 && betAmount <= balance)
        {
            balance -= betAmount;  // Subtract bet from balance
            currentBet = betAmount;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Invalid bet amount!");
        }
    }

    // Update the player's balance after a win or loss
    public void UpdateBalance(int amount)
    {
        balance += amount;  // Add the win or subtract the loss
    }

    // Ensure player's balance stays non-negative
    public void EnsureValidBalance()
    {
        if (balance < 0)
        {
            balance = 0;
        }
    }

    // Reset the player for a new round (clear hand and bet)
    public void ResetForNewRound()
    {
        ClearHand();    // Clear the player's hand
        currentBet = 0; // Reset the current bet
    }
}
