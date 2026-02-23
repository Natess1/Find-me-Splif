using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        SceneTransition.SwitchToScene("Level2");
    }

    public void LoadSettingsScene()
    {
        SceneTransition.SwitchToScene("SettingsScene");
    }

    public void LoadMenuScene()
    {
        SceneTransition.SwitchToScene("MainMenu");
    }
    public void ExitGame()
    {
        Debug.Log("Игра закрыта");
        Application.Quit();
    }
}
