public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
    }

    protected override void Update()
    {
        base.Update();

        if (isChilled && enemy.stats.currentHP > 0)
            StartCoroutine(enemy.FreezeTimeFor(2f));
        else if (isChilled && enemy.stats.currentHP <= 0)
            StartCoroutine(enemy.FreezeTimeFor(0f));
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();
    }
}
