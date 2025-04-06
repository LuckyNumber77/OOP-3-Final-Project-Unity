using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TMP_InputField betAmountInput;
    public TMP_Text balanceText;
    public TMP_Text handValueText;  // Added to display hand value

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
    private int currentBet;

    public int GetCurrentBet()
    {
        return currentBet;
    }

    public void SetBet(int betAmount)
    {
        currentBet = betAmount;
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

                // Notify GameManager that bet has been placed
                if (gameManager != null)
                {
                    gameManager.PlayerPlacedBet(playerNumber, betAmount);
                }

                // Disable betting and enable action buttons
                btnPlaceBet.interactable = false;
                btnHit.interactable = true;
                btnStand.interactable = true;
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

        // Call the GameManager to handle dealing a card to this player
        if (gameManager != null)
        {
            gameManager.PlayerHit(playerNumber);
        }
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
    }

    // Added for better control over button states
    public void EnableBettingOnly(bool isEnabled)
    {
        btnPlaceBet.interactable = isEnabled;
        btnHit.interactable = false;
        btnStand.interactable = false;
    }

    // Set player's hand value display
    public void UpdateHandValue(int value)
    {
        if (handValueText != null)
        {
            handValueText.text = "Hand: " + value.ToString();
        }
    }

    // Method to update balance from GameManager
    public void SetBalance(int newBalance)
    {
        balance = newBalance;
        UpdateBalanceText();
    }

    // Method to get current balance
    public int GetBalance()
    {
        return balance;
    }

    // Method to get current bet
    public int GetBet()
    {
        if (int.TryParse(betAmountInput.text, out int bet))
        {
            return bet;
        }
        return 0;
    }

    // Reset for new round
    public void ResetForNewRound()
    {
        // Clear bet input
        betAmountInput.text = "";

        // Enable only betting
        EnableBettingOnly(true);
    }
}