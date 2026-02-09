using System;
using UnityEngine;

public class FlashBlink : MonoBehaviour
{
    [SerializeField] private MonoBehaviour damagebleObject;
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private float blinkDuration = 0.2f;

    private float currentBlinkTimer;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;
    private bool IsBlinking;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        IsBlinking = true;

        if(damagebleObject is Player)
        {
            (damagebleObject as Player).OnPlayerBlink += damagebleObject_OnPlayerBlink;
        }
    }

    private void damagebleObject_OnPlayerBlink(object sender, EventArgs e)
    {
        SetBlinkingMaterial();
    }

    private void Update()
    {
        if (IsBlinking)
        {
            currentBlinkTimer -= Time.deltaTime;
            if(currentBlinkTimer < 0)
            {
                setDefaultMaterials();
            }
        }
    }

    public void StopBlinking()
    {
        setDefaultMaterials();
        IsBlinking = false;
    }

    private void setDefaultMaterials()
    {
        spriteRenderer.material = defaultMaterial;
    }

    private void SetBlinkingMaterial()
    {
        currentBlinkTimer = blinkDuration;
        spriteRenderer.material = blinkMaterial;
    }




}
