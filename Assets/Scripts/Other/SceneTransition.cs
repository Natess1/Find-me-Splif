using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Text LoadingPercent;
    public Image LoadingProgressBar;

    private static SceneTransition instance;
    private static bool shouldPlayOpeningAnimation = false;

    private Animator animator;
    private AsyncOperation loadingSceneOperation;

    public static void SwitchToScene(string sceneName)
    {
        instance.animator.SetTrigger("Close");

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);

        instance.loadingSceneOperation.allowSceneActivation = false;

        instance.LoadingProgressBar.fillAmount = 0;
    }

    private void Start()
    {
        instance = this;

        animator = GetComponent<Animator>();

        if (shouldPlayOpeningAnimation)
        {
            animator.SetTrigger("Open");
            instance.LoadingProgressBar.fillAmount = 1;

            shouldPlayOpeningAnimation = false;
        }
    }

    private void Update()
    {
        if (loadingSceneOperation != null)
        {
            LoadingPercent.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";

            LoadingProgressBar.fillAmount = Mathf.Lerp(LoadingProgressBar.fillAmount, loadingSceneOperation.progress,
                Time.deltaTime * 5);
        }
    }

    public void OnAnimationOver()
    {
        shouldPlayOpeningAnimation = true;
        loadingSceneOperation.allowSceneActivation = true;
    }
}