using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UI_SkillTree : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private UI uiManager;

    public bool isOpen = false;

    public int skillPoint;
    public bool EnoughSkillPoints(int cost) => skillPoint >= cost;
    public void RemoveSkillPoints(int cost) => skillPoint -= cost;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        uiManager = GetComponentInParent<UI>();

        // 游戏一开始，强制设为透明且不可点击
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen; // 状态反转

        // 杀掉没播完的动画，防止玩家狂按抽搐
        canvasGroup.DOKill();

        if (isOpen)
        {
            // --- 打开菜单 ---
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // 0.25秒淡入。SetUpdate(true) 保证游戏暂停时 UI 动画依然能播
            canvasGroup.DOFade(1f, 0.25f).SetUpdate(true);

        }
        else
        {
            // --- 关闭菜单 ---
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(0f, 0.2f).SetUpdate(true);

            // 极其关键的防 Bug 细节：关闭菜单时，强制隐藏残留的提示框！
            if (uiManager != null && uiManager.skillToolTip != null)
            {
                // 让总管家把幽灵提示框飞到 9999 坐标去
                uiManager.skillToolTip.ShowToolTip(false, null);
            }
        }
    }
}