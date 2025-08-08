using UnityEngine;

public class PlayerRun_to_IdleState : PlayerGroundedState
{
    public PlayerRun_to_IdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed, player.rb.linearVelocity.y);

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
