using UnityEngine;
using System;
using UnityEngine.InputSystem;


public class PlayerVisual : MonoBehaviour
{

    [SerializeField] private Joystick joystick;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private FlashBlink flashBlink;

    private static readonly int Die = Animator.StringToHash("IsDie");
    private static readonly int Running = Animator.StringToHash("IsRunning");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        flashBlink = GetComponent<FlashBlink>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }


    private void Update()
    {
        animator.SetBool(Running, Player.Instance.IsRunning());
        if (Player.Instance.IsAlive())
        {
            PlayerFacing();
        }
    }

    private void Player_OnPlayerDeath(object sender, EventArgs e)
    {
        animator.SetBool(Die, true);
        flashBlink.StopBlinking();
    }

    private void PlayerFacing()
    {
        Vector3 mousePos = GameInput.Instance.GetJoysticVector(joystick);
        Vector3 playerPos = Player.Instance.GetPlayerScreenPosition();


        spriteRenderer.flipX = mousePos.x < 0;
    }

    private void OnDestroy()
    {
        Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
    }


}
