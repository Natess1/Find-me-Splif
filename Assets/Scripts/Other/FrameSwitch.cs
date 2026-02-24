using UnityEngine;

public class FrameSwitch : MonoBehaviour
{
    public GameObject activeFrame;
    [SerializeField] private MonoBehaviour transitionObject;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (transitionObject is Player)
        {
            activeFrame.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (transitionObject is Player)
        {
            activeFrame.SetActive(false);
        }
    }
}
