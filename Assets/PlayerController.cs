using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TMP_InputField betAmountInput;
    public TMP_Text balanceText;

    public Button btnPlaceBet;
    public Button btnHit;
    public Button btnStand;

    public GameManager gameManager; // Reference to GameManager
    public int playerNumber;        // 1 for Player1, 2 for Player2

    private int balance = 1000;

    void Start()
    {
        UpdateBalanceText();

        btnPlaceBet.onClick.AddListener(PlaceBet);
        btnHit.onClick.AddListener(Hit);
        btnStand.onClick.AddListener(Stand);
    }

    void UpdateBalanceText()
    {
        balanceText.text = "Balance\n$" + balance.ToString();
    }

    void PlaceBet()
    {
        if (int.TryParse(betAmountInput.text, out int betAmount))
        {
            if (betAmount > 0 && betAmount <= balance)
            {
                balance -= betAmount;
                UpdateBalanceText();
                Debug.Log(playerNameInput.text + " placed a bet of $" + betAmount);
            }
            else
            {
                Debug.LogWarning("Invalid bet amount!");
            }
        }
        else
        {
            Debug.LogWarning("Please enter a valid number!");
        }
    }

    void Hit()
    {
        Debug.Log(playerNameInput.text + " chose to HIT!");
    }

    void Stand()
    {
        Debug.Log(playerNameInput.text + " chose to STAND!");

        // Disable action buttons
        SetActionButtons(false);

        // Notify GameManager
        if (gameManager != null)
        {
            gameManager.PlayerStand(playerNumber);
        }
    }

    public void SetActionButtons(bool isEnabled)
    {
        btnHit.interactable = isEnabled;
        btnStand.interactable = isEnabled;
        btnPlaceBet.interactable = isEnabled;
    }
}
