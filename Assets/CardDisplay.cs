using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;

    public Text cardName;
    public Text cardValue;

    public Sprite[] faces;  // Array to hold card faces
    public Sprite cardBack; // Card back sprite

    public int cardIndex;   // Index to represent the current card face

    private string cardNameStr;
    private int cardValueInt;
    private Sprite cardFaceSprite;

    public void Setup(Sprite image, string name, int value)
    {
        if (cardImage == null)
        {
            Debug.LogError("Card image is null on " + gameObject.name);
            return;
        }

        // Store the card data
        cardNameStr = name;
        cardValueInt = value;
        cardFaceSprite = image;  // Store the directly passed sprite

        // Set the image directly from the parameter
        cardImage.sprite = image;

        Debug.Log("Setting up card: " + name + " with value " + value + ", sprite: " + (image != null ? image.name : "null"));

        if (cardName != null)
            cardName.text = name;

        if (cardValue != null)
            cardValue.text = value.ToString();

        // Set a meaningful name for debugging
        gameObject.name = "Card_" + name;
    }

    public void ShowCardFace(bool showFace)
    {
        if (cardImage == null)
        {
            Debug.LogError("Card image is null on " + gameObject.name);
            return;
        }

        if (showFace)
        {
            // First priority: use the stored face sprite that was passed in Setup
            if (cardFaceSprite != null)
            {
                cardImage.sprite = cardFaceSprite;
                Debug.Log("Showing face for card: " + cardNameStr + " using stored sprite: " + cardFaceSprite.name);
            }
            // Fallback: if no stored sprite but we have faces array and valid index, use that
            else if (faces != null && faces.Length > 0 && cardIndex >= 0 && cardIndex < faces.Length)
            {
                cardImage.sprite = faces[cardIndex];
                Debug.Log("Showing face for card: " + cardNameStr + " using faces array index: " + cardIndex);
            }
            else
            {
                Debug.LogError("No valid face sprite found for " + cardNameStr);
            }
        }
        else
        {
            // Show card back
            if (cardBack != null)
            {
                cardImage.sprite = cardBack;
                Debug.Log("Showing back for card: " + cardNameStr);
            }
            else
            {
                Debug.LogError("Card back sprite is null");
            }
        }
    }

    // For backward compatibility
    public void Toggleface(bool surface)
    {
        ShowCardFace(surface);
    }

    // For debugging
    void OnEnable()
    {
        // Log when this card is enabled
        if (!string.IsNullOrEmpty(cardNameStr))
            Debug.Log("Card enabled: " + cardNameStr);
    }
}