using UnityEngine;

public class PlayerIaiState : PlayerState
{
    public PlayerIaiState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }


    private int attackWindowFrames;
    private const int maxAttackWindowFrames = 10;

    private float backwardTimer;
    private Vector2 startPosition;
    private const float backwardDuration = 0.2f;
    private const float backwardDistance = 0.8f;
    public override void Enter()
    {
        base.Enter();

        startPosition = player.transform.position;
        backwardTimer = 0f;

        stateTimer = player.iaiDuration;

        player.anim.SetBool("Iai_Slash", false);
        attackWindowFrames = 0;

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.ZeroVelocity();

        if (backwardTimer < backwardDuration)
        {
            backwardTimer += Time.deltaTime;
            float progress = Mathf.Min(1f, backwardTimer / backwardDuration);

            float easeProgress = 1 - Mathf.Pow(1 - progress, 2);

            Vector2 backwardDir = -player.facingDir * Vector2.right;

            player.transform.position = (Vector3)startPosition + (Vector3)(backwardDir * backwardDistance * easeProgress);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && attackWindowFrames == 0)
        {
            attackWindowFrames = maxAttackWindowFrames;
        }

        if (attackWindowFrames > 1)
        {
            attackWindowFrames--;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);


            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                {
                    if (hit.GetComponent<Enemy>().CanBeStunned())
                    {
                        stateTimer = 10;
                        player.anim.SetBool("Iai_Slash", true);

                        if (player.transform.position.x < hit.GetComponent<Enemy>().transform.position.x && hit.GetComponent<Enemy>().facingRight)
                            hit.GetComponent<Enemy>().Flip();
                        else if (player.transform.position.x > hit.GetComponent<Enemy>().transform.position.x && !hit.GetComponent<Enemy>().facingRight)
                            hit.GetComponent<Enemy>().Flip();

                        hit.GetComponent<Enemy>().Damage();

                        AttackScene.Instance.HitPause(player.iaiSlashHitPause);
                        AttackScene.Instance.CameraShake(player.iaiSlashShakeTime, player.iaiSlashHitMagnitude);

                        EnemyStats _target = hit.GetComponent<EnemyStats>();

                        player.stats.DoDamage(_target);
                        player.stats.DoMagicalDamage(_target);

                        attackWindowFrames = -1;
                        break;
                    }
                }
            }
        }


        if (stateTimer < 0 || triggerCalled )
            stateMachine.ChangeState(player.idleState);
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.Instance.dash.CanUseSkill())
        {
            player.dashDir = Input.GetAxisRaw("Horizontal");
            if (player.dashDir == 0)
                player.dashDir = player.facingDir;
            player.stateMachine.ChangeState(player.dashState);
        }
    }
}
