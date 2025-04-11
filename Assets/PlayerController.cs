using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // UI Components
    public TMP_InputField playerNameInput;
    public TMP_InputField betAmountInput;
    public TMP_Text balanceText;
    public TMP_Text handValueText;

    // Buttons for actions
    public Button btnPlaceBet;
    public Button btnHit;
    public Button btnStand;

    // Reference to GameManager; for single-player this controller always handles "Player 1"
    public GameManager gameManager;
    private int playerNumber = 1; // Always use player number 1

    // Player's Balance and Bet Information
    private int balance = 1000;
    private int currentBet;

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>(); // Find GameManager if not set
        }

        // Force this controller to be for player 1
        playerNumber = 1;
        InitializePlayer();
    }

    void InitializePlayer()
    {
        // Update balance text
        UpdateBalanceText();

        // Add listeners to buttons
        btnPlaceBet.onClick.AddListener(PlaceBet);
        btnHit.onClick.AddListener(Hit);
        btnStand.onClick.AddListener(Stand);

        // Set initial button states: enable only betting, disable actions until a bet is placed
        EnableBettingOnly(true);
    }

    // Update the player's balance UI
    void UpdateBalanceText()
    {
        balanceText.text = "Balance\n$" + balance.ToString();
    }

    // Return current bet value
    public int GetCurrentBet()
    {
        return currentBet;
    }

    // Set the current bet
    public void SetBet(int betAmount)
    {
        currentBet = betAmount;
    }

    // Place a bet and handle UI updates
    void PlaceBet()
    {
        if (int.TryParse(betAmountInput.text, out int betAmount))
        {
            if (betAmount > 0 && betAmount <= balance)
            {
                balance -= betAmount;
                UpdateBalanceText();
                Debug.Log(playerNameInput.text + " placed a bet of $" + betAmount);

                // Notify the GameManager that player 1 has placed a bet
                gameManager.PlayerPlacedBet(playerNumber, betAmount);

                // Disable the betting button and enable action buttons
                EnableBettingOnly(false);
                SetActionButtons(true);
            }
            else
            {
                Debug.LogWarning("Invalid bet amount! Bet should be between 1 and " + balance);
            }
        }
        else
        {
            Debug.LogWarning("Please enter a valid number for the bet amount!");
        }
    }

    // Player chooses to Hit
    void Hit()
    {
        Debug.Log(playerNameInput.text + " chose to HIT!");

        // Delegate to GameManager to handle dealing a card to player 1
        gameManager.PlayerHit(playerNumber);
    }

    // Player chooses to Stand
    void Stand()
    {
        Debug.Log(playerNameInput.text + " chose to STAND!");

        // Disable action buttons and notify GameManager that player 1 stands
        SetActionButtons(false);
        gameManager.PlayerStand(playerNumber);
    }

    // Enable/Disable action buttons (Hit & Stand)
    public void SetActionButtons(bool isEnabled)
    {
        btnHit.interactable = isEnabled;
        btnStand.interactable = isEnabled;
    }

    // Enable only the betting button, disabling action buttons (Hit & Stand)
    public void EnableBettingOnly(bool isEnabled)
    {
        btnPlaceBet.interactable = isEnabled;
        btnHit.interactable = false;
        btnStand.interactable = false;
    }

    // Update the hand value text when the player's hand changes
    public void UpdateHandValue(int value)
    {
        handValueText.text = "Hand: " + value.ToString();
    }

    // Update player's balance from GameManager (in case of win/loss)
    public void SetBalance(int newBalance)
    {
        balance = newBalance;
        UpdateBalanceText();
    }

    // Get the current balance
    public int GetBalance()
    {
        return balance;
    }

    // Reset player for a new round: clear bet input, reset hand display, and enable betting only
    public void ResetForNewRound()
    {
        betAmountInput.text = "";
        handValueText.text = "Hand: 0";
        EnableBettingOnly(true);
        SetActionButtons(false);
    }
}
