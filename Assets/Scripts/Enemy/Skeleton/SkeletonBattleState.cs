using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    Transform player;
    Enemy_Skeleton enemy;
    int movDir;
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        //player = GameObject.Find("Zero").transform;
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!enemy.IsGroundDetected())
            stateMachine.ChangeState(enemy.idleState);

        if(enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance && CanAttack()) 
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else if(stateTimer<0)
            stateMachine.ChangeState(enemy.idleState);

        if (player.position.x > enemy.transform.position.x)
            movDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            movDir = -1;

        enemy.SetVelocity(enemy.moveSpeed* movDir,enemy.rb.linearVelocity.y);
    }

    private bool CanAttack()
    {
        if (Time.time > enemy.lastTimeAttacked + enemy.attackCooldown)
            return true;
        return false;
    }
}
