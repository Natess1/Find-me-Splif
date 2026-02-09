using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SceletonVisual : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyEntity enemyEntity;
    [SerializeField] private GameObject sceletonShadow;

    private Animator animator;

    private const string IS_RUNNING = "IsRunning";
    private const string CHAISING_SPEED_MULTIPLIER = "ChaisingSpeedMultiplier";
    private const string ATTACK = "Attack";
    private const string TAKE_HIT = "TakeHit";
    private const string IS_DIE = "IsDie";

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, enemyAI.IsRunning);
        animator.SetFloat(CHAISING_SPEED_MULTIPLIER, enemyAI.GetRoamingSpeed());
    }

    private void Start()
    {
        enemyAI.OnEnemyAttack += enemyAI_OnEnemyAttack;
        enemyEntity.OnTakeHit += enemyEntity_OnTakeHit;
        enemyEntity.OnDeath += enemyEntity_OnDeath;
    }

    private void enemyEntity_OnDeath(object sender, EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
        spriteRenderer.sortingOrder = -1;
        sceletonShadow.SetActive(false);
    }

    private void enemyEntity_OnTakeHit(object sender, EventArgs e)
    {
        animator.SetTrigger(TAKE_HIT);
        
    }

    private void OnDestroy()
    {
        enemyAI.OnEnemyAttack -= enemyAI_OnEnemyAttack;
    }

    public void TriggerAttackAnimOff()
    {
        enemyEntity.PolygonColliderOff();
    }

    public void TriggerAttackAnimOn()
    {
        enemyEntity.PolygonColliderOn();
    }

    private void enemyAI_OnEnemyAttack(object sender, EventArgs e)
    {
        animator.SetTrigger(ATTACK);
    }
}
