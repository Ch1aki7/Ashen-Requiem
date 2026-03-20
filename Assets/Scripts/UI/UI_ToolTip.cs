using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Vector2 baseOffset = new Vector2(300, 100);

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (!show)
            rect.position = new Vector2(9999, 9999);
        else
            UpdatePosition(targetRect);
    }

    public void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;
        float screenCenterY = Screen.height / 2f;
        Vector2 targetPosition = targetRect.position;

        // 【核心代码】：根据屏幕宽高的比例动态放大偏移量
        // 假设在 4K 屏幕 (3840宽) 下：3840 / 1920 = 2。X偏移量自动 * 2
        float scaleX = Screen.width / 1920f;
        float scaleY = Screen.height / 1080f;

        Vector2 dynamicOffset = new Vector2(baseOffset.x * scaleX, baseOffset.y * scaleY);

        targetPosition.x = targetPosition.x > screenCenterX ? targetPosition.x - dynamicOffset.x : targetPosition.x + dynamicOffset.x;
        targetPosition.y = targetPosition.y > screenCenterY ? targetPosition.y - dynamicOffset.y : targetPosition.y + dynamicOffset.y;

        rect.position = targetPosition;
    }
}