using UnityEngine;

/// <summary>
/// Всплывающий эффект хила.
/// Появляется ЗА игроком (рисуется под его спрайтом), поднимается строго вверх
/// и растворяется с той же задержкой, что и раньше (первые 15% — появление,
/// с 65% времени жизни — растворение).
/// Длительность задаётся в инспекторе Player (healEffectDuration).
/// </summary>
public class HealEffect : MonoBehaviour
{
    private const float FadeInPart = 0.15f;    // первые 15% — появление
    private const float FadeOutStart = 0.65f;  // с 65% — растворение (задержка перед исчезновением)
    private const float StartOffset = 0.2f;    // старт чуть выше центра игрока, чтобы крест выходил из корпуса

    private SpriteRenderer spriteRenderer;
    private float duration;
    private float riseSpeed;
    private float life;

    /// <summary>
    /// Создаёт эффект на позиции цели. Если sprite == null — берётся Resources/Effects/HealCross.png
    /// </summary>
    public static void Spawn(Transform target, Sprite sprite, float effectDuration, float riseSpeed)
    {
        if (target == null) return;

        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>("Effects/HealCross");
        }

        if (sprite == null)
        {
            Debug.LogWarning("HealEffect: спрайт не найден (Resources/Effects/HealCross.png)");
            return;
        }

        var go = new GameObject("HealEffect");
        var effect = go.AddComponent<HealEffect>();

        effect.spriteRenderer = go.AddComponent<SpriteRenderer>();
        effect.spriteRenderer.sprite = sprite;

        // "За игроком": тот же слой сортировки, что у его спрайта, порядок на 1 ниже —
        // игрок рисуется поверх креста, пока тот не уйдёт вверх
        var targetRenderer = target.GetComponentInChildren<SpriteRenderer>();
        if (targetRenderer != null)
        {
            effect.spriteRenderer.sortingLayerID = targetRenderer.sortingLayerID;
            effect.spriteRenderer.sortingOrder = targetRenderer.sortingOrder - 1;
        }
        else
        {
            effect.spriteRenderer.sortingOrder = 99;
        }

        // Пиксель-арт: гарантируем резкость (спрайт из Resources импортируется с Bilinear)
        if (sprite.texture != null)
        {
            sprite.texture.filterMode = FilterMode.Point;
        }

        effect.duration = Mathf.Max(0.1f, effectDuration);
        effect.riseSpeed = Mathf.Max(0f, riseSpeed);

        go.transform.position = target.position + Vector3.up * StartOffset;
    }

    private void Update()
    {
        // Ровно вверх, без покачивания и без привязки к игроку
        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);

        life += Time.deltaTime;

        float t = life / duration;
        float alpha;

        if (t < FadeInPart)
        {
            alpha = t / FadeInPart;
        }
        else if (t > FadeOutStart)
        {
            alpha = Mathf.Clamp01(1f - (t - FadeOutStart) / (1f - FadeOutStart));
        }
        else
        {
            alpha = 1f;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

        if (life >= duration)
        {
            Destroy(gameObject);
        }
    }
}
