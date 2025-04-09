using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;       // Image to display the card (UI element)

    public Text cardName;         // Text to display card name (e.g., "Ace of Spades")
    public Text cardValue;        // Text to display card value (e.g., 10, Ace, etc.)

    public Sprite[] faces;        // Array to hold all possible face images (Card faces)
    public Sprite cardBack;       // Image for the card's back (when not showing the face)

    public int cardIndex;         // Index to identify the current card face in the `faces` array

    // Method to set up the card details (image, name, and value)
    public void Setup(Sprite image, string name, int value)
    {
        cardImage.sprite = image;                 // Set card image
        cardName.text = name;                     // Set card name (e.g., "Ace of Spades")
        cardValue.text = value.ToString();        // Set card value (e.g., "10", "Ace", etc.)
    }

    // Method to toggle between showing the face and back of the card
    public void ToggleFace(bool isFaceUp)
    {
        if (isFaceUp)
        {
            // Show the face of the card (based on cardIndex)
            cardImage.sprite = faces[cardIndex];
        }
        else
        {
            // Show the back of the card
            cardImage.sprite = cardBack;
        }
    }
}
