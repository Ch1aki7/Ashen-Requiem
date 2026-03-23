using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using System.Collections;

public class Oath_Skill : Skill
{
    private Player player;

    private SpriteRenderer playerSpriteRenderer;
    private Material playerMaterial;

    [Header("元素附魔刀光颜色 (支持HDR发光)")]
    [ColorUsage(true, true)]
    [SerializeField] private Color fireSwordColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color iceSwordColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color lightningSwordColor;

    [SerializeField] private string shaderColorPropertyName = "_SwordColor";

    private Color defaultSwordColor;

    // 记录当前的恢复协程，防止连续释放技能时颜色错乱
    private Coroutine resetColorCoroutine;

    private void Start()
    {
        player = PlayerManager.instance.player;

        playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();

        if (playerSpriteRenderer != null)
        {
            playerMaterial = playerSpriteRenderer.sharedMaterial;

            // 这样以后即使在 Shader Graph 里换了默认颜色，这里也不用去改数字了！
            defaultSwordColor = playerMaterial.GetColor(shaderColorPropertyName);
        }
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        System.Array elements = System.Enum.GetValues(typeof(ElementType));
        int randomIndex = Random.Range(1, elements.Length);
        ElementType randomElement = (ElementType)elements.GetValue(randomIndex);

        Debug.Log($"誓约技能发动！随机抽到的属性是: {randomElement}");

        float enchantDuration = 3f;
        player.stats.ApplyWeaponEnchantment(randomElement, 10, enchantDuration);

        ChangeSwordColor(randomElement);

        // 开启计时器，到期后恢复默认颜色
        // 如果之前有还没跑完的恢复倒计时，立刻停掉它（防止新附魔被旧计时器强行恢复）
        if (resetColorCoroutine != null)
        {
            StopCoroutine(resetColorCoroutine);
        }
        resetColorCoroutine = StartCoroutine(ResetColorAfterDelay(enchantDuration));
    }

    // --- 修改 Shader 颜色的核心方法 ---
    private void ChangeSwordColor(ElementType element)
    {
        if (playerMaterial == null) return;

        Color targetColor = Color.white;

        switch (element)
        {
            case ElementType.Fire:
                targetColor = fireSwordColor;
                break;
            case ElementType.Ice:
                targetColor = iceSwordColor;
                break;
            case ElementType.Lightning:
                targetColor = lightningSwordColor;
                break;
        }

        playerSpriteRenderer.material.SetColor(shaderColorPropertyName, targetColor);
        playerMaterial.SetColor(shaderColorPropertyName, targetColor);
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerSpriteRenderer != null && playerMaterial != null)
        {
            playerSpriteRenderer.material.SetColor(shaderColorPropertyName, defaultSwordColor);
            playerMaterial.SetColor(shaderColorPropertyName, defaultSwordColor);
        }

        resetColorCoroutine = null;
    }

    private void OnApplicationQuit()
    {
        if (playerMaterial != null)
        {
            // 退出游戏时恢复默认颜色
            playerMaterial.SetColor(shaderColorPropertyName, new Color(0.909f, 1.025f, 1.059f, 0f));
        }
    }
}