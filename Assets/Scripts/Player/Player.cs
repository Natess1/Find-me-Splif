using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System;
using System.Collections;
using UnityEditor;
using NUnit.Framework.Constraints;

[SelectionBase]
public class Player : Sounds
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPlayerDeath;
    public event EventHandler OnPlayerBlink;

    [Header("Player Settings")]
    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageRecoveryTime = 0.5f;
    [SerializeField] private float attackCoolDownTime = 0.3f;

    [Header("Dash Settings")]
    [SerializeField] private int dashSpeed = 5;
    [SerializeField] private float dashTime = 0.4f;
    [SerializeField] private float dashCoolDownTime = 3f;

    [Header("Other")]
    [SerializeField] private TrailRenderer trailRenderer;



    Vector2 InputVector;
    private Rigidbody2D rigidBody;
    private KnockBack KnockBack;
    private Camera mainCamera;

    private readonly float minMovingSpeed = 0.1f;
    private float startMovingSpeed;
    private int currentHealth;
    private int savingHealth;

    private bool canTakeDamage;
    private bool isRunning = false;
    private bool isAlive;
    private bool isDashing;
    private bool canAttack;


    private void Awake()
    {
        Instance = this;
        currentHealth = PlayerPrefs.GetInt("health");


        rigidBody = GetComponent<Rigidbody2D>();
        KnockBack = GetComponent<KnockBack>();
        mainCamera = Camera.main;

        startMovingSpeed = movingSpeed;
    }

    private void Update()
    {
        InputVector = GameInput.Instance.GetMovementVector();
        savingHealth = currentHealth;

        PlayerPrefs.SetInt("health", savingHealth);
    }

    private void FixedUpdate()
    {
        if (KnockBack.isGettingBack)
        {
            return;
        }
        HandleMovment();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        canTakeDamage = true;
        canAttack = true;
        isAlive = true;

        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
    }

    private void GameInput_OnPlayerDash(object sender, EventArgs e)
    {
        Dash();
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsAlive() => isAlive;


    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 screenPlayerPos = mainCamera.WorldToScreenPoint(transform.position);

        return screenPlayerPos;
    }


    public void TakeDamage(Transform damageSource, int damage)
    {
        if (canTakeDamage && isAlive)
        {
            canTakeDamage = false;
            currentHealth = Mathf.Max(0, currentHealth -= damage);
            KnockBack.GetKnockBack(damageSource);

            OnPlayerBlink?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryCoroutine());
        }
        savingHealth = currentHealth;

        DetectDeath();
    }

    private void Dash()
    {
        if (!isDashing)
        {
            if (Instance != null)
            {
                StartCoroutine(DashRoutine());
            }
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        movingSpeed *= dashSpeed;
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);


        trailRenderer.emitting = false;
        movingSpeed = startMovingSpeed;
        yield return new WaitForSeconds(dashCoolDownTime);
        isDashing = false;
    }

    private void DetectDeath()
    {
        if (currentHealth == 0 && isAlive)
        {
            isAlive = false;
            int allMoney = PlayerPrefs.GetInt("money");
            GameInput.Instance.RemoveMoney(allMoney);

            KnockBack.StopKnockBackMov();
            GameInput.Instance.DisableMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            SceneTransition.SwitchToScene("DeathScene");
        }
        savingHealth = currentHealth;

    }

    private IEnumerator DamageRecoveryCoroutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void HandleMovment()
    {

        rigidBody.MovePosition(rigidBody.position + InputVector * (movingSpeed * Time.deltaTime));

        if (Mathf.Abs(InputVector.x) > minMovingSpeed || Mathf.Abs(InputVector.y) > minMovingSpeed)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        if (canAttack)
        {
            ActiveWeapon.Instance.GetActiveWeapon().Attack();
            StartCoroutine(AttackCoolDownTime());
        }
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
    }

    private IEnumerator AttackCoolDownTime()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCoolDownTime);
        canAttack = true;
    }

}
