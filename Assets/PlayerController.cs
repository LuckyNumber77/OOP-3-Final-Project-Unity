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

    // Betting chip buttons
    public Button chip10Button;            // $10 chip button
    public Button chip25Button;            // $25 chip button 
    public Button chip100Button;           // $100 chip button

    // Buttons for actions
    public Button btnHit;                   // "Hit" button
    public Button btnStand;                 // "Stand" button
    public Button btnPlaceBet;              // "Place Bet" button
    public Button btnDeal;                  // "Deal" button  
    public Button btnClearBet;              // Optional "Clear Bet" button

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
        // Initialize balance and bet UI
        UpdateBalanceUI();
        UpdateBetDisplayUI();

        // Add listeners to action buttons
        if (btnHit != null) btnHit.onClick.AddListener(Hit);
        if (btnStand != null) btnStand.onClick.AddListener(Stand);
        if (btnPlaceBet != null) btnPlaceBet.onClick.AddListener(PlaceBet);
        if (btnDeal != null) btnDeal.onClick.AddListener(Deal);
        if (btnClearBet != null) btnClearBet.onClick.AddListener(ClearBet);

        // Add listeners to chip buttons if they exist
        if (chip10Button != null) chip10Button.onClick.AddListener(() => AddBet(10));
        if (chip25Button != null) chip25Button.onClick.AddListener(() => AddBet(25));
        if (chip100Button != null) chip100Button.onClick.AddListener(() => AddBet(100));

        // Initially disable action buttons until a bet is placed and the round begins
        SetActionButtons(false);

        // Initially disable the Deal button until a bet is placed
        if (btnDeal != null) btnDeal.interactable = false;

        // Initially disable the Place Bet button until a bet is added
        if (btnPlaceBet != null) btnPlaceBet.interactable = false;
    }

    /// <summary>
    /// Places the bet using the current accumulated bet amount
    /// </summary>
    void PlaceBet()
    {
        if (currentBet > 0)
        {
            gameManager.PlayerPlacedBet(playerNumber, currentBet);
            Debug.Log("Bet placed: $" + currentBet);

            // Disable the Place Bet button after placing a bet
            if (btnPlaceBet != null) btnPlaceBet.interactable = false;

            // Disable the chip buttons after placing a bet
            SetBettingEnabled(false);

            // Disable the Clear Bet button after placing a bet
            if (btnClearBet != null) btnClearBet.interactable = false;
        }
        else
        {
            Debug.Log("Cannot place a bet of $0");
        }
    }

    /// <summary>
    /// Starts the round by telling the GameManager to deal cards
    /// </summary>
    void Deal()
    {
        Debug.Log("Dealing cards for new round");
        gameManager.StartNewRound();

        // Disable the Deal button while round is in progress
        if (btnDeal != null) btnDeal.interactable = false;
    }

    /// <summary>
    /// Enables or disables the betting chips and related UI
    /// </summary>
    public void SetBettingEnabled(bool isEnabled)
    {
        // Enable/disable chip buttons
        if (chip10Button != null) chip10Button.interactable = isEnabled && balance >= 10;
        if (chip25Button != null) chip25Button.interactable = isEnabled && balance >= 25;
        if (chip100Button != null) chip100Button.interactable = isEnabled && balance >= 100;

        // Only enable the Clear Bet button if betting is enabled and there's a current bet
        if (btnClearBet != null) btnClearBet.interactable = isEnabled && currentBet > 0;
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

            // Enable the Place Bet button when there's a valid bet amount
            if (btnPlaceBet != null) btnPlaceBet.interactable = true;

            // Enable the Clear Bet button when there's a valid bet amount
            if (btnClearBet != null) btnClearBet.interactable = true;

            // Update chip buttons based on remaining balance
            if (chip10Button != null) chip10Button.interactable = balance >= 10;
            if (chip25Button != null) chip25Button.interactable = balance >= 25;
            if (chip100Button != null) chip100Button.interactable = balance >= 100;
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

        // Re-enable betting mechanisms
        SetBettingEnabled(true);

        // Reset button states
        if (btnPlaceBet != null) btnPlaceBet.interactable = false;
        if (btnDeal != null) btnDeal.interactable = false;
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

        // Update chip buttons based on new balance
        if (chip10Button != null) chip10Button.interactable = balance >= 10;
        if (chip25Button != null) chip25Button.interactable = balance >= 25;
        if (chip100Button != null) chip100Button.interactable = balance >= 100;
    }

    /// <summary>
    /// Returns the player's current balance.
    /// </summary>
    public int GetBalance()
    {
        return balance;
    }

    /// <summary>
    /// Returns the current bet amount.
    /// </summary>
    public int GetCurrentBet()
    {
        return currentBet;
    }

    /// <summary>
    /// Clears the current bet and refunds the amount to the balance.
    /// </summary>
    public void ClearBet()
    {
        balance += currentBet;
        currentBet = 0;
        UpdateBalanceUI();
        UpdateBetDisplayUI();

        // Reset button states
        if (btnPlaceBet != null) btnPlaceBet.interactable = false;
        if (btnClearBet != null) btnClearBet.interactable = false;

        // Update chip buttons based on new balance
        if (chip10Button != null) chip10Button.interactable = balance >= 10;
        if (chip25Button != null) chip25Button.interactable = balance >= 25;
        if (chip100Button != null) chip100Button.interactable = balance >= 100;
    }
}