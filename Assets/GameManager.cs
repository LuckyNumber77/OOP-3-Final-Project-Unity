using UnityEngine;
using TMPro;

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

    // In-game Text
    public TextMeshProUGUI gameStatusText;

    private PlayerData player1;
    private PlayerData player2;
    private int currentPlayerTurn = 1;

    public void StartGame()
    {
        player1 = new PlayerData(player1Input.text);
        player2 = new PlayerData(player2Input.text);

        UpdateBalanceUI();

        // Reset the board and shuffle cards before game begins
        deckManager.ResetDeck();
        deckManager.ShuffleDeck();

        // Deal 2 starting cards to each player
        deckManager.DealCardsToPlayer(1, 2);
        deckManager.DealCardsToPlayer(2, 2);

        // Enable Player 1's actions, disable Player 2's
        player1Controller.SetActionButtons(true);
        player2Controller.SetActionButtons(false);

        // In-game status text
        gameStatusText.text = "Player 1's Turn";
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
            Debug.Log("Both players have stood. Ready for dealer logic.");
        }
    }
}
