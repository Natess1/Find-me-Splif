using UnityEngine;

public class FrameSwitch : MonoBehaviour
{
    [SerializeField] private MonoBehaviour transitionObject;
    public GameObject activeFrame;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            if (collider is CapsuleCollider2D)
            {
                activeFrame.SetActive(true);
                Debug.Log("Player entered frame switch trigger");
                NavSurfManagment.Instance.RebakeNavMashSurface();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            if (collider is CapsuleCollider2D)
            {
                NavSurfManagment.Instance.RebakeNavMashSurface();
                activeFrame.SetActive(false);
            }
        }
    }
}
