using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Deck & Dealer")]
    public DeckManager deckManager;                  // Manages the deck, deals cards
    public Transform dealerCardArea;                 // Where dealer's cards spawn
    public TextMeshProUGUI dealerHandValueText;      // Displays dealer's hand value

    [Header("Player 1 UI References")]
    public PlayerController player1Controller;       // Single-player controller for Player 1
    public TMP_InputField player1Input;              // Name input (from Start Menu or in-scene)
    public TMP_InputField player1BetInput;           // Bet input field (if used)
    public TextMeshProUGUI player1BalanceText;       // Displays player's balance
    public TextMeshProUGUI player1HandValueText;     // Displays player's hand value

    [Header("Game Status & Buttons")]
    public TextMeshProUGUI gameStatusText;           // Displays messages such as "Place your bet", "Bust!", etc.
    public Button btnNewGame;                        // Used in Start Menu scene to start game
    public Button btnDealCards;                      // Deal button to start a round
    public Button btnRestart;                        // Restart button for game over
    public Button btnConfirmName;                    // Button to confirm name at game start

    [Header("Settings")]
    public float delayBetweenRounds = 2.0f;          // Time to wait before clearing cards after a round

    // Player & Dealer data
    private PlayerData player1;
    private List<DeckManager.CardData> player1Hand = new List<DeckManager.CardData>();
    private List<DeckManager.CardData> dealerHand = new List<DeckManager.CardData>();

    private bool playerBetPlaced = false;
    private bool playerHasStood = false;
    private bool nameConfirmed = false;
    private bool roundInProgress = false;

    void Start()
    {
        // In Start Menu scene, set up "New Game" button
        if (SceneManager.GetActiveScene().name == "Start Menu")
        {
            if (btnNewGame != null)
                btnNewGame.onClick.AddListener(StartGame);
        }
        else
        {
            // In Game Play scene, retrieve the stored player name and initialize
            string storedName = PlayerPrefs.GetString("Player1Name", "");

            if (string.IsNullOrEmpty(storedName))
            {
                // Show name input prompt
                gameStatusText.text = "Please enter your name and confirm to start";
                nameConfirmed = false;

                // Set up confirm name button
                if (btnConfirmName != null)
                {
                    btnConfirmName.gameObject.SetActive(true);
                    btnConfirmName.onClick.AddListener(ConfirmPlayerName);
                }

                // Disable betting until name is confirmed
                player1Controller.SetBettingEnabled(false);
            }
            else
            {
                // Name already exists, proceed with game
                player1 = new PlayerData(storedName);
                nameConfirmed = true;
                InitializeGame();

                // Hide name confirmation button if it exists
                if (btnConfirmName != null)
                    btnConfirmName.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Confirms the player's name entered in the input field
    /// </summary>
    public void ConfirmPlayerName()
    {
        if (player1Input != null && !string.IsNullOrEmpty(player1Input.text))
        {
            string playerName = player1Input.text;
            PlayerPrefs.SetString("Player1Name", playerName);
            player1 = new PlayerData(playerName);
            nameConfirmed = true;

            // Hide name confirmation button
            if (btnConfirmName != null)
                btnConfirmName.gameObject.SetActive(false);

            // Enable betting now that name is confirmed
            player1Controller.SetBettingEnabled(true);

            InitializeGame();
        }
        else
        {
            gameStatusText.text = "Please enter a valid name to continue";
        }
    }

    /// <summary>
    /// Called in the Start Menu when "New Game" is pressed.
    /// Stores player name and loads the Game Play scene.
    /// </summary>
    public void StartGame()
    {
        if (player1Input != null && player1Input.text.Length > 0)
            PlayerPrefs.SetString("Player1Name", player1Input.text);
        else
            PlayerPrefs.SetString("Player1Name", "Player 1");

        SceneManager.LoadScene("Game Play");
    }

    /// <summary>
    /// Initializes the game in the Game Play scene.
    /// Sets up deck, UI, and clears any previous hands.
    /// </summary>
    private void InitializeGame()
    {
        // Update the PlayerController's name input field if available
        if (player1Controller != null && player1 != null)
        {
            if (player1Controller.playerNameInput != null)
                player1Controller.playerNameInput.text = player1.playerName;
        }

        // Initialize and shuffle deck; clear hands
        deckManager.ResetDeck();
        deckManager.ShuffleDeck();

        player1Hand.Clear();
        dealerHand.Clear();

        // Hook up the Deal button if available
        if (btnDealCards != null)
        {
            btnDealCards.onClick.RemoveAllListeners(); // Remove any existing listeners
            btnDealCards.onClick.AddListener(StartNewRound);
            btnDealCards.interactable = false; // Disabled until bet is placed
        }

        // Hook up the Restart button if available
        if (btnRestart != null)
        {
            btnRestart.onClick.RemoveAllListeners();
            btnRestart.onClick.AddListener(RestartGame);
            btnRestart.gameObject.SetActive(false);
        }

        // Show welcome message
        gameStatusText.text = "Welcome to Blackjack! Place your bet to start.";

        // Reset dealer and player hand value displays
        if (dealerHandValueText != null)
            dealerHandValueText.text = "Dealer Hand: 0";
        if (player1HandValueText != null)
            player1HandValueText.text = "Hand: 0";

        UpdateBalanceUI();
        playerBetPlaced = false;
        playerHasStood = false;
        roundInProgress = false;
    }

    /// <summary>
    /// Called by the PlayerController when a bet is placed.
    /// For single-player, this simply marks that a bet is in.
    /// </summary>
    public void PlayerPlacedBet(int playerNumber, int betAmount)
    {
        // Ensure this is for Player 1 (in a single-player game, playerNumber must be 1)
        if (playerNumber == 1 && nameConfirmed && !roundInProgress)
        {
            playerBetPlaced = true;
            gameStatusText.text = "Bet placed. Click Deal to begin!";

            // Enable the Deal button now that bet is placed
            if (btnDealCards != null)
                btnDealCards.interactable = true;
        }
    }

    /// <summary>
    /// Starts a new round by checking that a bet is placed, then dealing cards and updating UI.
    /// </summary>
    public void StartNewRound()
    {
        if (!playerBetPlaced)
        {
            gameStatusText.text = "You must place a bet first!";
            return;
        }

        roundInProgress = true;
        ResetRound();
        DealInitialCards();
        UpdateUIForRoundStart();

        // Disable the Deal button while round is in progress
        if (btnDealCards != null)
            btnDealCards.interactable = false;
    }

    /// <summary>
    /// Resets the round state: clears hands, resets deck, and updates UI.
    /// </summary>
    private void ResetRound()
    {
        player1Hand.Clear();
        dealerHand.Clear();

        deckManager.ResetDeck();
        deckManager.ShuffleDeck();

        playerHasStood = false;

        if (player1HandValueText)
            player1HandValueText.text = "Hand: 0";
        if (dealerHandValueText)
            dealerHandValueText.text = "Dealer Hand: 0";
    }

    /// <summary>
    /// Deals the initial cards: two to player and two to dealer (one hidden).
    /// Updates hand values on the UI.
    /// </summary>
    private void DealInitialCards()
    {
        // Deal two cards to the player
        player1Hand.Add(deckManager.DealCardToPlayer(1));
        player1Hand.Add(deckManager.DealCardToPlayer(1));

        // Deal two cards to the dealer
        dealerHand.Add(deckManager.DealCardToDealer(true));  // First card face up
        dealerHand.Add(deckManager.DealCardToDealer(false)); // Second card face down

        int pVal = CalculateHandValue(player1Hand);
        UpdateHandValueDisplay(pVal);

        // For the dealer, only show the first card's value and hide the second
        UpdateDealerHandValueDisplay(dealerHand[0].value, hidden: true);
    }

    /// <summary>
    /// Updates the UI for round start and enables player's Hit/Stand actions.
    /// </summary>
    private void UpdateUIForRoundStart()
    {
        gameStatusText.text = "Player 1's Turn: Hit or Stand";
        player1Controller.SetActionButtons(true);
    }

    /// <summary>
    /// Called by the PlayerController when the player hits.
    /// Deals one card to the player and updates the hand value.
    /// </summary>
    public void PlayerHit(int playerNumber)
    {
        if (playerNumber != 1 || !roundInProgress) return;

        DeckManager.CardData newCard = deckManager.DealCardToPlayer(1);
        player1Hand.Add(newCard);

        int pVal = CalculateHandValue(player1Hand);
        UpdateHandValueDisplay(pVal);

        if (pVal > 21)
        {
            gameStatusText.text = "Bust! Dealer wins!";
            EndRound(playerWin: false);
        }
    }

    /// <summary>
    /// Called by the PlayerController when the player stands.
    /// Proceeds to dealer's turn.
    /// </summary>
    public void PlayerStand(int playerNumber)
    {
        if (playerNumber != 1 || !roundInProgress) return;

        playerHasStood = true;
        player1Controller.SetActionButtons(false);
        DealerTurn();
    }

    /// <summary>
    /// Dealer's turn: reveal dealer's hand, draw until reaching 17, then determine outcome.
    /// </summary>
    private void DealerTurn()
    {
        // Reveal all dealer cards
        deckManager.RevealDealerCards();

        // Update dealer hand value display to show full value
        int dealerValue = CalculateHandValue(dealerHand);
        UpdateDealerHandValueDisplay(dealerValue);

        // Dealer draws cards until reaching 17 or higher
        while (dealerValue < 17)
        {
            dealerHand.Add(deckManager.DealCardToDealer(true)); // All subsequent cards are face up
            dealerValue = CalculateHandValue(dealerHand);
            UpdateDealerHandValueDisplay(dealerValue);
        }

        // Determine outcome based on final hand values
        int playerVal = CalculateHandValue(player1Hand);
        if (dealerValue > 21)
        {
            gameStatusText.text = "Dealer busts! Player 1 wins!";
            EndRound(playerWin: true);
        }
        else if (playerVal > dealerValue)
        {
            gameStatusText.text = "Player 1 wins!";
            EndRound(playerWin: true);
        }
        else if (playerVal < dealerValue)
        {
            gameStatusText.text = "Dealer wins!";
            EndRound(playerWin: false);
        }
        else
        {
            gameStatusText.text = "It's a tie!";
            EndRound(playerWin: null);
        }
    }

    /// <summary>
    /// Updates the dealer's hand value display.
    /// </summary>
    private void UpdateDealerHandValueDisplay(int handValue, bool hidden = false)
    {
        if (dealerHandValueText)
        {
            if (hidden)
                dealerHandValueText.text = "Dealer Hand: " + handValue + " + ?";
            else
                dealerHandValueText.text = "Dealer Hand: " + handValue;
        }
    }

    /// <summary>
    /// Ends the round: updates balance, checks game over, and updates UI.
    /// </summary>
    /// <param name="playerWin">
    /// true if player wins, false if dealer wins, null if tie.
    /// </param>
    private void EndRound(bool? playerWin)
    {
        // Get the current bet amount from player controller
        int betAmount = player1Controller.GetCurrentBet();

        Debug.Log("End of round - Bet amount: " + betAmount + ", Original balance: " + player1.balance);

        // Update player balance based on outcome
        if (playerWin == true)
        {
            // Player wins - give back the bet amount (which was already subtracted) plus winnings
            player1.balance += betAmount * 2;
            Debug.Log("Player won! Adding " + (betAmount * 2) + " to balance. New balance: " + player1.balance);
        }
        else if (playerWin == false)
        {
            // Player loses - bet was already deducted when placed
            Debug.Log("Player lost! Bet of " + betAmount + " is forfeit. Balance remains: " + player1.balance);
        }
        else
        {
            // Tie - return the bet amount
            player1.balance += betAmount;
            Debug.Log("It's a tie! Returning bet of " + betAmount + ". New balance: " + player1.balance);
        }

        UpdateBalanceUI();

        if (player1.balance <= 0)
        {
            gameStatusText.text += "\nYou are out of money. Game Over!";
            if (btnRestart != null)
                btnRestart.gameObject.SetActive(true);
        }
        else
        {
            gameStatusText.text += "\nRound over. Place a new bet for the next round.";
        }

        // Wait for a moment before clearing the table
        StartCoroutine(DelayedRoundEnd());
    }

    /// <summary>
    /// Coroutine to delay clearing the table after round end
    /// </summary>
    private IEnumerator DelayedRoundEnd()
    {
        yield return new WaitForSeconds(delayBetweenRounds);

        // Clear the cards from the table
        deckManager.ResetDeck();

        // Reset player controller for next round
        player1Controller.ResetForNewRound();

        // Reset round state
        playerBetPlaced = false;
        roundInProgress = false;

        // Reset hand value displays
        if (player1HandValueText)
            player1HandValueText.text = "Hand: 0";
        if (dealerHandValueText)
            dealerHandValueText.text = "Dealer Hand: 0";
    }

    /// <summary>
    /// Updates the balance text using the PlayerData balance.
    /// </summary>
    private void UpdateBalanceUI()
    {
        if (player1BalanceText != null)
            player1BalanceText.text = "Balance: $" + player1.balance;

        // Also update the balance in the PlayerController
        if (player1Controller != null)
            player1Controller.SetBalance(player1.balance);
    }

    /// <summary>
    /// Calculates the hand value, treating Aces as 11 or 1 appropriately.
    /// </summary>
    private int CalculateHandValue(List<DeckManager.CardData> hand)
    {
        int total = 0;
        int aceCount = 0;

        foreach (var card in hand)
        {
            total += card.value;
            if (card.name.ToLower().Contains("ace") || card.name.ToLower().Contains("a_"))
                aceCount++;
        }

        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    /// <summary>
    /// Updates the player's hand value display.
    /// </summary>
    private void UpdateHandValueDisplay(int handValue)
    {
        if (player1HandValueText)
            player1HandValueText.text = "Hand: " + handValue;
    }

    /// <summary>
    /// Called by the Restart button to reset the game.
    /// </summary>
    public void RestartGame()
    {
        // Reset the player's balance to the default value.
        player1.balance = 1000;

        // Clear player name from PlayerPrefs
        PlayerPrefs.DeleteKey("Player1Name");

        if (btnRestart != null)
            btnRestart.gameObject.SetActive(false);

        StopAllCoroutines(); // Stop any pending coroutines

        ResetRound();
        UpdateBalanceUI();
        player1Controller.ResetForNewRound();

        playerBetPlaced = false;
        roundInProgress = false;

        gameStatusText.text = "Game restarted. Place your bet!";
    }
}