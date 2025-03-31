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
    public GameObject cardUIPrefab;

    [Header("Card Display Areas")]
    public Transform player1CardArea;
    public Transform player2CardArea;

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
            GameObject card = Instantiate(cardUIPrefab, cardArea);
            CardDisplay display = card.GetComponent<CardDisplay>();

            if (display != null)
            {
                display.cardImage.sprite = cardDeck[currentCardIndex].image;
            }
            else
            {
                Debug.LogWarning("CardDisplay script is missing on card prefab.");
            }

            currentCardIndex++;
        }
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

