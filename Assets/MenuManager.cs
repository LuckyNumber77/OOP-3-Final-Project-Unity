using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public GameObject rulesPanel; // Drag your RulesPanel object here in Inspector

    // Called when Start Game button is clicked
    public void StartGame()
    {
        // Keep only one of these lines, not both:
        // If your scene is named "Game Play", use:
        SceneManager.LoadScene("Game Play");
        // Otherwise, use this line and remove the one above:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Called when Game Rules button is clicked
    public void ShowRules()
    {
        // Remove the scene loading line
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // Just show the panel
        rulesPanel.SetActive(true);
    }

    // Add this method if it's missing
    public void HideRules()
    {
        // Just hide the panel
        rulesPanel.SetActive(false);
    }
}