using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] private GameObject MovigCamera;
    public float paralax = 0.5f;
    private float startingPosition;

    private void Start()
    {
        startingPosition = transform.position.x;
    }

    private void Update()
    {
        float distance = (MovigCamera.transform.position.x * (1 - paralax));
        transform.position = new Vector3(startingPosition + distance, transform.position.y, transform.position.z);
    }

}
