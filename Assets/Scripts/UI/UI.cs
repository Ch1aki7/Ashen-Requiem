using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("UI 组件引用")]
    public UI_SkillToolTip skillToolTip;
    public UI_SkillTree skillTreePanel; // 新增：技能树面板的引用

    [Header("快捷键设置")]
    public KeyCode skillTreeKey = KeyCode.P;

    private void Awake()
    {
        // 重点避坑：加上 (true) 参数！
        // 因为 UI 面板通常一开始是隐藏的，如果不加 true，它找不到被隐藏的物体，会报错 Null！
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>(true);
        skillTreePanel = GetComponentInChildren<UI_SkillTree>(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(skillTreeKey))
        {
            ToggleSkillTree();
        }

        // 如果以后有背包系统，可以继续加：
        // if (Input.GetKeyDown(KeyCode.B)) ToggleInventory();
    }

    private void ToggleSkillTree()
    {
        if (skillTreePanel != null)
        {
            skillTreePanel.ToggleMenu(); // 呼叫技能树面板自己的开关动画
        }
    }
}