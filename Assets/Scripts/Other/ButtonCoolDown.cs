using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCoolDown : MonoBehaviour
{
    [SerializeField] private float coolDownTime = 1f;

    private Button btn;
    private Image btnImage;
    private Color origColor;
    private Color darkenedColor;
    private Coroutine cooldownRoutine;

    void Start()
    {
        btn = GetComponent<Button>();
        btnImage = GetComponent<Image>();
        origColor = btnImage.color;
        darkenedColor = new Color(origColor.r * 0.5f, origColor.g * 0.5f, origColor.b * 0.5f, origColor.a);

        btn.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // если предыдущий кулдаун ещё не кончился — перезапускаем его, а не наслаиваем корутины
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
        }

        cooldownRoutine = StartCoroutine(PlayCoolDown());
    }

    private IEnumerator PlayCoolDown()
    {
        btn.interactable = false;
        btnImage.color = darkenedColor;
        yield return new WaitForSeconds(coolDownTime);

        cooldownRoutine = null;

        btnImage.color = origColor;
        btn.interactable = true;
    }

    private void OnDisable()
    {
        // Кнопку выключили посреди кулдауна (смена кадра аниматора, пауза и т.п.):
        // корутина остановится сама, иначе кнопка навсегда останется потухшей и некликабельной.
        cooldownRoutine = null;

        if (btn != null)
        {
            btn.interactable = true;
        }

        if (btnImage != null && origColor != default)
        {
            btnImage.color = origColor;
        }
    }
}
