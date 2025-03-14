
public class EnemyAnimation : AnimationManager
{
    private EnemyAI enemyAI;
    protected override void Awake()
    {
        base.Awake();
        enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.OnAttack += PlayAttackAnimation;
            enemyAI.OnDefend += PlayDefenseAnimation;
            enemyAI.OnDeath += PlayDeathAnimation;
        }

    }
    protected override void DisconectAllDelegates()
    {
        enemyAI.OnAttack -= PlayAttackAnimation;
        enemyAI.OnDefend -= PlayDefenseAnimation;
        enemyAI.OnDeath -= PlayDeathAnimation;
    }
    private void OnDisable()
    {
        if (enemyAI != null)
        {
            DisconectAllDelegates();
        }
    }
}
