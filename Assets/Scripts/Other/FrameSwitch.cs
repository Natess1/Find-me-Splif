using UnityEngine;

public class FrameSwitch : MonoBehaviour
{
    [SerializeField] private MonoBehaviour transitionObject;
    public GameObject activeFrame;

    private const string OPPENING = "openingLocate";
    private const string CLOSING = "closingLocate";

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
