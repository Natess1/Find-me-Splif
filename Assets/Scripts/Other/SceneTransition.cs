using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class SceneTransition : MonoBehaviour
{
    public Text LoadingPercent;
    public Image LoadingProgressBar;

    private static SceneTransition instance;
    private static bool shouldPlayOpeningAnimation = false;
    private static bool transitioning = false;   // идёт ли сейчас переход (защита от повторных нажатий)

    // Имена триггеров различаются у двух префабов перехода:
    // SceneTransition.prefab (DeathScene) — sceneEnd / sceneStart,
    // NewSceneTransition.prefab (меню, игра, туториал) — Close / Open.
    // Реальное имя подбирается в Start() из фактического аниматора.
    private int closeTriggerHash;
    private int openTriggerHash;

    // Страховка: в Ending.anim старого контроллера НЕТ AnimationEvent OnAnimationOver,
    // без таймера переход на таком префабе зависал бы навсегда.
    private const float MaxCloseWait = 2f;
    private Coroutine activationTimeout;

    /// <summary>Есть ли в текущей сцене объект SceneTransition.</summary>
    public static bool HasInstance => instance != null;

    private Animator animator;
    private AsyncOperation loadingSceneOperation;

    public static void SwitchToScene(string sceneName)
    {
        // Повторные нажатия, пока переход уже идёт, игнорируем
        if (transitioning)
        {
            return;
        }

        // В сцене нет живого SceneTransition (например, DeathScene) —
        // instance указывает на уничтоженный объект прошлой сцены и обращение к нему падает с NRE.
        // Также уходим в фолбэк, если у аниматора не нашлось триггера закрытия.
        if (instance == null || instance.animator == null || instance.closeTriggerHash == 0)
        {
            transitioning = true;
            StartFallbackTransition(sceneName);
            return;
        }

        transitioning = true;

        instance.animator.SetTrigger(instance.closeTriggerHash);

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);

        instance.loadingSceneOperation.allowSceneActivation = false;

        if (instance.LoadingProgressBar != null)
        {
            instance.LoadingProgressBar.fillAmount = 0;
        }

        // Если AnimationEvent OnAnimationOver не придёт (контроллер без событий в клипе) —
        // активируем сцену по таймеру, чтобы переход не завис
        instance.activationTimeout = instance.StartCoroutine(instance.ActivateAfterTimeout());
    }

    private void Start()
    {
        instance = this;

        animator = GetComponent<Animator>();

        // Подбираем имена триггеров из фактического контроллера:
        // у SceneTransition.prefab это sceneEnd/sceneStart, у NewSceneTransition.prefab — Close/Open
        closeTriggerHash = ResolveTriggerHash("sceneEnd", "Close");
        openTriggerHash = ResolveTriggerHash("sceneStart", "Open");

        // Новый переход начался в новой сцене — снимаем блокировку на случай,
        // если предыдущий не завершился штатно
        transitioning = false;

        if (shouldPlayOpeningAnimation)
        {
            if (openTriggerHash != 0)
            {
                animator.SetTrigger(openTriggerHash);
            }

            if (LoadingProgressBar != null)
            {
                LoadingProgressBar.fillAmount = 1;
            }

            shouldPlayOpeningAnimation = false;
        }
    }

    private void Update()
    {
        if (loadingSceneOperation == null)
        {
            return;
        }

        if (LoadingPercent != null)
        {
            LoadingPercent.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";
        }

        if (LoadingProgressBar != null)
        {
            LoadingProgressBar.fillAmount = Mathf.Lerp(LoadingProgressBar.fillAmount, loadingSceneOperation.progress,
                Time.deltaTime * 1);
        }
    }

    public void OnAnimationOver()
    {
        if (activationTimeout != null)
        {
            StopCoroutine(activationTimeout);
            activationTimeout = null;
        }

        shouldPlayOpeningAnimation = true;
        transitioning = false;

        if (loadingSceneOperation != null)
        {
            loadingSceneOperation.allowSceneActivation = true;
        }
    }

    // Сцена обязана активироваться даже если анимация не доиграла/события нет
    private IEnumerator ActivateAfterTimeout()
    {
        yield return new WaitForSecondsRealtime(MaxCloseWait);

        activationTimeout = null;
        OnAnimationOver();
    }

    // Ищем среди триггеров контроллера первое совпадение из names.
    // Не нашли ни одного — возвращаем 0 (переход пойдёт через код-фолбэк).
    private int ResolveTriggerHash(params string[] names)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return 0;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type != AnimatorControllerParameterType.Trigger)
            {
                continue;
            }

            foreach (string name in names)
            {
                if (parameter.name == name)
                {
                    return Animator.StringToHash(parameter.name);
                }
            }
        }

        return 0;
    }

    // ------------------------------------------------------------------
    // Переход без Animator'а: чёрный Canvas создаётся кодом, затемнение,
    // загрузка сцены, открытие. Ничего настраивать в сцене не нужно.
    // ------------------------------------------------------------------
    private static void StartFallbackTransition(string sceneName)
    {
        var go = new GameObject("SceneTransitionFallback");
        DontDestroyOnLoad(go);
        go.AddComponent<FallbackRunner>().sceneName = sceneName;
    }

    private class FallbackRunner : MonoBehaviour
    {
        public string sceneName;

        private IEnumerator Start()
        {
            var canvasGo = new GameObject("FallbackCanvas", typeof(Canvas));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;
            DontDestroyOnLoad(canvasGo);

            var imageGo = new GameObject("Fade", typeof(Image));
            imageGo.transform.SetParent(canvasGo.transform, false);
            var image = imageGo.GetComponent<Image>();
            image.raycastTarget = true;                       // на время перехода блокируем клики
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.sizeDelta = Vector2.zero;
            image.color = new Color(0f, 0f, 0f, 0f);

            yield return Fade(image, 0f, 1f, 0.2f);

            SceneManager.LoadScene(sceneName);

            // даём новой сцене отрисоваться
            yield return null;
            yield return null;

            // переход завершён — снимаем блокировку (если в новой сцене нет SceneTransition,
            // его Start() не сработает и снять её будет некому)
            transitioning = false;

            yield return Fade(image, 1f, 0f, 0.3f);

            Destroy(canvasGo);
            Destroy(gameObject);
        }

        private static IEnumerator Fade(Image image, float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                image.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, Mathf.Clamp01(t / duration)));
                yield return null;
            }

            image.color = new Color(0f, 0f, 0f, to);
        }
    }
}
