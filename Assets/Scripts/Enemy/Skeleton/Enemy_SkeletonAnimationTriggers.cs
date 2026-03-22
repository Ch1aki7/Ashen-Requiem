using UnityEngine;

public class Enemy_SkeletonAnimationTrigger : MonoBehaviour
{
    private Enemy_Skeleton skeleton => GetComponentInParent<Enemy_Skeleton>();
    private void AnimationTrigger()
    {
        skeleton.AnimationTrigger();
    }

    public void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(skeleton.attackCheck.position, skeleton.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {

                if (skeleton.transform.position.x < hit.GetComponent<Player>().transform.position.x && hit.GetComponent<Player>().facingRight)
                    hit.GetComponent<Player>().Flip();
                else if (skeleton.transform.position.x > hit.GetComponent<Player>().transform.position.x && !hit.GetComponent<Player>().facingRight)
                    hit.GetComponent<Player>().Flip();

                if (hit.GetComponent<Player>().GOD == false)
                {
                PlayerStats _target = hit.GetComponent<PlayerStats>();

                skeleton.stats.DoDamage(_target);
                skeleton.stats.DoMagicalDamage(_target);
                hit.GetComponent<Player>().Damage();
                }
            }
        }
    }

    public void OpenCounterWindow() => skeleton.OpenCounterAttackWindow();
    public void CloseCounterWindow() => skeleton.CloseCounterAttackWindow();
    public void OpenDangerWindow() => skeleton.OpenDangerWindow();
    public void CloseDangerWindow() => skeleton.CloseDangerWindow();
}
