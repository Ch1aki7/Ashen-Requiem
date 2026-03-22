using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (player.transform.position.x < hit.GetComponent<Enemy>().transform.position.x && hit.GetComponent<Enemy>().facingRight)
                    hit.GetComponent<Enemy>().Flip();
                else if(player.transform.position.x > hit.GetComponent<Enemy>().transform.position.x && !hit.GetComponent<Enemy>().facingRight)
                    hit.GetComponent<Enemy>().Flip();


                AttackScene.Instance.HitPause(player.hitPause);
                AttackScene.Instance.CameraShake(player.shakeTime, player.hitMagnitude);

                EnemyStats _target=hit.GetComponent<EnemyStats>();

                player.stats.DoDamage(_target);
                player.stats.DoMagicalDamage(_target);
                hit.GetComponent<Enemy>().Damage();

            }
        }
    }

    private void KickTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.kickCheck.position, player.kickCheckRadius);


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (player.transform.position.x < hit.GetComponent<Enemy>().transform.position.x && hit.GetComponent<Enemy>().facingRight)
                    hit.GetComponent<Enemy>().Flip();
                else if (player.transform.position.x > hit.GetComponent<Enemy>().transform.position.x && !hit.GetComponent<Enemy>().facingRight)
                    hit.GetComponent<Enemy>().Flip();

                hit.GetComponent<Enemy>().Damage();

                if (hit.GetComponent<Enemy>().canbeStunned)
                {
                    player.dashDir = Input.GetAxisRaw("Horizontal");
                    if (player.dashDir == 0)
                        player.dashDir = player.facingDir;
                    player.stateMachine.ChangeState(player.dragonState);
                }
                AttackScene.Instance.HitPause(player.hitPause);
                AttackScene.Instance.CameraShake(player.shakeTime, player.hitMagnitude);

                EnemyStats _target = hit.GetComponent<EnemyStats>();

                player.stats.DoDamage(_target);
                player.stats.DoMagicalDamage(_target);


            }


        }
    }

    private void GodTrigger()
    {
        player.GOD = true;
    }

    private void DegodTrigger()
    {
        player.GOD = false;
    }

    private void TheWorld()
    {
        AttackScene.Instance.HitPause(10);
    }

}
