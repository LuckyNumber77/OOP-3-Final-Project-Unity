using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Deck Manager
    public DeckManager deckManager;

    // Player Controllers
    public PlayerController player1Controller;
    public PlayerController player2Controller;

    // Name Inputs
    public TMP_InputField player1Input;
    public TMP_InputField player2Input;

    // Bet Inputs
    public TMP_InputField player1BetInput;
    public TMP_InputField player2BetInput;

    // Balance Displays
    public TextMeshProUGUI player1BalanceText;
    public TextMeshProUGUI player2BalanceText;

    // Hand Value Displays
    public TextMeshProUGUI player1HandValueText;
    public TextMeshProUGUI player2HandValueText;

    // In-game Text
    public TextMeshProUGUI gameStatusText;

    // Game Controls
    public Button btnNewGame;
    public Button btnDealCards;

    // Dealer
    public Transform dealerCardArea;
    public TextMeshProUGUI dealerHandValueText;

    private PlayerData player1;
    private PlayerData player2;
    private int currentPlayerTurn = 1;

    private List<DeckManager.CardData> player1Hand = new List<DeckManager.CardData>();
    private List<DeckManager.CardData> player2Hand = new List<DeckManager.CardData>();
    private List<DeckManager.CardData> dealerHand = new List<DeckManager.CardData>();

    private bool player1BetPlaced = false;
    private bool player2BetPlaced = false;

    void Start()
    {
        SetupButtons();

        // Initialize player data with default values
        player1 = new PlayerData("Player 1");
        player2 = new PlayerData("Player 2");

        UpdateBalanceUI();

        // Disable action buttons at start
        player1Controller.SetActionButtons(false);
        player2Controller.SetActionButtons(false);

        gameStatusText.text = "Welcome to Blackjack! Press Start Game to begin.";
    }

    private void SetupButtons()
    {
        if (btnNewGame != null)
        {
            btnNewGame.onClick.AddListener(StartGame);
        }

        if (btnDealCards != null)
        {
            btnDealCards.onClick.AddListener(StartNewRound);
        }

        // Assuming btnDoubleDown is your double down button
        btnDoubleDown.onClick.AddListener(delegate { DoubleDown(currentPlayerTurn); });

        // Setup Hit and Stand Button listeners
        player1Controller.btnHit.onClick.AddListener(delegate { PlayerHit(1); });
        player2Controller.btnHit.onClick.AddListener(delegate { PlayerHit(2); });
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

        // Adjust for Aces if total > 21
        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    public void StartGame()
    {
        player1 = new PlayerData(player1Input.text.Length > 0 ? player1Input.text : "Player 1");
        player2 = new PlayerData(player2Input.text.Length > 0 ? player2Input.text : "Player 2");

        player1Hand.Clear();
        player2Hand.Clear();
        dealerHand.Clear();

        // Set initial balances
        player1Controller.SetBalance(player1.balance);
        player2Controller.SetBalance(player2.balance);

        UpdateBalanceUI();

        // Reset the board
        deckManager.ResetDeck();

        // Enable betting for both players
        player1Controller.EnableBettingOnly(true);
        player2Controller.EnableBettingOnly(true);

        gameStatusText.text = "Place your bets to start the round.";
    }

    public void StartNewRound()
    {
        // Check if both players have placed bets
        if (!player1BetPlaced || !player2BetPlaced)
        {
            gameStatusText.text = "Both players need to place bets first!";
            return;
        }

        player1Hand.Clear();
        player2Hand.Clear();
        dealerHand.Clear();

        // Reset UI hand values
        UpdateHandValueDisplay(1, 0);
        UpdateHandValueDisplay(2, 0);
        if (dealerHandValueText != null)
        {
            dealerHandValueText.text = "Dealer: ?";
        }

        // Reset the board and shuffle cards before game begins
        deckManager.ResetDeck();
        deckManager.ShuffleDeck();

        // Deal 2 starting cards to each player
        DealInitialCards();

        // Reset bet flags
        player1BetPlaced = false;
        player2BetPlaced = false;

        // Enable Player 1's actions, disable Player 2's
        player1Controller.SetActionButtons(true);
        player2Controller.SetActionButtons(false);

        // In-game status text
        gameStatusText.text = "Player 1's Turn";

        currentPlayerTurn = 1;
    }

    private void DealInitialCards()
    {
        // Deal 2 cards to each player
        for (int i = 0; i < 2; i++)
        {
            // Player 1
            DeckManager.CardData card1 = deckManager.DealCardToPlayer(1);
            player1Hand.Add(card1);

            // Player 2
            DeckManager.CardData card2 = deckManager.DealCardToPlayer(2);
            player2Hand.Add(card2);
        }

        // Update hand values
        int player1Value = CalculateHandValue(player1Hand);
        int player2Value = CalculateHandValue(player2Hand);

        UpdateHandValueDisplay(1, player1Value);
        UpdateHandValueDisplay(2, player2Value);
    }

    public void PlayerPlacedBet(int playerNumber, int betAmount)
    {
        if (playerNumber == 1)
        {
            player1BetPlaced = true;
        }
        else if (playerNumber == 2)
        {
            player2BetPlaced = true;
        }

        // Update game status
        if (player1BetPlaced && player2BetPlaced)
        {
            gameStatusText.text = "Both players placed bets. Ready to deal cards.";
        }
        else if (player1BetPlaced)
        {
            gameStatusText.text = "Player 1 bet placed. Waiting for Player 2.";
        }
        else if (player2BetPlaced)
        {
            gameStatusText.text = "Player 2 bet placed. Waiting for Player 1.";
        }
    }

    public void PlaceBets()
    {
        if (!int.TryParse(player1BetInput.text, out int bet1) || bet1 <= 0)
        {
            Debug.Log("Invalid bet for Player 1.");
            return;
        }

        if (!int.TryParse(player2BetInput.text, out int bet2) || bet2 <= 0)
        {
            Debug.Log("Invalid bet for Player 2.");
            return;
        }

        if (bet1 > player1.balance || bet2 > player2.balance)
        {
            Debug.Log("One of the players does not have enough balance.");
            return;
        }

        player1.balance -= bet1;
        player2.balance -= bet2;

        player1Hand.Clear();
        player2Hand.Clear();

        UpdateBalanceUI();

        Debug.Log($"Player 1 bet: {bet1}, Player 2 bet: {bet2}");

        deckManager.ResetDeck();
        deckManager.ShuffleDeck();

        deckManager.DealCardsToPlayer(1, 2);
        deckManager.DealCardsToPlayer(2, 2);
    }

    private void UpdateBalanceUI()
    {
        player1BalanceText.text = $"Balance: ${player1.balance}";
        player2BalanceText.text = $"Balance: ${player2.balance}";

        // Also update the player controllers
        player1Controller.SetBalance(player1.balance);
        player2Controller.SetBalance(player2.balance);
    }

    public void PlayerStand(int playerNumber)
    {
        Debug.Log($"Player {playerNumber} STANDS!");

        if (playerNumber == 1)
        {
            player1Controller.SetActionButtons(false);
            player2Controller.SetActionButtons(true);
            currentPlayerTurn = 2;
            gameStatusText.text = "Player 2's Turn";
        }
        else if (playerNumber == 2)
        {
            player2Controller.SetActionButtons(false);
            gameStatusText.text = "Both players stood. Dealer's Turn...";
            Debug.Log("Both players have stood. Starting dealer turn.");

            // Start dealer's turn
            StartCoroutine(DealerTurn());
        }
    }

    public void PlayerHit(int playerNumber)
    {
        // Deal one card to the player
        DeckManager.CardData card = deckManager.DealCardToPlayer(playerNumber);

        // Add the card to the appropriate hand
        List<DeckManager.CardData> currentHand = (playerNumber == 1) ? player1Hand : player2Hand;
        currentHand.Add(card);

        // Calculate the new hand value
        int handValue = CalculateHandValue(currentHand);

        // Update UI
        UpdateHandValueDisplay(playerNumber, handValue);

        // Check for bust (over 21)
        if (handValue > 21)
        {
            Debug.Log($"Player {playerNumber} BUSTS with {handValue}!");

            if (playerNumber == 1)
            {
                player1Controller.SetActionButtons(false);
                // Player 1 busted, switch to player 2's turn
                currentPlayerTurn = 2;
                player2Controller.SetActionButtons(true);
                gameStatusText.text = "Player 1 Busted! Player 2's Turn";
            }
            else
            {
                player2Controller.SetActionButtons(false);
                // Player 2 busted, end the round
                gameStatusText.text = "Player 2 Busted! Dealer's Turn...";
                StartCoroutine(DealerTurn());
            }
        }
    }
    public void DoubleDown(int playerNumber)
    {
        PlayerData player = (playerNumber == 1) ? player1 : player2;
        PlayerController playerController = (playerNumber == 1) ? player1Controller : player2Controller;

        if (player.hand.Count == 2 && player.balance >= playerController.GetCurrentBet())
        {
            int currentBet = playerController.GetCurrentBet();
            player.balance -= currentBet; // Deduct the additional bet
            playerController.SetBet(currentBet * 2); // Update the bet amount
            PlayerHit(playerNumber); // Deal one more card
            PlayerStand(playerNumber); // End the player's turn
        }
        else
        {
            gameStatusText.text = "Cannot double down at this time.";
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

    private IEnumerator DealerTurn()
    {
        gameStatusText.text = "Dealer's Turn...";

        // Small delay before dealer starts
        yield return new WaitForSeconds(1.0f);

        // Clear any previous dealer cards
        foreach (Transform child in dealerCardArea)
        {
            Destroy(child.gameObject);
        }

        dealerHand.Clear();

        // Deal initial cards to dealer if not already dealt
        while (CalculateHandValue(dealerHand) < 17)
        {
            DeckManager.CardData card = deckManager.DrawCardToDealer();
            dealerHand.Add(card);
            UpdateDealerHandValueDisplay(CalculateHandValue(dealerHand));

            // Update UI for each card drawn
            yield return new WaitForSeconds(0.5f); // Delay between cards for visibility
        }

        // After dealer is done, check and display results
        CheckFinalResults();
        yield return new WaitForSeconds(1.0f); // Pause before showing results
    }

    private void UpdateDealerHandValueDisplay(int value)
    {
        dealerHandValueText.text = $"Dealer Hand: {value}";
    }

    private void CheckFinalResults()
    {
        int dealerValue = CalculateHandValue(dealerHand);
        string resultMessage = "Round over: ";

        // Compare dealer hand with each player
        resultMessage += CompareHands(player1Hand, dealerValue, 1);
        resultMessage += CompareHands(player2Hand, dealerValue, 2);

        gameStatusText.text = resultMessage;
    }

    private string CompareHands(List<DeckManager.CardData> playerHand, int dealerValue, int playerNumber)
    {
        int playerValue = CalculateHandValue(playerHand);
        if (playerValue > 21)
            return $"Player {playerNumber} busts. ";
        else if (playerValue > dealerValue)
            return $"Player {playerNumber} wins. ";
        else if (playerValue == dealerValue)
            return $"Player {playerNumber} pushes. ";
        else
            return $"Player {playerNumber} loses. ";
    }

    private void EndRound()
    {
        int player1Value = CalculateHandValue(player1Hand);
        int player2Value = CalculateHandValue(player2Hand);
        int dealerValue = CalculateHandValue(dealerHand);

        string resultMessage = "";

        // Check for busts
        bool player1Busted = player1Value > 21;
        bool player2Busted = player2Value > 21;
        bool dealerBusted = dealerValue > 21;

        // Determine results against dealer
        if (dealerBusted)
        {
            resultMessage = "Dealer busted! ";

            // Both players who didn't bust win
            if (!player1Busted)
            {
                player1.balance += player1Controller.GetBet() * 2; // Return bet + winnings
                resultMessage += $"{player1.playerName} wins. ";
            }

            if (!player2Busted)
            {
                player2.balance += player2Controller.GetBet() * 2; // Return bet + winnings
                resultMessage += $"{player2.playerName} wins.";
            }
        }
        else
        {
            // Dealer didn't bust, compare hands

            // Player 1 vs Dealer
            if (!player1Busted)
            {
                if (player1Value > dealerValue)
                {
                    player1.balance += player1Controller.GetBet() * 2; // Return bet + winnings
                    resultMessage += $"{player1.playerName} beats dealer. ";
                }
                else if (player1Value == dealerValue)
                {
                    player1.balance += player1Controller.GetBet(); // Push - return bet
                    resultMessage += $"{player1.playerName} pushed with dealer. ";
                }
                else
                {
                    resultMessage += $"{player1.playerName} lost to dealer. ";
                }
            }
            else
            {
                resultMessage += $"{player1.playerName} busted. ";
            }

            // Player 2 vs Dealer
            if (!player2Busted)
            {
                if (player2Value > dealerValue)
                {
                    player2.balance += player2Controller.GetBet() * 2; // Return bet + winnings
                    resultMessage += $"{player2.playerName} beats dealer.";
                }
                else if (player2Value == dealerValue)
                {
                    player2.balance += player2Controller.GetBet(); // Push - return bet
                    resultMessage += $"{player2.playerName} pushed with dealer.";
                }
                else
                {
                    resultMessage += $"{player2.playerName} lost to dealer.";
                }
            }
            else
            {
                resultMessage += $"{player2.playerName} busted.";
            }
        }

        // Update the game status text with results
        gameStatusText.text = resultMessage;

        // Update balance displays
        UpdateBalanceUI();

        // Re-enable betting for next round
        player1Controller.ResetForNewRound();
        player2Controller.ResetForNewRound();
    }
}
