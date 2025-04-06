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

    private SpriteRenderer spriteRenderer;  // Declare spriteRenderer



    public void Setup(Sprite image, string name, int value)
    {
        cardImage.sprite = image;
        cardName.text = name;
        cardValue.text = value.ToString();  // Assuming you want to display the value as text
    }
    // Called to toggle between showing face and back of the card
    public void Toggleface(bool surface)
    {
        if (surface)  // If surface is true, show the card face
        {
            spriteRenderer.sprite = faces[cardIndex];
        }
        else  // Otherwise, show the card back
        {
            spriteRenderer.sprite = cardBack;
        }
    }

    // Initialize spriteRenderer
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  // Corrected: fixed typo in component name ("SpriteRenderer" instead of "SpriteRendered")
    }
}
