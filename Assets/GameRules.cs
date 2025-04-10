using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRules : MonoBehaviour
{
    // Start is called before the first frame update
    // Called when Close button on Rules Panel is clicked
    public void HideRules()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Check if there's a previous scene
        if (currentSceneIndex > 0)
        {
            // Load the previous scene (index - 1)
            SceneManager.LoadScene(currentSceneIndex - 1);
        }
        else
        {
            Debug.Log("No previous scene to go back to.");
        }
    }
}
