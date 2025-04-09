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
    public List<CardData> cardDeck = new List<CardData>(); // Use this deck  
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
    /// Handles card area clearing.  
    /// </summary>  
    private void ClearCardArea(Transform cardArea)
    {
        if (cardArea != null)
        {
            foreach (Transform child in cardArea)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>  
    /// Deals a specified number of cards to the given player (1 or 2).  
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
    /// Deals a card to the specified card area and returns the card data.  
    /// </summary>  
    private CardData DealCard(Transform cardArea)
    {
        CardData currentCard = cardDeck[currentCardIndex];
        GameObject card = Instantiate(cardPrefab, cardArea);
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
    /// Handles logic when deck is exhausted.  
    /// </summary>  
    private void HandleDeckExhaustion()
    {
<<<<<<< Updated upstream
        Debug.LogError("Deck is exhausted! Reshuffling the deck.");
=======
        Debug.Log("Handling deck exhaustion: Reshuffling the deck.");
>>>>>>> Stashed changes
        ShuffleDeck();
        currentCardIndex = 0; // Reset index after shuffling
    }

<<<<<<< Updated upstream
    /// <summary>  
    /// Shuffles the deck randomly.  
    /// </summary>  
=======
    /// <summary>
    /// Shuffles the deck randomly.
    /// </summary>
>>>>>>> Stashed changes
    public void ShuffleDeck()
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            CardData temp = cardDeck[i];
            int randomIndex = Random.Range(i, cardDeck.Count);
            cardDeck[i] = cardDeck[randomIndex];
            cardDeck[randomIndex] = temp;
        }

        Debug.Log("Deck shuffled.");
    }
}