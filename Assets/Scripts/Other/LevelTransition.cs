using System.Diagnostics;
using Unity.VectorGraphics;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private MonoBehaviour transitionObject;
    public string ToScaneName;

    private PolygonCollider2D polygonCollider2d;

    private void Awake()
    {
        polygonCollider2d = GetComponent<PolygonCollider2D>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (transitionObject is Player player)
        {
            SceneTransition.SwitchToScene(ToScaneName);
        }
    }

}
