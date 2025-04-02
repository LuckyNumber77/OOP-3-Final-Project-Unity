using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;
    public string cardName;
    public int cardValue;

    public void Setup(Sprite image, string name, int value)
    {
        cardImage.sprite = image;
        cardName = name;
        cardValue = value;
    }
}