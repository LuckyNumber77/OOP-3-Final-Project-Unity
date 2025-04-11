using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // UI Components
    public TMP_InputField playerNameInput;  // Used to capture the player's name at the start
    public TMP_Text balanceText;            // Label to display the player's current balance (e.g., "Balance: $1000")
    public TMP_Text betDisplayText;         // Label to display the current bet (e.g., "Bet: $0")
    public TMP_Text handValueText;          // Label to display the player's current hand value (e.g., "Hand: 0")

    // Buttons for actions
    public Button btnHit;                   // "Hit" button
    public Button btnStand;                 // "Stand" button

    // Reference to GameManager; for single-player, this controller always handles Player 1
    public GameManager gameManager;
    private int playerNumber = 1;           // Always use player number 1

    // Player's financial data
    private int balance = 1000;             // Starting balance
    private int currentBet = 0;             // Current bet amount

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>(); // Find GameManager if not already assigned

        playerNumber = 1; // Force this controller to always be Player 1
        InitializePlayer();
    }

    void InitializePlayer()
    {
        // Optionally, update the player's name display (if your PlayerController uses the input field)
        if (playerNameInput != null && playerNameInput.text != "")
        {
            // You might update a name label here, for example.
        }

        // Initialize balance and bet UI
        UpdateBalanceUI();
        UpdateBetDisplayUI();

        // Add listeners to action buttons
        if (btnHit != null) btnHit.onClick.AddListener(Hit);
        if (btnStand != null) btnStand.onClick.AddListener(Stand);

        // Initially disable action buttons until a bet is placed and the round begins
        SetActionButtons(false);
    }

    /// <summary>
    /// Called by chip buttons to add to the bet.
    /// For example, pressing the $10 chip calls AddBet(10) and the $100 chip calls AddBet(100).
    /// This method increases the current bet and decreases the balance accordingly.
    /// </summary>
    public void AddBet(int amount)
    {
        // Ensure there is enough balance
        if (balance >= amount)
        {
            currentBet += amount;
            balance -= amount;
            UpdateBetDisplayUI();
            UpdateBalanceUI();
            Debug.Log("Added bet: " + amount + ". Current bet: " + currentBet + ", Balance: " + balance);

            // Optionally, notify the GameManager that a bet has been updated.
            gameManager.PlayerPlacedBet(playerNumber, currentBet);
        }
        else
        {
            Debug.Log("Not enough funds to add that bet amount.");
        }
    }

    /// <summary>
    /// Updates the balance label.
    /// </summary>
    private void UpdateBalanceUI()
    {
        if (balanceText != null)
            balanceText.text = "Balance: $" + balance;
    }

    /// <summary>
    /// Updates the bet label.
    /// </summary>
    private void UpdateBetDisplayUI()
    {
        if (betDisplayText != null)
            betDisplayText.text = "Bet: $" + currentBet;
    }

    /// <summary>
    /// Called when the player chooses to hit.
    /// Delegates to the GameManager to deal a card.
    /// </summary>
    void Hit()
    {
        Debug.Log("Player chose to HIT!");
        gameManager.PlayerHit(playerNumber);
    }

    /// <summary>
    /// Called when the player chooses to stand.
    /// Disables the action buttons and notifies the GameManager.
    /// </summary>
    void Stand()
    {
        Debug.Log("Player chose to STAND!");
        SetActionButtons(false);
        gameManager.PlayerStand(playerNumber);
    }

    /// <summary>
    /// Enables or disables the action buttons (Hit and Stand).
    /// </summary>
    public void SetActionButtons(bool isEnabled)
    {
        if (btnHit != null) btnHit.interactable = isEnabled;
        if (btnStand != null) btnStand.interactable = isEnabled;
    }

    /// <summary>
    /// Resets the bet and hand values for a new round.
    /// </summary>
    public void ResetForNewRound()
    {
        currentBet = 0;
        UpdateBetDisplayUI();
        if (handValueText != null)
            handValueText.text = "Hand: 0";

        // You might also re-enable betting mechanisms here.
    }

    /// <summary>
    /// Updates the player's hand value label.
    /// </summary>
    public void UpdateHandValue(int value)
    {
        if (handValueText != null)
            handValueText.text = "Hand: " + value;
    }

    /// <summary>
    /// Sets the player's balance (useful if GameManager adjusts balance based on round result).
    /// </summary>
    public void SetBalance(int newBalance)
    {
        balance = newBalance;
        UpdateBalanceUI();
    }

    /// <summary>
    /// Returns the player's current balance.
    /// </summary>
    public int GetBalance()
    {
        return balance;
    }

    /// <summary>
    /// Optionally, clears the current bet and refunds the amount to the balance.
    /// </summary>
    public void ClearBet()
    {
        balance += currentBet;
        currentBet = 0;
        UpdateBalanceUI();
        UpdateBetDisplayUI();
    }
}
