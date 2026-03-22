using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    public Transform kickCheck;
    public float kickCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsWall;

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDir;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    #endregion
    public int facingDir { get; private set; } = 1;
    public bool facingRight = true;
    public int backDir;
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D capsuleCollider { get; private set; }

    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponentInChildren<EntityFX>();
        stats = GetComponent<CharacterStats>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    protected virtual void Update()
    {

    }

    // ÊÜ»÷Ð§¹û
    public virtual void Damage()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
        if (stats.isShocked)
        {
            fx.FlashElementHit(ElementType.Lightning);
            stats.curShockCharge += 1;
            if(stats.curShockCharge >= stats.maxShockCharge)
            {
                fx.ThunderStrike();
                stats.TakeDamage(200);
                stats.curShockCharge = 0;
            }
        }
        if (stats.isChilled)
        {
            fx.FlashElementHit(ElementType.Ice);
            stats.curFreezeCharge += 1;
            if(stats.curFreezeCharge >= stats.maxFreezeCharge)
            {
                fx.IceBurst();
                stats.TakeDamage(100);
                stats.ApplyResistanceBuff(ElementType.Fire, 50, 5f);
                stats.curFreezeCharge = 0;
            }
        }
        if (stats.isIgnited)
        {
            fx.FlashElementHit(ElementType.Fire);
            stats.curBurnCharge += 1;
            if(stats.curBurnCharge >= stats.maxBurnCharge)
            {
                fx.FireBurning();
                stats.StartIgniteDoT(5f, 1f, 20f);
                stats.curBurnCharge = 0;
            }
        }
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        rb.linearVelocity = new Vector2(knockbackDir.x * -facingDir, knockbackDir.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }
    #region Velocity
    public void ZeroVelocity()
    {
        if (isKnocked)
            return;
        rb.linearVelocity = new Vector2(0, 0);
    }
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;
        rb.linearVelocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion
    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
        Gizmos.DrawWireSphere(kickCheck.position, kickCheckRadius);

    }
    #endregion
    #region Flip
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        onFlipped?.Invoke();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();

    }
    #endregion

    public virtual void Die()
    {

    }
}
