using UnityEngine;

public class FrameSwitch : MonoBehaviour
{
    public GameObject activeFrame;
    [SerializeField] private MonoBehaviour transitionObject;


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            if (collider is CapsuleCollider2D)
            {
                activeFrame.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            if (collider is CapsuleCollider2D)
            {
                activeFrame.SetActive(false);
            }
        }
    }
}
