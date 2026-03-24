using UnityEngine;

public class SlashEffect_HitBox : MonoBehaviour
{
    private CharacterStats playerStats;

    public void SetupSlash(CharacterStats _playerStats)
    {
        playerStats = _playerStats;
    }

    // 关键：当刀光碰到任何有 Collider 的物体时触发
    // 前提条件：
    // 1. 刀光预制体上必须挂载一个 Collider2D 组件（比如 BoxCollider2D 或 PolygonCollider2D）
    // 2. 该 Collider2D 必须勾选 "Is Trigger"！
    // 3. 刀光预制体上最好挂载一个 Rigidbody2D，把 Body Type 设为 Kinematic（运动学），否则无法触发检测。
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            // 防止报空指针（如果生成的瞬间还没传过来 stats）
            if (playerStats == null) return;

            playerStats.DoDamage(enemyStats);
            playerStats.DoMagicalDamage(enemyStats);

            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage();

                AttackScene.Instance.HitPause(PlayerManager.instance.player.hitPause / 2);
                AttackScene.Instance.CameraShake(PlayerManager.instance.player.shakeTime, PlayerManager.instance.player.hitMagnitude);
            }

            // 穿透效果开关
            // Destroy(gameObject); 
        }
    }
}