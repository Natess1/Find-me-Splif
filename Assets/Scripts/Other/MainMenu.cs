using UnityEngine;
[RequireComponent(typeof(SceneTransition))]
public class MainMenu : MonoBehaviour
{




    public void PlayGame()
    {
        SceneTransition.SwitchToScene("NewPlayScene");
    }

    public void LoadSettingsScene()
    {
        SceneTransition.SwitchToScene("SettingsScene");
    }

    public void ShowTradeMenu()
    {
        GameInput.Instance.DisableMovement();

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
