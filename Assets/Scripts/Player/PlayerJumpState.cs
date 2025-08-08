using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.airState);

        player.SetVelocity(xInput * player.moveSpeed, player.rb.linearVelocity.y);
    }
}
