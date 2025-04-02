using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    [System.Serializable]
    public class CardData
    {
        public string name;
        public Sprite image;
        public int value;
    }

    [Header("Deck Setup")]
    public List<CardData> cardDeck = new List<CardData>();
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
        {
            Destroy(child.gameObject);
        }

        // Clear player 2's card area
        foreach (Transform child in player2CardArea)
        {
            Destroy(child.gameObject);
        }

        // Clear dealer's card area
        if (dealerCardArea != null)
        {
            foreach (Transform child in dealerCardArea)
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

                // If we have a full CardDisplay implementation with setup method
                if (display.GetType() == typeof(CardDisplay))
                {
                    display.Setup(cardDeck[currentCardIndex].image,
                                  cardDeck[currentCardIndex].name,
                                  cardDeck[currentCardIndex].value);
                }
            }
            else
            {
                Debug.LogWarning("CardDisplay script is missing on card prefab.");
            }

            currentCardIndex++;
        }
    }

    /// <summary>
    /// Deals a single card to a player and returns the card data
    /// </summary>
    public CardData DealCardToPlayer(int playerNumber)
    {
        if (playerNumber != 1 && playerNumber != 2)
        {
            Debug.LogWarning("Invalid player number passed to DealCardToPlayer.");
            return null;
        }

        Transform cardArea = (playerNumber == 1) ? player1CardArea : player2CardArea;

        if (currentCardIndex >= cardDeck.Count)
        {
            Debug.LogWarning("Out of cards in the deck!");
            return null;
        }

        // Get the current card
        CardData currentCard = cardDeck[currentCardIndex];

        // Create the visual card
        GameObject card = Instantiate(cardPrefab, cardArea);
        CardDisplay display = card.GetComponent<CardDisplay>();

        if (display != null)
        {
            display.cardImage.sprite = currentCard.image;

            // If we have a full CardDisplay implementation with setup method
            if (display is CardDisplay cardDisplay)
            {
                cardDisplay.Setup(currentCard.image, currentCard.name, currentCard.value);
            }
        }

        // Increment card index
        currentCardIndex++;

        return currentCard;
    }

    /// <summary>
    /// Deals a card to the dealer and returns the card data
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
            return null;
        }

        // Get the current card
        CardData currentCard = cardDeck[currentCardIndex];

        // Create the visual card
        GameObject card = Instantiate(cardPrefab, dealerCardArea);
        CardDisplay display = card.GetComponent<CardDisplay>();

        if (display != null)
        {
            display.cardImage.sprite = currentCard.image;

            // If we have a full CardDisplay implementation with setup method
            if (display is CardDisplay cardDisplay)
            {
                cardDisplay.Setup(currentCard.image, currentCard.name, currentCard.value);
            }
        }

        // Increment card index
        currentCardIndex++;

        return currentCard;
    }

    /// <summary>
    /// Returns the last card that was dealt from the deck
    /// </summary>
    public CardData DealCardToPlayer(int playerNumber)
    {
        if (playerNumber != 1 && playerNumber != 2)
        {
            Debug.LogWarning("Invalid player number passed to DealCardToPlayer.");
            return null;
        }

        Transform cardArea = (playerNumber == 1) ? player1CardArea : player2CardArea;

        if (currentCardIndex >= cardDeck.Count)
        {
            Debug.LogWarning("Out of cards in the deck!");
            HandleDeckExhaustion();  // Call to handle when the deck is exhausted
            return null;
        }

        // Get the current card
        CardData currentCard = cardDeck[currentCardIndex];

        // Create the visual card
        GameObject card = Instantiate(cardPrefab, cardArea);
        CardDisplay display = card.GetComponent<CardDisplay>();

        if (display != null)
        {
            display.cardImage.sprite = currentCard.image;

            // If we have a full CardDisplay implementation with setup method
            if (display is CardDisplay cardDisplay)
            {
                cardDisplay.Setup(currentCard.image, currentCard.name, currentCard.value);
            }
        }

        // Increment card index
        currentCardIndex++;

        return currentCard;
    }


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