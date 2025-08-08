using UnityEngine;
using UnityEngine.Playables;

public class PlayerDeadState : PlayerState
{
    
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.backDir = -player.facingDir;

        stateTimer = .5f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            player.rb.linearVelocity = new Vector2(5 * player.backDir, 0);
        }
    }
}
