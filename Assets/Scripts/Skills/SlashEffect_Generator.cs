using UnityEngine;

public class SlashEffect_Generator : Skill
{
    private Player player;

    [Header("刀光设置")]
    [Tooltip("刀光距离玩家中心的偏移距离 (半径)")]
    [SerializeField] private float slashOffsetDistance = 0.72f;

    [Tooltip("刀光图片的基础旋转校正度数 (0, 90, 180, -90)")]
    [SerializeField] private float baseRotationOffset = 0f;

    private void Start()
    {
        player = PlayerManager.instance.player;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        // 获取鼠标在屏幕上的坐标，转换为世界坐标
        Vector3 mouseScreenPosition = Input.mousePosition;
        // 把Z轴设为摄像机到玩家的距离（由于是正交相机，或者2D游戏，其实随便给个大于0的值就行）
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        mouseWorldPosition.z = 0;

        // 确定发射起点 (建议用玩家的身体中心，而不是之前的 attackCheck)
        Vector3 playerCenterPos = player.transform.position + new Vector3(0, 0, 0); // 根据模型高度微调

        // 计算从玩家指向鼠标的方向向量
        Vector3 direction = (mouseWorldPosition - playerCenterPos).normalized;

        // 计算 2D 旋转角度！(极其核心的公式)
        // Mathf.Atan2(y, x) 算出弧度，再乘以 Rad2Deg 转成角度。
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 如果你的图片画的刀光，初始状态下(0度)是向右劈的，就不需要管。
        // 如果它初始是向上劈的，你需要减去90度 (baseRotationOffset = -90f)。
        angle += baseRotationOffset;

        // 将计算出的角度转换成四元数 (围绕Z轴旋转)
        Quaternion slashRotation = Quaternion.Euler(0, 0, angle);

        // 计算刀光的最终生成位置 (顺着鼠标方向，往外推移一段距离)
        Vector3 slashPosition = playerCenterPos + (direction * slashOffsetDistance);

        // 生成刀光！
        // 极其关键：因为是万向的，千万【不要】把它挂在 player.transform 下面当子物体！
        GameObject newSlash = Instantiate(player.slashEffectPrefab, slashPosition, slashRotation);

        SlashEffect_HitBox hitBox = newSlash.GetComponent<SlashEffect_HitBox>();

        if (hitBox != null)
        {
            hitBox.SetupSlash(player.stats);
        }
        else
        {
            Debug.LogWarning("你的刀光预制体上没有挂载 SlashEffect_HitBox 脚本！伤害将无法生效。");
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}