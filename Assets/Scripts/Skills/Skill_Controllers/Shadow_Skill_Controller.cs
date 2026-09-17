using System.Collections;
using UnityEngine;

public class Shadow_Skill_Controller : MonoBehaviour
{

    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLosingSpeed;

    [SerializeField] private float shadowDuration;
    private float shadowTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius;
    [SerializeField] private float slashFireDelay = 0.2f;
    private Transform closestEnemy;
    private Transform assignedTarget;
    private bool useSlashProjectile;
    private bool slashFired;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    public void Update()
    {
        shadowTimer -= Time.deltaTime;

        if (shadowTimer <= 0)
        {
            sr.color = new Color(1f, 1f, 1f, sr.color.a - (Time.deltaTime * colorLosingSpeed));

            if (sr.color.a < 0f)
                Destroy(gameObject);
        }
    }
    public void SetupShadow(Transform _newTransform, bool _canAttack,Vector3 _offset)
    {
        useSlashProjectile = false;
        if (_canAttack)
        {
            anim.SetInteger("AttackMode", UnityEngine.Random.Range(1, 3));
        }

        transform.position = _newTransform.position + _offset;
        shadowTimer = shadowDuration;

        FacingClosestTarget();
    }

    public void SetupShadowNoIai(Transform _newTransform, bool _canAttack, Vector3 _offset)
    {
        assignedTarget = _newTransform;
        useSlashProjectile = true;
        slashFired = false;

        if (_canAttack)
        {
            anim.SetInteger("AttackMode", 1);
            StartCoroutine(FireSlashAtAttackFrame());
        }

        transform.position = _newTransform.position + _offset;
        shadowTimer = shadowDuration;

        FacingClosestTarget();
    }

    private void AnimationTrigger()
    {
        shadowTimer = -.1f;
    }
    private void AttackTrigger()
    {
        if (useSlashProjectile)
        {
            FireSlashAtAssignedTarget();
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage();
        }
    }

    private void FireSlashAtAssignedTarget()
    {
        if (slashFired)
            return;

        Transform target = assignedTarget != null ? assignedTarget : closestEnemy;
        SlashEffect_Generator slashGenerator = SkillManager.Instance != null
            ? SkillManager.Instance.slashEffect
            : null;

        if (target == null || slashGenerator == null)
        {
            Debug.LogWarning("黑洞影子缺少攻击目标或刀光生成器。", this);
            return;
        }

        slashFired = slashGenerator.CreateSlashTowards(transform.position, target.position) != null;
    }

    private IEnumerator FireSlashAtAttackFrame()
    {
        yield return new WaitForSeconds(slashFireDelay);
        FireSlashAtAssignedTarget();
    }

    private void IaiTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    anim.SetBool("Iai_Slash", true);
                }
            }
        }
    }

    private void FacingClosestTarget()
    {
        if (assignedTarget != null)
        {
            closestEnemy = assignedTarget;
            FaceTarget(closestEnemy);
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;

        foreach (var hit in colliders)
        {
            if ((hit.GetComponent<Enemy>() != null))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        FaceTarget(closestEnemy);
    }

    private void FaceTarget(Transform target)
    {
        if (target != null && transform.position.x > target.position.x)
            transform.Rotate(0, 180, 0);

    }


}
