using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject rulesPanel; // Drag your RulesPanel object here in Inspector

    // Called when Start Game button is clicked
    public void StartGame()
    {
        SceneManager.LoadScene("Game Play"); // Replace "Game Play" with your exact Gameplay scene name if different
    }

    // Called when Game Rules button is clicked
    public void ShowRules()
    {
        rulesPanel.SetActive(true); // Show the Rules Panel
    }

    // Called when Close button on Rules Panel is clicked
    public void HideRules()
    {
        rulesPanel.SetActive(false); // Hide the Rules Panel
    }
}
