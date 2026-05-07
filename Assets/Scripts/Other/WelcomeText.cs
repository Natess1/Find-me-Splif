using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float visibleTime = 3f;
    [SerializeField] private string locationName;

    private void Start()
    {
        text.text = locationName;
        Color _color = text.color;
        _color.a = 0f;
        text.color = _color;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Coroutine fadeRoutine = StartCoroutine(ShowAndDestroy());
        }
    }



    private IEnumerator ShowAndDestroy()
    {
        yield return StartCoroutine(FadeText(0f, 1f, fadeDuration));

        yield return new WaitForSeconds(visibleTime);

        yield return StartCoroutine(FadeText(1f, -1f, fadeDuration));

        Destroy(this.gameObject);
    }

    private IEnumerator FadeText(float fromAlpha, float ToAlpha, float duration)
    {
        Color color = text.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            color.a = Mathf.Lerp(fromAlpha, ToAlpha, t);

            text.color = color;

            yield return null;
        }

    }

}
