using UnityEngine;

public class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;

    private string animBoolName;

    protected bool triggerCalled;
    protected float stateTimer;

    public EnemyState(Enemy _enemyBase,EnemyStateMachine _stateMachine,string _animBoolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        triggerCalled = false;
        enemyBase.anim.SetBool(animBoolName, true);
    }

    public virtual void Exit()
    {

        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimBoolName(animBoolName);
    }
    public virtual void Update()
    {
        stateTimer-= Time.deltaTime;
    }
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

}
