using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void newGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void loadGame()
    {
        Debug.Log("TODO");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
