using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (xInput == 0)
            stateMachine.ChangeState(player.run_to_IdleState);

        player.SetVelocity(xInput * player.moveSpeed, player.rb.linearVelocity.y);
    }
}
