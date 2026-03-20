using UnityEngine;

public class UI_TreeConnection : MonoBehaviour
{
    [SerializeField] private RectTransform rotationPoint;
    [SerializeField] private RectTransform connectionLength;
    [SerializeField] private RectTransform connectionPosition;

    public void DirectConnection(NodeDirectionType direction, float length)
    {
        bool shouldBeActive = direction != NodeDirectionType.None;
        float finalLength = shouldBeActive ? length : 0;
        float angle = GetDirectionAngle(direction);

        rotationPoint.localRotation= Quaternion.Euler(0,0,angle);
        connectionLength.sizeDelta =new Vector2 (finalLength,connectionLength.sizeDelta.y);
    }

    public Vector2 GetConnectionPoint(RectTransform rect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                rect.parent as RectTransform,
                connectionPosition.position,
                null,
                out var localPosition
            );
        return localPosition;
    }
    private float GetDirectionAngle(NodeDirectionType type)
    {
        switch(type)
        {
            // --- 基础四向 ---
            case NodeDirectionType.Right: return 0f;
            case NodeDirectionType.Up: return 90f;
            case NodeDirectionType.Left: return 180f;
            case NodeDirectionType.Down: return 270f; // 或者写 -90f

            // --- 对角四向 (每次偏移45度) ---
            case NodeDirectionType.UpRight: return 45f;
            case NodeDirectionType.UpLeft: return 135f;
            case NodeDirectionType.DownLeft: return 225f; // 或者写 -135f
            case NodeDirectionType.DownRight: return 315f; // 或者写 -45f

            // --- 默认情况 ---
            case NodeDirectionType.None:
            default: return 0f;   // 兜底返回0度
        }
    }
}

public enum NodeDirectionType
{
    None,
    UpLeft,
    Up,
    UpRight,
    Left,
    Right,
    DownLeft,
    Down,
    DownRight
}