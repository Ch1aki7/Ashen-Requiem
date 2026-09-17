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

        if (!ResolvePlayer() || Camera.main == null)
        {
            Debug.LogWarning("无法生成刀光：玩家或主摄像机不存在。", this);
            return;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0;

        CreateSlashTowards(player.transform.position, mouseWorldPosition);
    }

    /// <summary>
    /// 让玩家本体以外的攻击者复用当前刀光 Prefab 与 HitBox 伤害流程。
    /// 黑洞影子会在攻击动作的判定帧调用此方法。
    /// </summary>
    public GameObject CreateSlashTowards(Vector3 origin, Vector3 targetPosition)
    {
        if (!ResolvePlayer())
            return null;

        if (player.slashEffectPrefab == null)
        {
            Debug.LogWarning("玩家没有配置刀光 Prefab。", player);
            return null;
        }

        Vector3 direction = targetPosition - origin;
        direction.z = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector3.right * player.facingDir;
        else
            direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += baseRotationOffset;

        Quaternion slashRotation = Quaternion.Euler(0, 0, angle);
        Vector3 slashPosition = origin + direction * slashOffsetDistance;
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

        return newSlash;
    }

    private bool ResolvePlayer()
    {
        if (player == null && PlayerManager.instance != null)
            player = PlayerManager.instance.player;

        return player != null;
    }

    protected override void Update()
    {
        base.Update();
    }
}
