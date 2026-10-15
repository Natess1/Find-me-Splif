using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthPurchase : MonoBehaviour
{
    [Header("Purchase Settings")]
    [SerializeField] private int price = 50;
    [SerializeField] private int healthToRestore = 3;

    [Header("Button")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buttonText;

    [Header("Texts")]
    [SerializeField] private string enoughMoneyText = "Пополнить здоровье на {0} за {1} монет";
    [SerializeField] private string notEnoughMoneyText = "Недостаточно монет";
    [SerializeField] private string fullHealthText = "Здоровье уже полностью восстановлено";

    [Header("Button Position")]
    [SerializeField] private Vector3 buttonOffset = new Vector3(0f, 1.5f, 0f);

    private Player player;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        buyButton.gameObject.SetActive(false);
        buyButton.onClick.AddListener(BuyHealth);
    }

    private void Update()
    {
        if (player != null && buyButton.gameObject.activeSelf)
        {
            UpdateButtonPosition();
            UpdateButton();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player enteredPlayer = other.GetComponent<Player>();

        if (enteredPlayer == null)
            return;

        player = enteredPlayer;

        buyButton.gameObject.SetActive(true);

        UpdateButtonPosition();
        UpdateButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Player exitedPlayer = other.GetComponent<Player>();

        if (exitedPlayer == null)
            return;

        if (exitedPlayer == player)
        {
            player = null;
            buyButton.gameObject.SetActive(false);
        }
    }

    private void UpdateButtonPosition()
    {
        Vector3 worldPosition = transform.position + buttonOffset;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        buyButton.transform.position = screenPosition;
    }

    private void UpdateButton()
    {
        if (player == null)
            return;

        if (player.GetCurrentHealth() >= player.GetMaxHealth())
        {
            buttonText.text = fullHealthText;
            buyButton.interactable = false;
            return;
        }

        int money = PlayerPrefs.GetInt("money");

        if (money >= price)
        {
            buttonText.text = string.Format(
                enoughMoneyText,
                healthToRestore,
                price
            );

            buyButton.interactable = true;
        }
        else
        {
            buttonText.text = notEnoughMoneyText;
            buyButton.interactable = false;
        }
    }

    private void BuyHealth()
    {
        if (player == null)
            return;

        int money = PlayerPrefs.GetInt("money");

        if (money < price)
        {
            UpdateButton();
            return;
        }

        if (player.GetCurrentHealth() >= player.GetMaxHealth())
        {
            UpdateButton();
            return;
        }

        GameInput.Instance.RemoveMoney(price);

        player.Heal(healthToRestore);

        UpdateButton();
    }

    private void OnDestroy()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(BuyHealth);
        }
    }
}