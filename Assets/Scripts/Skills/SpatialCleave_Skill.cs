using UnityEngine;
using System.Collections;

public class SpatialCleaveSkill : Skill
{
    [Header("视觉材质控制")]
    public Material screenSliceMaterial;
    public float sliceDuration = 1.2f;
    public float maxOffset = 0.06f;

    [Header("物理与伤害判定")]
    public float baseSliceAngle = 45f;   // 基础角度
    public float angleRandomRange = 180f; // 随机波动范围 (设置为180则全向随机)
    public float damageThickness = 2.5f;
    public int massiveDamage = 9999;
    public LayerMask enemyLayer;

    private bool isSlicing = false;
    private Camera mainCam;

    // 性能优化：提前缓存 Shader 属性 ID，避免每帧进行字符串寻址
    private static readonly int SliceAngleID = Shader.PropertyToID("_SliceAngle");
    private static readonly int SliceCenterID = Shader.PropertyToID("_SliceCenter");
    private static readonly int SliceOffsetID = Shader.PropertyToID("_SliceOffset");
    private static readonly int EdgeColorID = Shader.PropertyToID("_EdgeGlowColor");

    private void Start()
    {
        mainCam = Camera.main;
    }

    public override bool CanUseSkill()
    {
        return !isSlicing && base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        StartCoroutine(ExecuteSpatialCleave());
    }

    private IEnumerator ExecuteSpatialCleave()
    {
        isSlicing = true;

        // 1. 获取随机角度
        // 如果想完全随机，用 Random.Range(0, 360)
        float currentAngle = baseSliceAngle + Random.Range(-angleRandomRange, angleRandomRange);

        // 2. 坐标精确换算
        Vector3 mousePos = Input.mousePosition;

        // 修正盲区：确保 Shader 坐标精确对准像素
        Vector2 shaderCenter = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);

        // 物理坐标转换：Z轴必须为摄像机到平面的距离，否则会有视差偏移
        float camDist = Mathf.Abs(mainCam.transform.position.z);
        Vector2 worldCenter = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camDist));

        // 3. 应用 Shader 效果
        screenSliceMaterial.SetFloat(SliceAngleID, currentAngle);
        screenSliceMaterial.SetVector(SliceCenterID, shaderCenter);

        float angleRad = currentAngle * Mathf.Deg2Rad;
        // 法线方向：垂直于切割线的方向
        Vector2 offsetDir = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));

        // 初始撕裂
        screenSliceMaterial.SetVector(SliceOffsetID, offsetDir * maxOffset);
        screenSliceMaterial.SetColor(EdgeColorID, Color.cyan * 5f); // 增强亮度

        // 4. 物理判定
        // 增加判定框长度（50->100）以彻底覆盖所有屏幕纵横比，消除边缘盲区
        Vector2 boxSize = new Vector2(100f, damageThickness);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            worldCenter,
            boxSize,
            currentAngle,
            Vector2.zero,
            0f,
            enemyLayer
        );

        // 5. 处理伤害与反馈
        if (hits.Length > 0)
        {
            // 顿帧与震动
            AttackScene.Instance.HitPause(10);
            AttackScene.Instance.CameraShake(0.2f, 3f);

            foreach (RaycastHit2D hit in hits)
            {
                EnemyStats enemy = hit.collider.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(massiveDamage);
                    // 额外效果：顺着切口方向推开敌人
                    if (hit.collider.TryGetComponent(out Rigidbody2D rb))
                        rb.AddForce(offsetDir * 5f, ForceMode2D.Impulse);
                }
            }
        }

        // 6. 愈合动画 (平滑曲线)
        float timer = 0f;
        while (timer < sliceDuration)
        {
            timer += Time.deltaTime;
            // 使用平滑插值 (降速回归)
            float t = timer / sliceDuration;
            float curve = 1 - Mathf.Pow(1 - t, 3); // EaseOut 效果

            Vector2 currentOffset = Vector2.Lerp(offsetDir * maxOffset, Vector2.zero, curve);
            Color currentColor = Color.Lerp(Color.cyan * 5f, Color.black, curve);

            screenSliceMaterial.SetVector(SliceOffsetID, currentOffset);
            screenSliceMaterial.SetColor(EdgeColorID, currentColor);

            yield return null;
        }

        // 重置
        screenSliceMaterial.SetVector(SliceOffsetID, Vector2.zero);
        screenSliceMaterial.SetColor(EdgeColorID, Color.black);
        isSlicing = false;
    }
}