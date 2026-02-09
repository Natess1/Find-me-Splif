using UnityEngine;
using System;

public class ActiveWeapon : MonoBehaviour
{

    public static ActiveWeapon Instance { get; private set; }
    [SerializeField] private Sword sword;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(Player.Instance.IsAlive())
            FollowMousePos();
    }

    public Sword GetActiveWeapon()
    {
        return sword;
    }

    private void FollowMousePos()
    {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPos = Player.Instance.GetPlayerScreenPosition();

        if (mousePos.x < playerPos.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
