using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    private const float NON_TRANSPARENT = 1f;

    [Range(0f, 1f)]

    [SerializeField] private float transparancyAmount = 0.5f;
    [SerializeField] private float transparancySpeed = 0.5f;
    SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.GetComponent<Player>())
        {
            if(collider is CapsuleCollider2D)
                StartCoroutine(FadeRoutine(spriteRenderer, spriteRenderer.color.a, transparancyAmount, transparancySpeed ));
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.GetComponent<Player>())
        {
            if(collider is CapsuleCollider2D)
                StartCoroutine(FadeRoutine(spriteRenderer, spriteRenderer.color.a, transparancyAmount, NON_TRANSPARENT ));
        }
    }

    private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer, float transparancySpeed, float startTransparencyAmount, float targetTransparencyAmount)
    {
        float elapsedTime = 0f;
        while(elapsedTime < transparancySpeed)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startTransparencyAmount, targetTransparencyAmount, elapsedTime/transparancySpeed);
            spriteRenderer.color = new Color(spriteRenderer.color.r,spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);

            yield return null;

        }

    }


}
