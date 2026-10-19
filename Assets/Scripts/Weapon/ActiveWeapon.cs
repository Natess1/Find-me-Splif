using UnityEngine;
using System;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }
    [SerializeField] private Sword activeSword;
    [SerializeField] private Joystick joystick;

    private Sword nonActiveSword;

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
        // Оружие и его хитбокс разворачиваются по Player.FacingLeft — туда же, куда смотрит спрайт.
        // Раньше тут читался сырой джойстик: при отпущенном (x = 0) оружие отворачивалось вправо,
        // хотя персонаж оставался смотреть влево — и удар шёл не в ту сторону.
        transform.rotation = Player.Instance.FacingLeft
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.Euler(0f, 0f, 0f);
    }


}
