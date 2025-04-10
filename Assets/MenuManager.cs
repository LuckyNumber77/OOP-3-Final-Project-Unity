using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject rulesPanel; // Drag your RulesPanel object here in Inspector

    // Called when Start Game button is clicked
    public void StartGame()
    {
        SceneManager.LoadScene("Game Play"); // Replace "Game Play" with your exact Gameplay scene name if different
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    // Called when Game Rules button is clicked
    public void ShowRules()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        rulesPanel.SetActive(true); // Show the Rules Panel
    }

    
}
