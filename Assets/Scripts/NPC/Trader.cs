using UnityEngine;
using UnityEngine.UI;

public class Trader : MonoBehaviour
{
    public Button buyButton;
    public GameObject shopPanel;
    private Animator animator;
    private Collider2D _collider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider2D>();
        shopPanel.SetActive(false);
        buyButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        shopPanel.SetActive(false);
        buyButton.gameObject.SetActive(false);

        buyButton.onClick.AddListener(OpenShop);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        {
            buyButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider)
        {
            buyButton.gameObject.SetActive(false);
            shopPanel.SetActive(false);
        }
    }

    private void OpenShop()
    {
        shopPanel.SetActive(true);
    }



}