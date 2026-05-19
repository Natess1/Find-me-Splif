using UnityEngine;
using UnityEngine.UI;

public class HelperText : MonoBehaviour
{
    [SerializeField] private Button helper;

    private void Awake()
    {
        helper.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            helper.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            helper.gameObject.SetActive(false);
        }
    }
}
