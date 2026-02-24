using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    [SerializeField] private TMP_Text moneyCounter;
    [SerializeField] private TMP_Text heartCounter;

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerDash;
    public event EventHandler OnPlayerLoot;

    private PlayerInputActions playerInputActions;

    private int money = 0;

    private int currentMoney;

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
        moneyCounter.text = PlayerPrefs.GetInt("money").ToString();
        heartCounter.text = PlayerPrefs.GetInt("health").ToString();
    }
    public int GetMoney()
    {
        Debug.Log(money);
        return money;
    }

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

    public void DisableMovement()
    {
        playerInputActions.Disable();
    }

    public void ActivateMovement()
    {
        playerInputActions.Enable();
    }


    public Vector2 GetMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }


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
