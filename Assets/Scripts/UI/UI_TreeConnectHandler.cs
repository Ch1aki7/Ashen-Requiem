using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]

public class UI_TreeConnectionDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f,350f)] public float length;

}
public class UI_TreeConnectHandler : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private UI_TreeConnectionDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private void OnValidate()
    {
        if(rect == null)
            rect=GetComponent<RectTransform>();

        // 【新增防错】：防止数组为空时报错
        if (connectionDetails == null || connections == null)
            return;

        if (connectionDetails.Length != connections.Length)
        {
            // 避免频繁弹警告，最好只在长度不一致时提示一次
            Debug.LogWarning($"[{gameObject.name}] 的连接物体和连接线数组长度不相等！");
            return;
        }

        UpdateConnection();
    }
    private  void UpdateConnection()
    {
        for(int i = 0;i<connectionDetails.Length;i++)
        {
            // 【防错 1】：如果数组里有空槽位（即面板上显示为 None），直接跳过，防止报错
            if (connections[i] == null) continue;
            if (connectionDetails[i] == null || connectionDetails[i].childNode == null) continue;

            // 只有当连线和子节点都拖入面板后，才执行计算
            Vector2 targetPosition = connections[i].GetConnectionPoint(rect);

            connections[i].DirectConnection(connectionDetails[i].direction, connectionDetails[i].length);
            connectionDetails[i].childNode.SetPosition(targetPosition);
        }
    }

    public void SetPosition(Vector2 position)
    {
        // 【防错 2 解决核心Bug】：当父节点强制呼叫子节点时，必须确保子节点的 rect 已经被初始化！
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }

        rect.anchoredPosition = position;
    }
}
