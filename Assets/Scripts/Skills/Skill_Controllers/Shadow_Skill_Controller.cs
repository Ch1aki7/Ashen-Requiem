using System;
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
    private Transform closestEnemy;

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
        if (_canAttack)
        {
            anim.SetInteger("AttackMode", 1);
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage();
        }
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

        if(closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
                transform.Rotate(0, 180, 0);
        }

    }


}
