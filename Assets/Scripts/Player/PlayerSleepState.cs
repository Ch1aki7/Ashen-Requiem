using UnityEngine;

public class PlayerSleepState : PlayerState
{
    public PlayerSleepState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if(xInput != 0)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
