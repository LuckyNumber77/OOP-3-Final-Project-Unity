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

        // Clear player 1's card area
        foreach (Transform child in player1CardArea)
            Destroy(child.gameObject);

        // Clear player 2's card area
        foreach (Transform child in player2CardArea)
            Destroy(child.gameObject);

        // Clear dealer's card area
        if (dealerCardArea != null)
        {
            foreach (Transform child in dealerCardArea)
                Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Deals a specified number of cards to the given player (1 or 2).
    /// </summary>
    public void DealCardsToPlayer(int playerNumber, int cardCount)
    {
        if (playerNumber != 1 && playerNumber != 2)
        {
            Debug.LogWarning("Invalid player number passed to DealCardsToPlayer.");
            return;
        }

        Transform cardArea = (playerNumber == 1) ? player1CardArea : player2CardArea;

        for (int i = 0; i < cardCount && currentCardIndex < cardDeck.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardArea);
            CardDisplay display = card.GetComponent<CardDisplay>();

            if (display != null)
            {
                display.cardImage.sprite = cardDeck[currentCardIndex].image;
                display.Setup(cardDeck[currentCardIndex].image, cardDeck[currentCardIndex].name, cardDeck[currentCardIndex].value);
            }
            else
            {
                Debug.LogWarning("CardDisplay script is missing on card prefab.");
            }

            currentCardIndex++;
        }
    }

    /// <summary>
    /// Deals a single card to a player and returns the card data.
    /// </summary>
    public CardData DealCardToPlayer(int playerNumber)
    {
        if (playerNumber != 1 && playerNumber != 2)
        {
            Debug.LogWarning("Invalid player number passed to DealCardToPlayer.");
            return null;
        }

        if (currentCardIndex >= cardDeck.Count)
        {
            Debug.LogWarning("Out of cards in the deck!");
            HandleDeckExhaustion();
            return null;
        }

        Transform cardArea = (playerNumber == 1) ? player1CardArea : player2CardArea;

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
            Debug.LogWarning("Out of cards in the deck!");
            HandleDeckExhaustion();
            return null;
        }

        CardData currentCard = cardDeck[currentCardIndex];
        GameObject card = Instantiate(cardPrefab, dealerCardArea);
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
        Debug.LogError("Deck is exhausted! Consider reshuffling or ending game.");
        ShuffleDeck();
        currentCardIndex = 0;
        // Optional: Trigger UI/logic for reshuffling, ending round, etc.
    }

=======
        Debug.Log("Handling deck exhaustion: Reshuffling the deck.");
        ShuffleDeck();
        currentCardIndex = 0; // Reset index after shuffling
    }

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
