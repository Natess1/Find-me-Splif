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



    SpriteRenderer spriteRenderer;

    private static readonly int Running = Animator.StringToHash("IsRunning");
    private static readonly int ChasingSpeedMultiplier = Animator.StringToHash("ChaisingSpeedMultiplier");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int TakeHit = Animator.StringToHash("TakeHit");
    private static readonly int Die = Animator.StringToHash("IsDie");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(Running, enemyAI.IsRunning);
        animator.SetFloat(ChasingSpeedMultiplier, enemyAI.GetRoamingSpeed());
    }

    private void Start()
    {
        enemyAI.OnEnemyAttack += enemyAI_OnEnemyAttack;
        enemyEntity.OnTakeHit += enemyEntity_OnTakeHit;
        enemyEntity.OnDeath += enemyEntity_OnDeath;
    }

    private void enemyEntity_OnDeath(object sender, EventArgs e)
    {
        animator.SetBool(Die, true);
        spriteRenderer.sortingOrder = -1;
        sceletonShadow.SetActive(false);
    }

    private void enemyEntity_OnTakeHit(object sender, EventArgs e)
    {
        animator.SetTrigger(TakeHit);
        
    }

    private void OnDestroy()
    {
        enemyAI.OnEnemyAttack -= enemyAI_OnEnemyAttack;
        enemyEntity.OnTakeHit -= enemyEntity_OnTakeHit;
        enemyEntity.OnDeath -= enemyEntity_OnDeath;
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
        
        animator.SetTrigger(Attack);
    }
}
