using UnityEditor.Rendering.Universal;
using UnityEngine;

public class SwordSplashVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;

    private Animator animator;

    private static readonly int Attack = Animator.StringToHash("Attack");

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
        animator.SetTrigger(Attack);
    }

    private void OnDestroy()
    {
        sword.OnSwordSWing -= Sword_OnSwordSWing;

    }
}
