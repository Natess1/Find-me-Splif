using System;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private EnemyEntity boss;
    [SerializeField] private GameObject[] door;

    void Start()
    {
        boss = GetComponent<EnemyEntity>();
        boss.OnBossDeath += Boss_OnBossDeath;
    }

    private void Boss_OnBossDeath(object sender, EventArgs e)
    {
        OpenDoor();
    }

    private void OpenDoor()
    {
        foreach (var item in door)
        {
            Destroy(item);
        }

    }
}
