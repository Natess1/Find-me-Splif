using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Добавляет кнопку «ОБУЧЕНИЕ» в главное меню прямо в рантайме.
/// Клонирует существующую PlayButton (тот же вид), меняет надпись,
/// ставит её ниже и ведёт в сцену обучения.
/// Повесь на любой объект сцены MainMenu (например на Canvas).
/// </summary>
public class MainMenuTutorialButton : MonoBehaviour
{
    [SerializeField] private string playButtonName = "PlayButton";
    [SerializeField] private Vector2 offsetFromPlayButton = new Vector2(0f, -110f);
    [SerializeField] private string buttonText = "ОБУЧЕНИЕ";
    [SerializeField] private string tutorialSceneName = "TutorialScene";

    private void Start()
    {
        Button btn = TryClonePlayButton();

        if (btn == null)
        {
            btn = BuildFallbackButton();
        }

        if (btn == null)
        {
            Debug.LogWarning("MainMenuTutorialButton: не удалось создать кнопку обучения");
            return;
        }

        // Очищаем унаследованные обработчики (клон копирует onClick от Play)
        btn.onClick = new Button.ButtonClickedEvent();
        btn.onClick.AddListener(LoadTutorial);
    }

    private void LoadTutorial()
    {
        SceneTransition.SwitchToScene(tutorialSceneName);
    }

    private Button TryClonePlayButton()
    {
        GameObject source = GameObject.Find(playButtonName);
        if (source == null) return null;

        GameObject clone = Instantiate(source, source.transform.parent);
        clone.name = "TutorialButton";

        var rt = clone.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition += offsetFromPlayButton;
        }

        var label = clone.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.text = buttonText;
        }

        var btn = clone.GetComponent<Button>();
        if (btn == null)
        {
            btn = clone.AddComponent<Button>();
        }

        return btn;
    }

    private Button BuildFallbackButton()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        var rect = new GameObject("TutorialButton", typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -60f);
        rect.sizeDelta = new Vector2(220f, 60f);

        var image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.24f, 0.2f, 0.38f, 1f);

        var btn = rect.gameObject.AddComponent<Button>();
        btn.targetGraphic = image;

        var labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(rect, false);
        var label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text = buttonText;
        label.fontSize = 24;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;

        var labelRect = (RectTransform)labelGO.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return btn;
    }
}
