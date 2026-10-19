using UnityEngine;

public class CameraParallax : MonoBehaviour
{
    [SerializeField] public float cameraSpeed = 2f;
    [SerializeField] public float movingTime = 3f;

    private float timer = 0f;
    private int direction = 1;

    void Update()
    {
        transform.position += Vector3.right * direction * cameraSpeed * Time.deltaTime;

        timer += Time.deltaTime;

        if (timer >= movingTime)
        {
            direction *= -1;
            timer = 0f;
        }

    }
}