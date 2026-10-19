using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Проигрывает последовательность спрайтов на Image (демо в окошках обучения).
/// Повесь на объект с Image, задай кадры — или вызывай Play() из кода.
/// </summary>
public class SpriteSequencePlayer : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 8f;
    [SerializeField] private bool playOnEnable = true;

    private int frameIndex;
    private float timer;
    private bool playing;

    private void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (playOnEnable && frames != null && frames.Length > 0)
        {
            Play();
        }
    }

    private void Update()
    {
        if (!playing || frames == null || frames.Length == 0) return;

        float step = 1f / Mathf.Max(1f, fps);
        timer += Time.unscaledDeltaTime;

        while (timer >= step)
        {
            timer -= step;
            frameIndex = (frameIndex + 1) % frames.Length;
            ApplyFrame();
        }
    }

    public void Play()
    {
        if (frames == null || frames.Length == 0) return;

        playing = true;
        frameIndex = 0;
        timer = 0f;
        ApplyFrame();
    }

    public void Play(Sprite[] newFrames, float newFps)
    {
        frames = newFrames;
        fps = newFps;
        Play();
    }

    public void Stop()
    {
        playing = false;
    }

    private void ApplyFrame()
    {
        if (targetImage == null || frames == null || frames.Length == 0) return;

        targetImage.sprite = frames[frameIndex];
        targetImage.enabled = true;
    }
}
