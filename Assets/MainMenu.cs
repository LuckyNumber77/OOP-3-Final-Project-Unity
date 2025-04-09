using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image cardImage;        // The Image component to display the card's sprite
    public Text cardNameText;      // Text component to display the card name (optional)
    public Text cardValueText;     // Text component to display the card value (optional)
    public string cardName;        // The name of the card (e.g., "Ace of Spades")
    public int cardValue;          // The numerical value of the card (e.g., 10 for King)

    // Setup method to initialize the card's UI
    public void Setup(Sprite image, string name, int value)
    {
        cardImage.sprite = image;  // Set the image to the card's sprite
        cardName = name;           // Set the name of the card (e.g., "Ace of Spades")
        cardValue = value;         // Set the numerical value of the card (e.g., 10, 11, etc.)

        if (cardNameText != null)
        {
            cardNameText.text = cardName;  // Display the card's name if a Text component exists
        }

        if (cardValueText != null)
        {
            cardValueText.text = cardValue.ToString();  // Display the card's value if a Text component exists
        }
    }

    // You can add more methods here for interactions, e.g., clicking the card
    public void OnCardClick()
    {
        // Handle card click event, like choosing to hit or stand in Blackjack
        Debug.Log($"Card clicked: {cardName}, Value: {cardValue}");
    }
}
