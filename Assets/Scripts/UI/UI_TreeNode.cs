using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;

    [SerializeField] private SkillDataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Color skillLockedColor;
    public bool isUnlocked;
    public bool isLocked;

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
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();

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

    private void Unlock()
    {
        isUnlocked = true;

        UpdateIconColor(Color.white);
    }

    private bool CanBeUnlocked()
    {
        if(isUnlocked||isLocked)
        {
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
        else
            Debug.Log("解锁不能");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, skillData);

        UpdateIconColor(Color.white * .9f);

        // 变大+悬浮
        RectTransform rectTrans = skillIcon.GetComponent<RectTransform>();
        rectTrans.localScale = originalScale * hoverScale; // 放大
        rectTrans.anchoredPosition = originalPos + new Vector2(0, hoverYOffset); // 向上偏移
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

        // 恢复缩放和位置
        RectTransform rectTrans = skillIcon.GetComponent<RectTransform>();
        rectTrans.localScale = originalScale;
        rectTrans.anchoredPosition = originalPos;
    }
}
