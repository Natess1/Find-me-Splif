using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCoolDown : MonoBehaviour
{
    [SerializeField] private float coolDownTime = 1f;

    private Button btn;
    private Image btnImage;
    private Color origColor;
    private Color darkenedColor;

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
        StartCoroutine(PlayCoolDown());


    }

    private IEnumerator PlayCoolDown()
    {
        btn.interactable = false;
        btnImage.color = darkenedColor;
        yield return new WaitForSeconds(coolDownTime);

        btnImage.color = origColor;
        btn.interactable = true;


    }
}
