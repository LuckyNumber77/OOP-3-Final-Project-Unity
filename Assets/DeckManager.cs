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

    [Header("Card Spacing")]
    public float cardSpacing = 100f; // Customizable card spacing

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

        Debug.Log("All card areas cleared.");
    }

    /// <summary>  
    /// Handles card area clearing.  
    /// </summary>  
    private void ClearCardArea(Transform cardArea)
    {
        if (cardArea != null)
        {
            int childCount = cardArea.childCount;

            // Destroy from last to first to avoid index shifting issues
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(cardArea.GetChild(i).gameObject);
            }

            Debug.Log($"Cleared {childCount} cards from {cardArea.name}");
        }
        else
        {
            Debug.LogWarning("Attempted to clear a null card area.");
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
        return DealCard(cardArea, true);
    }

    /// <summary>  
    /// Deals a card to the dealer and returns the card data.  
    /// </summary>  
    public CardData DealCardToDealer(bool faceUp = true)
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

        return DealCard(dealerCardArea, faceUp);
    }

    /// <summary>  
    /// Deals a card to the specified card area and returns the card data.  
    /// </summary>  
    private CardData DealCard(Transform cardArea, bool faceUp = true)
    {
        // Check if we've run out of cards
        if (currentCardIndex >= cardDeck.Count)
        {
            Debug.LogWarning("Deck is exhausted! Reshuffling...");
            ShuffleDeck();
            currentCardIndex = 0;
        }

        // Get the current card from the deck list
        CardData currentCard = cardDeck[currentCardIndex];

        // Check if the card area exists
        if (cardArea == null)
        {
            Debug.LogError("Card area is null. Cannot deal card.");
            return currentCard;
        }

        // Instantiate the card prefab as a child of the given card area
        GameObject card = Instantiate(cardPrefab, cardArea);

        if (card == null)
        {
            Debug.LogError("Failed to instantiate card prefab. Check if prefab is assigned.");
            return currentCard;
        }

        Debug.Log("Card dealt: " + currentCard.name + " with value " + currentCard.value);

        // Get the RectTransform component if this is a UI element.
        RectTransform cardRT = card.GetComponent<RectTransform>();
        if (cardRT != null)
        {
            // Use the current number of child objects in the cardArea to determine offset.
            // Increased spacing for better visibility
            int cardCount = cardArea.childCount - 1; // Subtract one if the new card is already counted
            cardRT.anchoredPosition = new Vector2(cardCount * cardSpacing, 0);

            Debug.Log("Card positioned at X offset: " + (cardCount * cardSpacing));
        }
        else
        {
            Debug.LogWarning("Card doesn't have a RectTransform component. Position may not be as expected.");
        }

        // Set up the card's display details using CardDisplay
        CardDisplay display = card.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.Setup(currentCard.image, currentCard.name, currentCard.value);

            // Show the card face up or down based on the parameter
            display.ShowCardFace(faceUp);

            // Set a meaningful name for debugging
            card.name = currentCard.name + (faceUp ? "_FaceUp" : "_FaceDown");
        }
        else
        {
            Debug.LogError("CardDisplay script is missing on the card prefab. Please add it to your card prefab.");
        }

        // Increment the index so the next card is dealt from the deck
        currentCardIndex++;
        return currentCard;
    }

    /// <summary>
    /// Flips over all dealer cards to face up (used at the end of a round)
    /// </summary>
    public void RevealDealerCards()
    {
        if (dealerCardArea != null)
        {
            foreach (Transform child in dealerCardArea)
            {
                CardDisplay display = child.GetComponent<CardDisplay>();
                if (display != null)
                {
                    display.ShowCardFace(true);
                    Debug.Log("Revealed dealer card: " + child.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("Dealer card area is not assigned. Cannot reveal cards.");
        }
    }

    /// <summary>  
    /// Handles logic when deck is exhausted.  
    /// </summary>  
    private void HandleDeckExhaustion()
    {
        Debug.LogError("Deck is exhausted! Reshuffling the deck.");
        ShuffleDeck();
        currentCardIndex = 0; // Reset index after shuffling
    }

    /// <summary>  
    /// Shuffles the deck randomly.  
    /// </summary>  
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