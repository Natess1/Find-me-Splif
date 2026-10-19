using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    [SerializeField] private TMP_Text moneyCounter;
    [SerializeField] private TMP_Text heartCounter;
    [SerializeField] private TMP_Text timerText;   // опционально: таймер забега
    [SerializeField] private TMP_Text levelText;   // опционально: текущий уровень
    [SerializeField] private Joystick joystick;

    [Header("Debug: текущие значения (можно менять прямо в Play Mode)")]
    [SerializeField] private int debugCurrentMoney;   // текущие деньги (не заработок) — редактируется в инспекторе

#if UNITY_EDITOR
    private int lastDebugMoney;
    private bool debugMoneyInited;
#endif

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerDash;
    public event EventHandler OnPlayerLoot;

    private PlayerInputActions playerInputActions;

    private int money = 0;


    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();


        playerInputActions.Combat.Attack.started += PlayerAttack_started;

        playerInputActions.Player.Loot.started += PlayerLoot_started;

        playerInputActions.Player.Dash.performed += PlayerDashPerformed;
    }



    void Update()
    {
#if UNITY_EDITOR
        SyncDebugMoney();
#endif

        moneyCounter.text = PlayerPrefs.GetInt("money").ToString();
        heartCounter.text = PlayerPrefs.GetInt("health").ToString();

        if (timerText != null)
        {
            timerText.text = LevelSystem.Instance != null
                ? LevelSystem.FormatTime(LevelSystem.Instance.ElapsedTime)
                : "00:00";
        }

        if (levelText != null)
        {
            levelText.text = LevelSystem.Instance != null
                ? "lvl. " + LevelSystem.Instance.CurrentLevel
                : "lvl. 1";
        }
    }
    public int GetMoney()
    {
        Debug.Log(money);
        return money;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Поле "Debug Current Money" в инспекторе = ручной ввод текущих денег.
    /// Изменили руками — значение сразу уходит в PlayerPrefs (и в HUD).
    /// Если деньги изменил геймплей (подбор монет) — поле само обновляется из PlayerPrefs.
    /// В билд этот код не попадает (#if UNITY_EDITOR).
    /// </summary>
    private void SyncDebugMoney()
    {
        int actual = PlayerPrefs.GetInt("money");

        if (!debugMoneyInited)
        {
            debugMoneyInited = true;
            lastDebugMoney = actual;
            debugCurrentMoney = actual;
            return;
        }

        if (debugCurrentMoney != lastDebugMoney)
        {
            lastDebugMoney = Mathf.Max(0, debugCurrentMoney);
            debugCurrentMoney = lastDebugMoney;
            PlayerPrefs.SetInt("money", lastDebugMoney);
        }
        else if (actual != lastDebugMoney)
        {
            lastDebugMoney = actual;
            debugCurrentMoney = actual;
        }
    }
#endif

    public void AddMoney(int addingMoney)
    {
        money = PlayerPrefs.GetInt("money");
        PlayerPrefs.SetInt("money", money + addingMoney);
    }

    public void RemoveMoney(int removingMoney)
    {
        money = PlayerPrefs.GetInt("money");
        PlayerPrefs.SetInt("money", money - removingMoney);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector2 GetJoysticVector(Joystick joystick)
    {
        Vector2 JoysticVector = new(joystick.Horizontal, joystick.Vertical);
        return JoysticVector;
    }

    public void DisableMovement()
    {
        playerInputActions.Disable();
    }

    public void EnableMovement()
    {
        playerInputActions.Enable();
    }


    public Vector2 GetMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

    // public Vector2 GetJoystickPosition()
    // {
    //     Vector2 joystickPos = 
    //     return joystickPos;
    // }


    private void PlayerLoot_started(InputAction.CallbackContext context)
    {
        OnPlayerLoot?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerDashPerformed(InputAction.CallbackContext context)
    {
        OnPlayerDash?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerAttack_started(InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }

}
