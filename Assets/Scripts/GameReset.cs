using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check for a reset input, such as the 'R' key
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset the current scene
            ResetGame();
        }
    }

    void ResetGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
