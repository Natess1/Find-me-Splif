using UnityEngine;
using System;
using UnityEditor.Build.Content;

public class Sword : MonoBehaviour
{
    [SerializeField] private int damageAmount = 2;

    public event EventHandler OnSwordSWing;

    private PolygonCollider2D polygonCollider2d;



    private void Awake()
    {
        polygonCollider2d = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        AttackColliderOf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
        }
    }

    public void Attack()
    {
        AttackColliderOfOn();

        OnSwordSWing?.Invoke(this, EventArgs.Empty);
    }

    public void AttackColliderOf()
    {
        polygonCollider2d.enabled = false;
    }

    private void AttackColliderOn()
    {
        polygonCollider2d.enabled = true;
    }

    private void AttackColliderOfOn()
    {
        AttackColliderOf();
        AttackColliderOn();
    }



}
