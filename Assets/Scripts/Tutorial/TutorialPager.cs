using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using TMPro;
using UnityEngine.InputSystem.UI;

public class TutorialPager : MonoBehaviour
{
    [Serializable]
    public class TutorialPage
    {
        public string title;
        public VideoClip video;
        public string videoResourcesPath;
        [TextArea(2, 4)] public string controlsText;
        public bool isWarning = false;
        [TextArea(4, 10)] public string warningText;
    }

    [Header("Страницы (пусто — создадутся стандартные)")]
    [SerializeField] private List<TutorialPage> pages = new List<TutorialPage>();

    [Header("Шрифт всех текстов экрана")]
    [SerializeField] private TMP_FontAsset font;

    [Header("Окно с видео")]
    [SerializeField, Range(0.3f, 1f)] private float videoSize = 1f;
    [SerializeField, Range(0f, 60f)] private float windowPadding = 16f;

    [Header("Компоновка")]
    [SerializeField, Range(30f, 50f)] private float topMargin = 40f;

    [Header("Оформление")]
    [SerializeField, Range(0f, 1f)] private float backgroundDarkening = 0.35f;
    [SerializeField] private Color windowColor = new Color(0.16f, 0.14f, 0.27f, 0.92f);
    [SerializeField] private Color titleColor = new Color(0.91f, 0.76f, 0.35f, 1f);
    [SerializeField] private Color textColor = new Color(0.85f, 0.85f, 0.85f, 1f);

    [Header("Фон как в главном меню")]
    [SerializeField] private GameObject backgroundPrefab;
    [SerializeField] private float backgroundScale = 1f;
    [SerializeField] private float menuCameraSpeed = 0.5f;
    [SerializeField] private float menuCameraMovingTime = 50f;

    [Header("Спрайты стрелок (пусто — возьмутся из Resources)")]
    [SerializeField] private Sprite arrowLeftSprite;
    [SerializeField] private Sprite arrowRightSprite;

    private RectTransform root;
    private RectTransform windowRect;
    private RectTransform videoHolder;
    private RectTransform sceneTitleRect;
    private RectTransform captionRect;
    private RectTransform backRect;
    private CanvasScaler scaler;
    private int currentPage;

    private TextMeshProUGUI pageTitleText;
    private TextMeshProUGUI controlsLabel;
    private TextMeshProUGUI warningLabel;
    private RawImage videoRaw;
    private VideoPlayer videoPlayer;
    private Button leftButton;
    private Button rightButton;

    private readonly List<TextMeshProUGUI> allTexts = new List<TextMeshProUGUI>();
    private readonly Dictionary<VideoClip, RenderTexture> rtCache = new Dictionary<VideoClip, RenderTexture>();
    private VideoClip currentClip;
    private bool firstFrameShown;
    private float layoutAspect = 16f / 9f;
    private Vector2 lastCanvasSize = new Vector2(-1f, -1f);
    private bool layoutDirty = true;

    private const float headerExtra = 116f;
    private const float arrowZone = 120f;
    private const float captionWidth = 560f;

    private void Awake()
    {
        if (pages == null || pages.Count == 0)
        {
            BuildDefaultPages();
        }

        EnsureCanvasAndEventSystem();
        BuildUI();
        layoutAspect = FindFirstAspect();
        ApplyFont();
        SetupBackground();
        ShowPage(0);
    }

    private void OnValidate()
    {
        layoutDirty = true;
        ApplyFont();
    }

    private void LateUpdate()
    {
        if (root == null) return;

        float match = Screen.width >= Screen.height ? 1f : 0f;
        if (!Mathf.Approximately(scaler.matchWidthOrHeight, match))
        {
            scaler.matchWidthOrHeight = match;
            layoutDirty = true;
        }

        if (layoutDirty || root.rect.size != lastCanvasSize)
        {
            UpdateLayout();
        }
    }

    private void BuildDefaultPages()
    {
        pages = new List<TutorialPage>
        {
            new TutorialPage
            {
                title = "Особенности ведения боя <color=#E04747>!</color>",
                isWarning = true,
                warningText = "Смерть обнуляет монеты и уровень.\nВраги приходят волнами —\nпродержись как можно дольше!",
                controlsText = "Будь осторожен!"
            },
            new TutorialPage
            {
                title = "Ходьба",
                videoResourcesPath = "Tutorial/Videos/Walk",
                controlsText = "Потяни джойстик, чтобы ходить"
            },
            new TutorialPage
            {
                title = "Дэш",
                videoResourcesPath = "Tutorial/Videos/Dash",
                controlsText = "Нажми кнопку DASH,\nчтобы сделать рывок"
            },
            new TutorialPage
            {
                title = "Атака",
                videoResourcesPath = "Tutorial/Videos/Attack",
                controlsText = "Нажми кнопку ATTACK,\nчтобы ударить мечом"
            },
            new TutorialPage
            {
                title = "Хил",
                videoResourcesPath = "Tutorial/Videos/Heal",
                controlsText = "Подойди к торговцу\nи купи лечение за монеты"
            }
        };
    }

    private void EnsureCanvasAndEventSystem()
    {
        var rootGO = new GameObject("TutorialRoot", typeof(RectTransform));
        rootGO.transform.SetParent(transform, false);

        var canvas = rootGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        scaler = rootGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800f, 600f);
        scaler.matchWidthOrHeight = Screen.width >= Screen.height ? 1f : 0f;

        rootGO.AddComponent<GraphicRaycaster>();
        root = (RectTransform)rootGO.transform;

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }
    }

    private void BuildUI()
    {
        var bg = CreateImage(root, "Background", null, new Color(0f, 0f, 0f, backgroundDarkening));
        Stretch(bg.rectTransform);

        var title = CreateText(root, "SceneTitle", "Обучение", 44, titleColor, TextAlignmentOptions.Center);
        sceneTitleRect = title.rectTransform;

        pageTitleText = CreateText(root, "PageTitle", "", 26, titleColor, TextAlignmentOptions.Center);

        var window = CreateImage(root, "Window", null, windowColor);
        windowRect = window.rectTransform;
        windowRect.pivot = new Vector2(0.5f, 0.5f);

        videoHolder = CreateRect(windowRect, "VideoHolder");
        videoHolder.pivot = new Vector2(0.5f, 0.5f);

        videoRaw = videoHolder.gameObject.AddComponent<RawImage>();
        videoRaw.raycastTarget = false;
        videoRaw.color = Color.white;
        videoRaw.enabled = false;

        videoPlayer = windowRect.gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = true;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.frameReady += OnVideoFrameReady;

        warningLabel = CreateText(windowRect, "WarningText", "", 21, textColor, TextAlignmentOptions.Center);
        StretchWithPadding(warningLabel.rectTransform, 40f, 10f, 40f, 10f);

        controlsLabel = CreateText(root, "ControlsText", "", 20, textColor, TextAlignmentOptions.Center);
        captionRect = controlsLabel.rectTransform;

        if (arrowLeftSprite == null) arrowLeftSprite = Resources.Load<Sprite>("Tutorial/ArrowLeft");
        if (arrowRightSprite == null) arrowRightSprite = Resources.Load<Sprite>("Tutorial/ArrowRight");

        leftButton = CreateArrowButton(root, "ArrowLeft", arrowLeftSprite, new Vector2(0f, 0.5f), new Vector2(80f, 0f));
        leftButton.onClick.AddListener(() => ShowPage(currentPage - 1));

        rightButton = CreateArrowButton(root, "ArrowRight", arrowRightSprite, new Vector2(1f, 0.5f), new Vector2(-80f, 0f));
        rightButton.onClick.AddListener(() => ShowPage(currentPage + 1));

        backRect = CreateRect(root, "BackButton");
        var backImage = backRect.gameObject.AddComponent<Image>();
        backImage.color = new Color(0.24f, 0.2f, 0.38f, 1f);
        var backBtn = backRect.gameObject.AddComponent<Button>();
        backBtn.targetGraphic = backImage;
        var backLabel = CreateText(backRect, "Label", "В МЕНЮ", 24, Color.white, TextAlignmentOptions.Center);
        Stretch(backLabel.rectTransform);
        backBtn.onClick.AddListener(BackToMenu);
    }

    private void UpdateLayout()
    {
        Vector2 canvas = root.rect.size;
        if (canvas.x <= 1f || canvas.y <= 1f) return;

        bool wide = canvas.x >= captionWidth + 500f;
        float top = topMargin + headerExtra;
        float bottom = wide ? 78f : 130f;

        SetAnchor(sceneTitleRect, new Vector2(0.5f, 1f), new Vector2(0f, -(topMargin + 28f)), new Vector2(500f, 56f));
        SetAnchor(pageTitleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -(topMargin + 88f)), new Vector2(560f, 36f));
        SetAnchor(captionRect, new Vector2(0.5f, 0f), new Vector2(0f, wide ? 44f : 90f), new Vector2(captionWidth, 46f));

        if (wide)
        {
            SetAnchor(backRect, new Vector2(0f, 0f), new Vector2(130f, 38f), new Vector2(220f, 52f));
        }
        else
        {
            SetAnchor(backRect, new Vector2(0.5f, 0f), new Vector2(0f, 28f), new Vector2(220f, 52f));
        }

        float pad = windowPadding;
        float availW = Mathf.Max(10f, canvas.x - 2f * arrowZone - 2f * pad);
        float availH = Mathf.Max(10f, canvas.y - top - bottom - 2f * pad);
        float aspect = Mathf.Max(0.1f, layoutAspect);

        float width;
        float height;

        if (availW / availH > aspect)
        {
            height = availH;
            width = height * aspect;
        }
        else
        {
            width = availW;
            height = width / aspect;
        }

        float scale = Mathf.Clamp(videoSize, 0.3f, 1f);
        width *= scale;
        height *= scale;

        windowRect.anchorMin = new Vector2(0.5f, 0.5f);
        windowRect.anchorMax = new Vector2(0.5f, 0.5f);
        windowRect.anchoredPosition = new Vector2(0f, (bottom - top) * 0.5f);
        windowRect.sizeDelta = new Vector2(width + 2f * pad, height + 2f * pad);

        videoHolder.anchorMin = new Vector2(0.5f, 0.5f);
        videoHolder.anchorMax = new Vector2(0.5f, 0.5f);
        videoHolder.anchoredPosition = Vector2.zero;
        videoHolder.sizeDelta = new Vector2(width, height);

        lastCanvasSize = canvas;
        layoutDirty = false;
    }

    private void ApplyFont()
    {
        if (font == null) return;

        foreach (var text in allTexts)
        {
            if (text != null) text.font = font;
        }
    }

    private void SetupBackground()
    {
        if (backgroundPrefab == null) return;

        Camera cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();

        var bg = Instantiate(backgroundPrefab, transform);
        bg.name = "TutorialBG";
        Vector3 camPos = cam != null ? cam.transform.position : Vector3.zero;
        bg.transform.position = new Vector3(camPos.x, camPos.y, 0f);
        bg.transform.localScale = Vector3.one * Mathf.Max(0.01f, backgroundScale);

        if (cam == null) return;

        foreach (var p in bg.GetComponentsInChildren<Paralax>(true))
        {
            p.MovigCamera = cam.gameObject;
        }

        var camParallax = cam.GetComponent<CameraParallax>();
        if (camParallax == null) camParallax = cam.gameObject.AddComponent<CameraParallax>();
        camParallax.cameraSpeed = menuCameraSpeed;
        camParallax.movingTime = menuCameraMovingTime;
    }

    private void BackToMenu()
    {
        if (SceneTransition.HasInstance)
        {
            SceneTransition.SwitchToScene("MainMenu");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    private VideoClip ResolveClip(TutorialPage page)
    {
        if (page.isWarning) return null;

        VideoClip clip = page.video;
        if (clip == null && !string.IsNullOrEmpty(page.videoResourcesPath))
        {
            clip = Resources.Load<VideoClip>(page.videoResourcesPath);
        }
        return clip;
    }

    private float FindFirstAspect()
    {
        foreach (var page in pages)
        {
            VideoClip clip = ResolveClip(page);
            if (clip != null && clip.width > 0 && clip.height > 0)
            {
                return (float)clip.width / clip.height;
            }
        }
        return 16f / 9f;
    }

    private void ShowPage(int index)
    {
        currentPage = Mathf.Clamp(index, 0, pages.Count - 1);
        TutorialPage page = pages[currentPage];

        pageTitleText.text = page.title;
        controlsLabel.text = page.controlsText;
        warningLabel.text = page.warningText;
        warningLabel.gameObject.SetActive(page.isWarning);

        StopVideo();

        VideoClip clip = ResolveClip(page);
        if (clip != null)
        {
            PlayVideo(clip);
        }

        leftButton.interactable = currentPage > 0;
        rightButton.interactable = currentPage < pages.Count - 1;
    }

    private void PlayVideo(VideoClip clip)
    {
        currentClip = clip;
        firstFrameShown = false;

        if (clip.width > 0 && clip.height > 0)
        {
            layoutAspect = (float)clip.width / clip.height;
            layoutDirty = true;
        }

        RenderTexture rt = GetOrCreateRT(clip);

        videoRaw.texture = rt;
        videoRaw.enabled = false;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = rt;
        videoPlayer.clip = clip;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        if (currentClip == null || vp.clip != currentClip) return;
        vp.Play();
    }

    private void OnVideoFrameReady(VideoPlayer vp, long frameIdx)
    {
        if (currentClip == null || firstFrameShown) return;

        firstFrameShown = true;
        vp.sendFrameReadyEvents = false;
        videoRaw.enabled = true;
    }

    private void StopVideo()
    {
        currentClip = null;
        firstFrameShown = false;
        videoPlayer.sendFrameReadyEvents = false;
        videoPlayer.Stop();
        videoRaw.enabled = false;
        videoRaw.texture = null;
    }

    private RenderTexture GetOrCreateRT(VideoClip clip)
    {
        int width = Mathf.Max(2, (int)clip.width);
        int height = Mathf.Max(2, (int)clip.height);

        if (rtCache.TryGetValue(clip, out RenderTexture rt) && rt != null)
        {
            if (rt.width == width && rt.height == height) return rt;
            rt.Release();
            Destroy(rt);
            rtCache.Remove(clip);
        }

        rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32) { name = "TutorialVideoRT_" + clip.name };
        rt.Create();
        rtCache[clip] = rt;
        return rt;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.frameReady -= OnVideoFrameReady;
        }

        foreach (var kv in rtCache)
        {
            if (kv.Value != null)
            {
                kv.Value.Release();
                Destroy(kv.Value);
            }
        }
        rtCache.Clear();
    }

    private static RectTransform CreateRect(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static Image CreateImage(Transform parent, string name, Sprite sprite, Color color)
    {
        var rect = CreateRect(parent, name);
        var img = rect.gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment)
    {
        var rect = CreateRect(parent, name);
        var tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.raycastTarget = false;
        allTexts.Add(tmp);
        return tmp;
    }

    private static void SetAnchor(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void StretchWithPadding(RectTransform rt, float left, float top, float right, float bottom)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    private Button CreateArrowButton(RectTransform parent, string name, Sprite sprite, Vector2 anchor, Vector2 pos)
    {
        var rect = CreateRect(parent, name);
        SetAnchor(rect, anchor, pos, new Vector2(72f, 72f));

        var img = rect.gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.color = Color.white;

        var btn = rect.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;

        var colors = btn.colors;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.25f);
        btn.colors = colors;

        return btn;
    }
}