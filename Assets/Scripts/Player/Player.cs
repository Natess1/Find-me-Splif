using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System;
using System.Collections;

[SelectionBase]
public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }

    public event EventHandler OnPlayerDeath;

    public event EventHandler OnPlayerBlink;

    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageRecoveryTime = 0.5f;


    Vector2 InputVector;

    private Rigidbody2D rb;

    private KnockBack KnockBack;

    private float minMovingSpeed = 0.1f;

    private int currentHealth;

    private bool canTakeDamage;

    private bool isRunning = false;
    private bool isAlive;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Instance = this;
        KnockBack = GetComponent<KnockBack>();
    }

    private void Update()
    {
        InputVector = GameInput.Instance.GetMovementVector();
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
        isAlive = true;


        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 screenPlayerPos = Camera.main.WorldToScreenPoint(transform.position);

        return screenPlayerPos;
    }

    public bool IsAlive() => isAlive;



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

        DetectDeath();
    }

    private void DetectDeath()
    {
        if(currentHealth == 0 && isAlive)
        {
            isAlive = false;

            KnockBack.StopKnockBackMov();
            GameInput.Instance.DisableMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator DamageRecoveryCoroutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void HandleMovment()
    {

        rb.MovePosition(rb.position + InputVector * (movingSpeed * Time.deltaTime));

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
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

}
