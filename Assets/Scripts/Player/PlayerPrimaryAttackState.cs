using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 0.3f;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        SkillManager.Instance.slashEffect.UseSkill();

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        player.SetVelocity(player.attackMovement[comboCounter].x * player.facingDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();

        comboCounter++;
        lastTimeAttacked = Time.time;

    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed, player.rb.linearVelocity.y);
        //if (stateTimer < 0)
        //    player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.rb.linearVelocity.y);
        //player.SetVelocity(0, 0);
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

}
