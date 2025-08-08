using UnityEngine;

public class PlayerDragonState : PlayerState
{
    public PlayerDragonState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.GOD = true;
        AttackScene.Instance.UseTimeShrink(30);


    }

    public override void Exit()
    {
        base.Exit();

        player.SetVelocity(0, player.rb.linearVelocity.y);

        player.GOD = false;
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashDir * player.dashSpeed * 3, 0);

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
