using UnityEngine;

public class PlayerDashState : PlayerGroundedState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        SkillManager.Instance.shadow.CreateShadow(player.transform,new Vector3());

        stateTimer = player.dashDuration;
        
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, player.rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(player.dashDir * player.dashSpeed, 0);
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
