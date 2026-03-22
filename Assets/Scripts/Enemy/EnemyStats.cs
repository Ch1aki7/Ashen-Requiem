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

    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();
    }

    public override void ApplyAilments(bool _ignite, bool _chill, bool _shock, out ElementType element)
    {
        base.ApplyAilments(_ignite, _chill, _shock,out element);

        //if (isChilled && enemy.stats.currentHP > 0)
        //    StartCoroutine(enemy.FreezeTimeFor(2f));
        //else if (isChilled && enemy.stats.currentHP <= 0)
        //    StartCoroutine(enemy.FreezeTimeFor(0f));

    }
}
