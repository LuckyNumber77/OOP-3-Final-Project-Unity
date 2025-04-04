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

    public void ResetDeck()
    {
        currentCardIndex = 0;

        foreach (Transform child in player1CardArea)
            Destroy(child.gameObject);

        foreach (Transform child in player2CardArea)
            Destroy(child.gameObject);

        if (dealerCardArea != null)
        {
            foreach (Transform child in dealerCardArea)
                Destroy(child.gameObject);
        }
    }

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
                display.Setup(cardDeck[currentCardIndex].image,
                              cardDeck[currentCardIndex].name,
                              cardDeck[currentCardIndex].value);
            }
            else
            {
                Debug.LogWarning("CardDisplay script is missing on card prefab.");
            }

            currentCardIndex++;
        }
    }

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

        currentCardIndex++;
        return currentCard;
    }

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

        CardData currentCard = cardDeck[currentCardIndex];
        GameObject card = Instantiate(cardPrefab, dealerCardArea);
        CardDisplay display = card.GetComponent<CardDisplay>();

        if (display != null)
        {
            display.cardImage.sprite = currentCard.image;
            display.Setup(currentCard.image, currentCard.name, currentCard.value);
        }

        currentCardIndex++;
        return currentCard;
    }

    {
        Debug.LogError("Deck is exhausted! Consider reshuffling or ending game.");
        // Optional: Trigger UI/logic for reshuffling, ending round, etc.
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
