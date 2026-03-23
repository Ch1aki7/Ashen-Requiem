 using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float iaiDuration;
    public GameObject slashEffectPrefab;
    public float shakeTime;
    public int hitPause;
    public float hitMagnitude;
    public float iaiSlashShakeTime;
    public int iaiSlashHitPause;
    public float iaiSlashHitMagnitude;

    [Header("Move info")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir;

    [Header("MODE")]
    public bool GOD = false;


    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerDefendState defendState { get; private set; }
    public PlayerHurtState hurtState { get; private set; }
    public PlayerIaiState iaiState { get; private set; }
    public PlayerIdle_to_RunState idle_to_runState { get; private set; }
    public PlayerRun_to_IdleState run_to_IdleState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDragonState dragonState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerShockedState shockedState { get; private set; }
    public PlayerOathState oathState { get; private set; }
    public PlayerKickState kickState { get; private set; }
    public PlayerSleepState sleepState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        defendState = new PlayerDefendState(this, stateMachine, "Defend");
        hurtState = new PlayerHurtState(this, stateMachine, "Hurt");
        iaiState = new PlayerIaiState(this, stateMachine, "Iai");
        idle_to_runState = new PlayerIdle_to_RunState(this, stateMachine, "Idle_to_Run");
        run_to_IdleState = new PlayerRun_to_IdleState(this, stateMachine, "Run_to_Idle");
        blackholeState = new PlayerBlackholeState(this, stateMachine, "Jump");
        dragonState = new PlayerDragonState(this, stateMachine, "Dragon");
        deadState = new PlayerDeadState(this, stateMachine, "Dead");
        shockedState = new PlayerShockedState(this, stateMachine, "Shocked");
        oathState = new PlayerOathState(this, stateMachine, "Oath");
        kickState = new PlayerKickState(this, stateMachine, "Kick");
        sleepState = new PlayerSleepState(this, stateMachine, "Sleep");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
        DragonApproaching();
    }

    public void ExitBlackhole()
    {
        stateMachine.ChangeState(airState);
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
    private void CheckForDashInput()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.Instance.dash.CanUseSkill()) 
        {
            SkillManager.Instance.dash.SetSkillCD();
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
                dashDir = facingDir;
            stateMachine.ChangeState(dashState);
        }
    }

    private void DragonApproaching()
    {
        if(Input.GetKeyDown(KeyCode.RightShift)&&SkillManager.Instance.dragon.CanUseSkill())
        {
            SkillManager.Instance.dragon.SetSkillCD();
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
                dashDir = facingDir;
            stateMachine.ChangeState(dragonState);
        }
    }

    public override void Damage()
    {
        base.Damage();
        //stateMachine.ChangeState(hurtState);
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }


}
