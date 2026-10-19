using UnityEngine;
using System;

public class Sword : MonoBehaviour
{
    [Header("Урон")]
    [SerializeField] private int baseDamage = 2;
    [SerializeField] private bool scaleWithLevel = true;   // +урон за каждый уровень игрока

    /// <summary>Итоговый урон: база + бонус за уровень (LevelSystem).</summary>
    private int damageAmount
    {
        get
        {
            if (!scaleWithLevel || LevelSystem.Instance == null)
            {
                return baseDamage;
            }

            return baseDamage + (LevelSystem.Instance.CurrentLevel - 1) * LevelSystem.Instance.SwordBonusPerLevel;
        }
    }

    private PolygonCollider2D polygonCollider2d;

    public event EventHandler OnSwordSWing;

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
