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

    // Changed from SpriteRenderer to Image since we're using UI elements

    public void Setup(Sprite image, string name, int value)
    {
        if (cardImage == null)
        {
            Debug.LogError("Card image is null on " + gameObject.name);
            return;
        }

        cardImage.sprite = image;

        if (cardName != null)
            cardName.text = name;

        if (cardValue != null)
            cardValue.text = value.ToString();

        // Make sure the card face is showing (not the back)
        ShowCardFace(true);

        Debug.Log("Card setup complete: " + name + " with value " + value);
    }

    // New method to show/hide card face
    public void ShowCardFace(bool showFace)
    {
        if (cardImage == null)
        {
            Debug.LogError("Card image is null on " + gameObject.name);
            return;
        }

        if (showFace)
        {
            // If we have a valid index and faces array, show that face
            if (faces != null && faces.Length > 0 && cardIndex >= 0 && cardIndex < faces.Length)
            {
                cardImage.sprite = faces[cardIndex];
            }
            // Otherwise use the sprite assigned during Setup
        }
        else
        {
            // Show card back
            if (cardBack != null)
            {
                cardImage.sprite = cardBack;
            }
        }
    }

    // For backward compatibility with any existing code
    public void Toggleface(bool surface)
    {
        ShowCardFace(surface);
    }
}