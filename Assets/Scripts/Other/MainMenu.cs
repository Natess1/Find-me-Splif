using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MainMenu");

    }
    public void ExitGame()
    {
        Debug.Log("Игра закрыта");
        Application.Quit();
    }
}
