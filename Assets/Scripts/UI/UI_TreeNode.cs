using DG.Tweening;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;

    [Header("Unlock details")]
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill details")]
    public SkillDataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private Color skillLockedColor;

    // hover悬浮变量
    [Header("悬浮放大倍数")]
    public float hoverScale = 1.1f; // 放大10%，可改1.05/1.2等
    [Header("悬浮向上偏移（像素）")]
    public float hoverYOffset = 5f; // 向上悬浮5像素

    // 缓存初始状态（避免重复计算）
    private Vector3 originalScale; // 初始缩放
    private Vector2 originalPos;   // 初始位置

    private void OnValidate()
    {
        if (skillData == null)
            return;

        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree=GetComponentInParent<UI_SkillTree>();

        UpdateIconColor(skillLockedColor);

        if (skillIcon == null)
        {
            skillIcon = GetComponent<Image>();
        }

        // 记录初始缩放和位置（以RectTransform为准）
        RectTransform rectTrans = skillIcon.GetComponent<RectTransform>();
        originalScale = rectTrans.localScale;
        originalPos = rectTrans.anchoredPosition;
    }

    private void LockConflictNodes()
    {
        foreach(var node in conflictNodes)
        {
            node.isLocked = true;
        }
    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        skillTree.RemoveSkillPoints(skillData.cost);
        LockConflictNodes();

        // 果冻弹跳效果：参数(弹跳力度Vector3, 持续时间, 震动次数, 弹性)
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.5f, 5, 1f);

        // 配合颜色渐变变白
        skillIcon.DOColor(Color.white, 0.3f);
    }

    private bool CanBeUnlocked()
    {
        if(isUnlocked||isLocked)
        {
            return false;
        }

        if(!skillTree.EnoughSkillPoints(skillData.cost))
            return false;

        // 父节点必须先解锁
        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false) 
                return false;
        }

        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)
                return false;
        }

        return true;
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        skillIcon.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
        {
            Unlock();
        }
        else if (isLocked)
        {
            ui.skillToolTip.LockedSkillEffect1();
            rect.DOKill();

            // 震动效果：参数(持续时间, 震动强度像素, 震频, 随机性)
            // 注意：UI震动一定要用 DOShakeAnchorPos，不要用 DOShakePosition
            rect.DOShakeAnchorPos(0.3f, strength: new Vector2(10f, 0f), vibrato: 30, randomness: 90);
        }
        else if (!isUnlocked)
        {
            ui.skillToolTip.LockedSkillEffect2();
            rect.DOKill();

            rect.DOShakeAnchorPos(0.3f, strength: new Vector2(10f, 0f), vibrato: 30, randomness: 90);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);

        UpdateIconColor(Color.white * .9f);

        transform.DOKill(); // 杀掉上一个动画防抖
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);

        if (isUnlocked)
        {
            UpdateIconColor(Color.white);
        }
        else
            UpdateIconColor(skillLockedColor);

        transform.DOKill();
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutQuad);
    }
}
