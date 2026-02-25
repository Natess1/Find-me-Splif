using UnityEngine;
using UnityEngine.UI;

public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private MonoBehaviour transitionObject;

    public Button TradeButton;

    private Animator animator;

    void Awake()
    {
        animator = TradeButton.GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is CapsuleCollider2D)
        {
            if (transitionObject is Player player)
            {
                animator.SetTrigger("isTrigger");
            }
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision is CapsuleCollider2D)
        {
            if (transitionObject is Player player)
            {
                animator.SetTrigger("isTrigger");
            }

        }
    }
}
