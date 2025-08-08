using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    private Animator hitFXAnimator;
    public LayerMask whatisPlayer;

    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDir;
    public bool canbeStunned;

    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    private float defaultMoveSpeed;

    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;
    public float battleTime;
    [SerializeField] protected GameObject counterImage;
    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }


    protected override void Start()
    {
        base.Start();
        hitFXAnimator=transform.Find("HitFX").GetComponentInChildren<Animator>();
    }
    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

    }

    public virtual void AssignLastAnimBoolName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }

    public override void Flip()
    {
        base.Flip();
        counterImage.transform.Rotate(0, 180, 0);
    }
    
    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed= 0f;
            anim.speed= 0f;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    public virtual IEnumerator FreezeTimeFor(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    public override void Damage()
    {
        base.Damage();
        hitFXAnimator.SetTrigger("Hit");
    }

    public virtual bool CanBeStunned()
    {
        if (canbeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }
    public virtual void OpenCounterAttackWindow()
    {
        canbeStunned = true;
    }

    public virtual void CloseCounterAttackWindow()
    {
        canbeStunned = false;
    }

    public virtual void OpenDangerWindow()
    {
        counterImage.SetActive(true);
    }

    public virtual void CloseDangerWindow()
    {
        counterImage.SetActive(false);
    }
    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 50, whatisPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }

}
