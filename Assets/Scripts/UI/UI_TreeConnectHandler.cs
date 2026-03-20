using System;
using System.Runtime.CompilerServices;
using UnityEditor.MemoryProfiler;
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
            Debug.Log("连接物体和连接应该相等");
            return;
        }

        UpdateConnection();
    }
    private  void UpdateConnection()
    {
        for(int i = 0;i<connectionDetails.Length;i++)
        {
            Vector2 targetPosition = connections[i].GetConnectionPoint(rect);

            connections[i].DirectConnection(connectionDetails[i].direction, connectionDetails[i].length);
            connectionDetails[i].childNode.SetPosition(targetPosition);
        }
    }

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
