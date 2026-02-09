using UnityEditor.Rendering.Universal;
using UnityEngine;

public class SwordSplashVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;

    private const string ATTACK = "Attack";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        sword.OnSwordSWing += Sword_OnSwordSWing;
    }

    private void Sword_OnSwordSWing(object sender, System.EventArgs e)
    {
        animator.SetTrigger(ATTACK);
    }
}
