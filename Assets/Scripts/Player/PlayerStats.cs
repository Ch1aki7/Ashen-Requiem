public class PlayerStats : CharacterStats
{
    private Player player;
    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    protected override void Update()
    {
        base.Update();

        if (isShocked && player.stats.currentHP > 0)
            player.stateMachine.ChangeState(player.shockedState);
    }
    protected override void Die()
    {
        base.Die();

        player.Die();
    }
}
