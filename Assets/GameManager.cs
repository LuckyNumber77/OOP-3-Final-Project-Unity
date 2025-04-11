using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // References to other components
    public DeckManager deckManager;
    public PlayerController player1Controller;
    public PlayerController player2Controller;
    public TMP_InputField player1Input;
    public TMP_InputField player2Input;
    public TMP_InputField player1BetInput;
    public TMP_InputField player2BetInput;
    public TextMeshProUGUI player1BalanceText;
    public TextMeshProUGUI player2BalanceText;
    public TextMeshProUGUI player1HandValueText;
    public TextMeshProUGUI player2HandValueText;
    public TextMeshProUGUI gameStatusText;
    public Button btnNewGame;
    public Button btnDealCards;
    public Transform dealerCardArea;
    public TextMeshProUGUI dealerHandValueText;

    // Player and Dealer Data
    private PlayerData player1;
    private PlayerData player2;
    private List<DeckManager.CardData> player1Hand = new List<DeckManager.CardData>();
    private List<DeckManager.CardData> player2Hand = new List<DeckManager.CardData>();
    private List<DeckManager.CardData> dealerHand = new List<DeckManager.CardData>();

    private bool player1BetPlaced = false;
    private bool player2BetPlaced = false;
    private bool player1HasStood = false;
    private bool player2HasStood = false;

    void Start()
    {
        // Check if we're in the Start Menu or Game Play scene
        if (SceneManager.GetActiveScene().name == "Start Menu")
        {
            // In Start Menu scene, just set up the start game button
            if (btnNewGame != null)
                btnNewGame.onClick.AddListener(StartGame);
        }
        else // We're in the Game Play scene
        {
            // Get player names from PlayerPrefs if they exist
            string player1Name = PlayerPrefs.GetString("Player1Name", "Player 1");
            string player2Name = PlayerPrefs.GetString("Player2Name", "Player 2");

            // Initialize with the player names
            player1 = new PlayerData(player1Name);
            player2 = new PlayerData(player2Name);

            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        SetupButtons();
        UpdateBalanceUI();
        player1Controller.SetActionButtons(false);
        player2Controller.SetActionButtons(false);
        gameStatusText.text = "Welcome to Blackjack! Place your bets to start.";

        // Allow both players to place bets
        player1Controller.EnableBettingOnly(true);
        player2Controller.EnableBettingOnly(true);
    }

    private void SetupButtons()
    {
        // Only set up these buttons in the Game Play scene
        if (btnDealCards != null)
            btnDealCards.onClick.AddListener(StartNewRound);

        if (player1Controller != null && player1Controller.btnHit != null)
            player1Controller.btnHit.onClick.AddListener(() => PlayerHit(1));

        if (player2Controller != null && player2Controller.btnHit != null)
            player2Controller.btnHit.onClick.AddListener(() => PlayerHit(2));

        if (player1Controller != null && player1Controller.btnStand != null)
            player1Controller.btnStand.onClick.AddListener(() => PlayerStand(1));

        if (player2Controller != null && player2Controller.btnStand != null)
            player2Controller.btnStand.onClick.AddListener(() => PlayerStand(2));
    }

    public void StartGame()
    {
        // Store player names in PlayerPrefs to persist between scenes
        if (player1Input != null)
            PlayerPrefs.SetString("Player1Name", player1Input.text.Length > 0 ? player1Input.text : "Player 1");

        if (player2Input != null)
            PlayerPrefs.SetString("Player2Name", player2Input.text.Length > 0 ? player2Input.text : "Player 2");

        // Now load the Game Play scene
        SceneManager.LoadScene("Game Play");
    }

    public void StartNewRound()
    {
        if (!player1BetPlaced || !player2BetPlaced)
        {
            gameStatusText.text = "Both players need to place bets first!";
            return;
        }

        ResetRound();
        DealInitialCards();
        UpdateUIForRoundStart();
    }

    private void ResetRound()
    {
        player1Hand.Clear();
        player2Hand.Clear();
        dealerHand.Clear();
        deckManager.ResetDeck();
        deckManager.ShuffleDeck();
        UpdateHandValueDisplay(1, 0);
        UpdateHandValueDisplay(2, 0);
        dealerHandValueText.text = "Dealer: ?";

        // Reset bet flags and update UI
        player1BetPlaced = false;
        player2BetPlaced = false;
        player1HasStood = false;
        player2HasStood = false;
    }

    private void DealInitialCards()
    {
        for (int i = 0; i < 2; i++)
        {
            player1Hand.Add(deckManager.DealCardToPlayer(1));
            player2Hand.Add(deckManager.DealCardToPlayer(2));
        }

        UpdateHandValueDisplay(1, CalculateHandValue(player1Hand));
        UpdateHandValueDisplay(2, CalculateHandValue(player2Hand));
    }

    private void UpdateUIForRoundStart()
    {
        player1Controller.SetActionButtons(true);
        player2Controller.SetActionButtons(false);
        gameStatusText.text = "Player 1's Turn";
    }

    public void PlayerPlacedBet(int playerNumber, int betAmount)
    {
        if (playerNumber == 1)
            player1BetPlaced = true;
        else
            player2BetPlaced = true;

        UpdateGameStatusAfterBets();
    }

    private void UpdateGameStatusAfterBets()
    {
        if (player1BetPlaced && player2BetPlaced)
        {
            gameStatusText.text = "Both players placed bets. Ready to deal cards.";
        }
        else if (player1BetPlaced)
        {
            gameStatusText.text = "Player 1 bet placed. Waiting for Player 2.";
        }
        else
        {
            gameStatusText.text = "Player 2 bet placed. Waiting for Player 1.";
        }
    }

    public void PlayerHit(int playerNumber)
    {
        List<DeckManager.CardData> currentHand = playerNumber == 1 ? player1Hand : player2Hand;
        DeckManager.CardData newCard = deckManager.DealCardToPlayer(playerNumber);
        currentHand.Add(newCard);

        int handValue = CalculateHandValue(currentHand);
        UpdateHandValueDisplay(playerNumber, handValue);

        if (handValue > 21)
        {
            HandlePlayerBust(playerNumber);
        }
    }

    private void HandlePlayerBust(int playerNumber)
    {
        gameStatusText.text = $"Player {playerNumber} Busted!";
        player1Controller.SetActionButtons(playerNumber == 2);
        player2Controller.SetActionButtons(playerNumber == 1);

        CheckIfRoundShouldEnd();
    }

    public void PlayerStand(int playerNumber)
    {
        if (playerNumber == 1)
        {
            player1HasStood = true;
            player1Controller.SetActionButtons(false);
        }
        else
        {
            player2HasStood = true;
            player2Controller.SetActionButtons(false);
        }

        gameStatusText.text = $"Player {playerNumber} stands.";

        CheckIfRoundShouldEnd();
    }

    private void CheckIfRoundShouldEnd()
    {
        if (player1HasStood && player2HasStood)
        {
            EndRound();
        }
        else if (player1HasStood)
        {
            gameStatusText.text = "Player 2's Turn";
            player2Controller.SetActionButtons(true);
        }
        else if (player2HasStood)
        {
            gameStatusText.text = "Player 1's Turn";
            player1Controller.SetActionButtons(true);
        }
    }

    private void UpdateHandValueDisplay(int playerNumber, int value)
    {
        if (playerNumber == 1)
        {
            player1HandValueText.text = $"Hand: {value}";
            player1Controller.UpdateHandValue(value);
        }
        else
        {
            player2HandValueText.text = $"Hand: {value}";
            player2Controller.UpdateHandValue(value);
        }
    }

    private int CalculateHandValue(List<DeckManager.CardData> hand)
    {
        int total = 0;
        int aceCount = 0;

        foreach (var card in hand)
        {
            total += card.value;
            if (card.name.ToLower().StartsWith("ace")) aceCount++;
        }

        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    private void UpdateBalanceUI()
    {
        player1BalanceText.text = $"Balance: ${player1.balance}";
        player2BalanceText.text = $"Balance: ${player2.balance}";
        player1Controller.SetBalance(player1.balance);
        player2Controller.SetBalance(player2.balance);
    }

    // End of Round Logic
    public void EndRound()
    {
        int dealerValue = CalculateHandValue(dealerHand);
        int player1Value = CalculateHandValue(player1Hand);
        int player2Value = CalculateHandValue(player2Hand);

        string resultMessage = "Round over: ";

        // Check if the dealer busted
        if (dealerValue > 21)
        {
            resultMessage += "Dealer busts! ";
            resultMessage += DetermineWinner(player1Value, 21, 1); // Pass dealer value as 21 when the dealer busts
            resultMessage += DetermineWinner(player2Value, 21, 2); // Pass dealer value as 21 when the dealer busts
        }
        else
        {
            resultMessage += DetermineWinner(player1Value, dealerValue, 1);
            resultMessage += DetermineWinner(player2Value, dealerValue, 2);
        }

        gameStatusText.text = resultMessage;
    }

    private string DetermineWinner(int playerValue, int dealerValue, int playerNumber)
    {
        if (playerValue > 21)
            return $"Player {playerNumber} busts. ";
        else if (playerValue > dealerValue)
            return $"Player {playerNumber} wins. ";
        else if (playerValue == dealerValue)
            return $"Player {playerNumber} pushes. ";
        else
            return $"Player {playerNumber} loses. ";
    }
}