using UnityEngine;
using System;
using Unity.VisualScripting;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }
    [SerializeField] private Sword activeSword;
    [SerializeField] private Sword nonActiveSword;

    private int currentSword;

    void Start()
    {
        currentSword = PlayerPrefs.GetInt("currentWeapon");
    }

    private void switchWeapon()
    {
        activeSword.gameObject.SetActive(false);
        nonActiveSword.gameObject.SetActive(true);
    }


    private void Awake()
    {
        Instance = this;
        if (currentSword == 2)
        {
            switchWeapon();
        }
    }

    private void Update()
    {
        if (Player.Instance.IsAlive())
            FollowMousePos();
    }

    public Sword GetActiveWeapon()
    {
        return activeSword;
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
