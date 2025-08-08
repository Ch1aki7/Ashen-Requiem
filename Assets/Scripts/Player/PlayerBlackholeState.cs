using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;
        skillUsed = false;
        stateTimer = flyTime;
        player.rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            player.rb.linearVelocity = new Vector2(0, 15);

        if (stateTimer < 0)
        {

            player.rb.linearVelocity = new Vector2(0, -.1f);

            if (!skillUsed)
            {

                SkillManager.Instance.blackhole.UseSkill();
                Debug.Log("Trace ON!");
                skillUsed = true;

            }
        }
    }
}
