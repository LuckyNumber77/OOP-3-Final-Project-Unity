using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour, IDeckManager
{
    [System.Serializable]
    public class CardData
    {
        public string name;
        public Sprite image;
        public int value;
    }

    [Header("Deck Setup")]
    public List<CardData> cardDeck = new List<CardData>(); // List of card data (deck)
    public GameObject cardPrefab;

    [Header("Card Display Areas")]
    public Transform player1CardArea;
    public Transform player2CardArea;
    public Transform dealerCardArea;

    private int currentCardIndex = 0;

    /// <summary>  
    /// Resets the deck state and clears any displayed cards on the table.  
    /// </summary>  
    public void ResetDeck()
    {
        currentCardIndex = 0;

        ClearCardArea(player1CardArea);
        ClearCardArea(player2CardArea);
        ClearCardArea(dealerCardArea);
    }

    /// <summary>  
    /// Clears cards from the specified card area.
    /// </summary>  
    private void ClearCardArea(Transform cardArea)
    {
        if (cardArea != null)
        {
            foreach (Transform child in cardArea)
            {
                Destroy(child.gameObject);  // Destroy all card objects in the area
            }
        }
    }

    /// <summary>  
    /// Deals a specified number of cards to a given player (1 or 2).  
    /// </summary>  
    public void DealCardsToPlayer(int playerNumber, int cardCount)
    {
        if (playerNumber < 1 || playerNumber > 2)
        {
            Debug.LogWarning("Invalid player number passed to DealCardsToPlayer.");
            return;
        }

        Transform cardArea = (playerNumber == 1) ? player1CardArea : player2CardArea;

        for (int i = 0; i < cardCount && currentCardIndex < cardDeck.Count; i++)
        {
            DealCard(cardArea);
        }
    }

    /// <summary>  
    /// Deals a single card to a player and returns the card data.  
    /// </summary>  
    public CardData DealCardToPlayer(int playerNumber)
    {
        if (playerNumber < 1 || playerNumber > 2)
        {
            Debug.LogWarning("Invalid player number passed to DealCardToPlayer.");
            return null;
        }

        if (currentCardIndex >= cardDeck.Count)
        {
            HandleDeckExhaustion();
            return null;
        }

        Transform cardArea = (playerNumber == 1) ? player1CardArea : player2CardArea;
        return DealCard(cardArea);
    }

    /// <summary>  
    /// Deals a card to the dealer and returns the card data.  
    /// </summary>  
    public CardData DealCardToDealer()
    {
        if (dealerCardArea == null)
        {
            Debug.LogWarning("Dealer card area is not assigned.");
            return null;
        }

        if (currentCardIndex >= cardDeck.Count)
        {
            HandleDeckExhaustion();
            return null;
        }

        return DealCard(dealerCardArea);
    }

    /// <summary>  
    /// Deals a single card to the specified card area and returns the card data.  
    /// </summary>  
    private CardData DealCard(Transform cardArea)
    {
        if (currentCardIndex >= cardDeck.Count)
        {
            HandleDeckExhaustion();
            return null;
        }

        CardData currentCard = cardDeck[currentCardIndex];
        GameObject card = Instantiate(cardPrefab, cardArea);  // Instantiate the card prefab
        CardDisplay display = card.GetComponent<CardDisplay>();

        if (display != null)
        {
            display.cardImage.sprite = currentCard.image;
            display.Setup(currentCard.image, currentCard.name, currentCard.value);
        }
        else
        {
            Debug.LogWarning("CardDisplay script is missing on card prefab.");
        }

        currentCardIndex++;
        return currentCard;
    }

    /// <summary>  
    /// Handles logic when the deck is exhausted (reshuffles the deck).  
    /// </summary>  
    private void HandleDeckExhaustion()
    {
        Debug.LogError("Deck is exhausted! Reshuffling the deck.");
        ShuffleDeck();
        currentCardIndex = 0; // Reset index after shuffling  
    }

    /// <summary>  
    /// Shuffles the deck using Fisher-Yates shuffle algorithm.  
    /// </summary>  
    public void ShuffleDeck()
    {
        int n = cardDeck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            CardData value = cardDeck[k];
            cardDeck[k] = cardDeck[n];
            cardDeck[n] = value;
        }

        Debug.Log("Deck shuffled.");
    }
}
