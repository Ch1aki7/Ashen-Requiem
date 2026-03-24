using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
        if (Input.GetKeyDown(KeyCode.R) && SkillManager.Instance.blackhole.CanUseSkill())  
            stateMachine.ChangeState(player.blackholeState);

        if (Input.GetKeyDown(KeyCode.Mouse1))
            stateMachine.ChangeState(player.iaiState);

        if(!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        if (Input.GetKeyDown(KeyCode.O) && SkillManager.Instance.oath.CanUseSkill())
        {
            SkillManager.Instance.oath.UseSkill();
            SkillManager.Instance.oath.SetSkillCD();
            stateMachine.ChangeState(player.oathState);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            stateMachine.ChangeState(player.kickState); 
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            stateMachine.ChangeState(player.sleepState);
        }

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }
}
