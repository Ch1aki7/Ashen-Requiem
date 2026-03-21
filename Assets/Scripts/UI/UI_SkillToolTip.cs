using System.Collections;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using DG.Tweening;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;
    [SerializeField] private TextMeshProUGUI conflictSkills;

    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string notMetConditionHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] private Color exampleColor;
    [SerializeField] private string lockedReason1 = "你已经做出了选择\n该技能现已被锁定.";
    [SerializeField] private string lockedReason2 = "请先解锁上方的技能";

    protected override void Awake()
    {
        base.Awake();
        skillTree = GetComponentInParent<UI_SkillTree>();
    }
    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)
            return;

        skillName.text = node.skillData.displayName;
        skillDescription.text = node.skillData.description;

        string skillLockedText = $"<color={importantInfoHex}>{lockedReason1}</color>";
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost, node.neededNodes);

        skillRequirements.text = requirements;
        conflictSkills.text = GetConflicts(node.conflictNodes);

    }

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("需要:");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notMetConditionHex;
        sb.AppendLine($"<color={costColor}> - {skillCost}技能点</color>");

        foreach (var node in neededNodes)
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            sb.AppendLine($"<color={nodeColor}> - {node.skillData.displayName}</color>");
        }

        return sb.ToString();
    }

    private string GetConflicts(UI_TreeNode[] conflictNodes)
    {
        if (conflictNodes == null || conflictNodes.Length <= 0)
            return ""; // 没有冲突时返回空

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<color={importantInfoHex}>只能选择其一：</color>");

        foreach (var node in conflictNodes)
        {
            sb.AppendLine($"<color={importantInfoHex}> - {node.skillData.displayName}</color>");
        }

        return sb.ToString();
    }


    public void LockedSkillEffect1()
    {
        // 将 Hex 转换为 Color
        UnityEngine.ColorUtility.TryParseHtmlString(notMetConditionHex, out Color blinkColor);
        UnityEngine.ColorUtility.TryParseHtmlString(importantInfoHex, out Color normalColor);

        // 确保文字纯净并设为基础色
        skillRequirements.text = lockedReason1;
        skillRequirements.color = normalColor;

        // 杀掉之前的动画（防止狂点叠加）
        skillRequirements.DOKill();

        // 【这1行代码代替了你整个协程】：
        // 0.15秒变到红色，来回循环6次（3次红3次原色），循环方式为 Yoyo (像溜溜球一样来回)
        skillRequirements.DOColor(blinkColor, 0.15f).SetLoops(6, LoopType.Yoyo);
    }

    public void LockedSkillEffect2()
    {
        // 将 Hex 转换为 Color
        UnityEngine.ColorUtility.TryParseHtmlString(notMetConditionHex, out Color blinkColor);
        UnityEngine.ColorUtility.TryParseHtmlString(importantInfoHex, out Color normalColor);

        // 确保文字纯净并设为基础色
        skillRequirements.text = lockedReason2;
        skillRequirements.color = normalColor;

        // 杀掉之前的动画（防止狂点叠加）
        skillRequirements.DOKill();

        // 【这1行代码代替了你整个协程】：
        // 0.15秒变到红色，来回循环6次（3次红3次原色），循环方式为 Yoyo (像溜溜球一样来回)
        skillRequirements.DOColor(blinkColor, 0.15f).SetLoops(6, LoopType.Yoyo);
    }
}
