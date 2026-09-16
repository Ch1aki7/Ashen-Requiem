using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialGuide : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Enemy_Skeleton enemy;
    [SerializeField] private TMP_FontAsset chineseFont;

    private readonly string[] titles =
    {
        "移动", "跳跃", "攀爬与墙跳", "普通攻击", "冲刺", "技能树", "实战技能", "完成"
    };

    private readonly string[] instructions =
    {
        "按 A / D 或方向键移动一段距离。",
        "按 Space 跳跃，并安全落地。",
        "前往右侧高墙，跳起后按住朝墙方向进入贴墙滑行；贴墙时按 Space 蹬墙跳。",
        "用鼠标左键攻击；刀光会朝鼠标方向飞出。",
        "按左 Shift 冲刺。未输入方向时，角色会朝当前面向冲刺。",
        "按 P 打开技能树，移动鼠标查看技能说明，再按 P 关闭。",
        "右键进入居合。等待敌人攻击的反击判定出现，再按一次右键完成反击。",
        "全部教学完成！按 Enter 重玩，或按 P 继续查看技能树。"
    };

    private enum CombatPhase
    {
        IaiReady, IaiWaiting, Kick, EnchantPrompt, EnchantActive,
        BlackholeReady, BlackholeActive, DimensionalSlash
    }

    private readonly ElementType[] tutorialElements =
    {
        ElementType.Fire, ElementType.Ice, ElementType.Lightning
    };

    private int step;
    private float startingX;
    private bool jumped;
    private bool wallSlideSeen;
    private bool openedTree;
    private bool combatPrepared;
    private CombatPhase combatPhase;
    private int elementIndex;
    private int observedEnchantUses;
    private int burstCountBeforeEnchant;
    private Oath_Skill oath;

    private TMP_Text titleText;
    private TMP_Text instructionText;
    private TMP_Text progressText;
    private Image progressFill;

    private void Awake()
    {
        if (player == null || enemy == null || chineseFont == null)
        {
            Debug.LogError("TutorialGuide references are missing.", this);
            enabled = false;
            return;
        }

        startingX = player.transform.position.x;
        player.GOD = true;
        CreateOverlay();
        ShowStep();
    }

    private void Update()
    {
        if (step < 7)
            player.GOD = true;

        if (Input.GetKeyDown(KeyCode.Backspace) || (step == 7 && Input.GetKeyDown(KeyCode.Return)))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        switch (step)
        {
            case 0:
                if (Mathf.Abs(player.transform.position.x - startingX) >= 1.5f) Advance();
                break;
            case 1:
                if (player.stateMachine.currentState == player.jumpState) jumped = true;
                if (jumped && player.IsGroundDetected()) Advance();
                break;
            case 2:
                UpdateWallTutorial();
                break;
            case 3:
                if (player.stateMachine.currentState == player.primaryAttackState) Advance();
                break;
            case 4:
                if (player.stateMachine.currentState == player.dashState) Advance();
                break;
            case 5:
                UpdateSkillTreeTutorial();
                break;
            case 6:
                UpdateCombatTutorial();
                break;
        }
    }

    private void UpdateWallTutorial()
    {
        if (player.stateMachine.currentState == player.wallSlideState && !wallSlideSeen)
        {
            wallSlideSeen = true;
            instructionText.text = "已进入贴墙滑行。现在按 Space 蹬离墙面，完成墙跳。";
        }

        if (wallSlideSeen && player.stateMachine.currentState == player.wallJumpState)
            Advance();
    }

    private void UpdateSkillTreeTutorial()
    {
        UI ui = FindFirstObjectByType<UI>();
        bool treeOpen = ui != null && ui.skillTreePanel != null && ui.skillTreePanel.isOpen;
        if (treeOpen) openedTree = true;
        if (openedTree && !treeOpen) Advance();
    }

    private void UpdateCombatTutorial()
    {
        if (!combatPrepared)
        {
            if (SkillManager.Instance == null || SkillManager.Instance.oath == null || enemy.stats == null)
                return;

            oath = SkillManager.Instance.oath;
            oath.ConfigureTutorialSequence(tutorialElements);
            observedEnchantUses = oath.EnchantmentUseCount;
            enemy.stats.currentHP = Mathf.Max(enemy.stats.currentHP, 5000f);
            combatPrepared = true;
            combatPhase = CombatPhase.IaiReady;
            ShowCombatInstruction();
        }

        switch (combatPhase)
        {
            case CombatPhase.IaiReady:
                if (player.stateMachine.currentState == player.iaiState)
                {
                    combatPhase = CombatPhase.IaiWaiting;
                    ShowCombatInstruction();
                }
                break;
            case CombatPhase.IaiWaiting:
                if (player.anim.GetBool("Iai_Slash") ||
                    enemy.stateMachine.currentState == enemy.stunnedState)
                {
                    combatPhase = CombatPhase.Kick;
                    ShowCombatInstruction();
                }
                else if (player.stateMachine.currentState != player.iaiState)
                {
                    combatPhase = CombatPhase.IaiReady;
                    ShowCombatInstruction();
                }
                break;
            case CombatPhase.Kick:
                if (player.stateMachine.currentState == player.dragonState)
                {
                    combatPhase = CombatPhase.EnchantPrompt;
                    ShowCombatInstruction();
                }
                break;
            case CombatPhase.EnchantPrompt:
                if (oath.EnchantmentUseCount > observedEnchantUses)
                {
                    observedEnchantUses = oath.EnchantmentUseCount;
                    burstCountBeforeEnchant = enemy.stats.ElementalBurstCount;
                    combatPhase = CombatPhase.EnchantActive;
                    ShowCombatInstruction();
                }
                break;
            case CombatPhase.EnchantActive:
                if (enemy.stats.ElementalBurstCount > burstCountBeforeEnchant &&
                    enemy.stats.LastElementalBurst == tutorialElements[elementIndex])
                {
                    elementIndex++;
                    if (elementIndex < tutorialElements.Length)
                    {
                        oath.ReadyNextTutorialEnchant();
                        combatPhase = CombatPhase.EnchantPrompt;
                    }
                    else
                    {
                        combatPhase = CombatPhase.BlackholeReady;
                    }
                    ShowCombatInstruction();
                }
                break;
            case CombatPhase.BlackholeReady:
                if (player.stateMachine.currentState == player.blackholeState)
                {
                    combatPhase = CombatPhase.BlackholeActive;
                    ShowCombatInstruction();
                }
                break;
            case CombatPhase.BlackholeActive:
                if (player.stateMachine.currentState != player.blackholeState)
                {
                    combatPhase = CombatPhase.DimensionalSlash;
                    ShowCombatInstruction();
                }
                break;
            case CombatPhase.DimensionalSlash:
                if (enemy.stats.currentHP <= 0f) Advance();
                break;
        }
    }

    private void ShowCombatInstruction()
    {
        switch (combatPhase)
        {
            case CombatPhase.IaiReady:
                SetCombatText("居合反击", "按右键进入居合架势。进入架势后不要立刻连按：等待敌人攻击前出现红色反击提示，再按一次右键。成功会打断并眩晕敌人。");
                break;
            case CombatPhase.IaiWaiting:
                SetCombatText("居合反击", "保持居合架势，观察敌人的攻击提示。反击判定出现时再按右键；过早按下不会命中反击窗口。");
                break;
            case CombatPhase.Kick:
                SetCombatText("踢击与追击", "等待敌人再次出现反击判定，在窗口内按 K 踢击。若踢中可眩晕目标，会自动衔接高速 Dragon 冲刺并穿向当前方向。");
                break;
            case CombatPhase.EnchantPrompt:
                string nextName = ElementName(tutorialElements[elementIndex]);
                SetCombatText("黄金树立誓 · " + nextName,
                    $"按 O 使用黄金树立誓。本教学按固定顺序附魔，本次将获得{nextName}附魔。附魔后用左键连续攻击敌人。");
                break;
            case CombatPhase.EnchantActive:
                SetCombatText("黄金树立誓 · " + ElementName(tutorialElements[elementIndex]),
                    ElementDescription(tutorialElements[elementIndex]));
                break;
            case CombatPhase.BlackholeReady:
                SetCombatText("黑洞", "按 R 升空并释放黑洞。黑洞会冻结范围内的敌人，并在目标上方生成按键提示。");
                break;
            case CombatPhase.BlackholeActive:
                SetCombatText("黑洞", "观察敌人上方的按键并按下它，将目标加入影袭列表；再按 R 开始连续影子攻击。黑洞出现约 1 秒后，可按 Space 主动收束并退出。");
                break;
            case CombatPhase.DimensionalSlash:
                SetCombatText("次元斩", "将鼠标指向敌人并按 V。次元斩会沿鼠标位置生成贯穿屏幕的随机斩线，造成极高伤害；用它结束本次实战。");
                break;
        }
    }

    private static string ElementName(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return "火焰";
            case ElementType.Ice: return "冰霜";
            case ElementType.Lightning: return "雷电";
            default: return "元素";
        }
    }

    private static string ElementDescription(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                return "火焰附魔：每次命中积累燃烧层数。趁剑身仍在发光，用左键连续命中 3 次；叠满后触发燃烧，在 5 秒内每秒造成 20 点伤害。";
            case ElementType.Ice:
                return "冰霜附魔：用左键连续命中 3 次积满冻结层数。叠满后触发冰爆，额外造成 100 点伤害；当前实现还会使目标的火焰抗性提高 50，持续 5 秒。";
            case ElementType.Lightning:
                return "雷电附魔：用左键连续命中 3 次积满感电层数。叠满后召唤落雷并额外造成 200 点伤害，是三种附魔中爆发最高的一种。";
            default:
                return string.Empty;
        }
    }

    private void SetCombatText(string subtitle, string body)
    {
        titleText.text = "07  实战 · " + subtitle;
        instructionText.text = body;
        progressText.text = "引导  7 / 7     ·     Backspace 重来";
        progressFill.fillAmount = 1f;
    }

    private void Advance()
    {
        step++;
        ShowStep();
    }

    private void ShowStep()
    {
        titleText.text = $"{step + 1:00}  {titles[step]}";
        instructionText.text = instructions[step];
        progressText.text = step < 7 ? $"引导  {step + 1} / 7     ·     Backspace 重来" : "引导完成     ·     Enter 重玩";
        progressFill.fillAmount = Mathf.Clamp01((step + 1f) / 7f);
    }

    private void CreateOverlay()
    {
        GameObject canvasObject = new GameObject("Tutorial UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform panel = CreateRect("Guide card", canvasObject.transform);
        panel.anchorMin = new Vector2(0.5f, 1f);
        panel.anchorMax = new Vector2(0.5f, 1f);
        panel.pivot = new Vector2(0.5f, 1f);
        panel.anchoredPosition = new Vector2(0f, -32f);
        panel.sizeDelta = new Vector2(1080f, 255f);
        Image background = panel.gameObject.AddComponent<Image>();
        background.color = new Color(0.055f, 0.065f, 0.085f, 0.94f);
        background.raycastTarget = false;

        titleText = CreateText("Title", panel, new Vector2(0f, -12f), new Vector2(1000f, 48f), 37f,
            new Color(1f, 0.83f, 0.48f));
        instructionText = CreateText("Instruction", panel, new Vector2(0f, -61f), new Vector2(1000f, 125f), 27f,
            Color.white);
        progressText = CreateText("Progress", panel, new Vector2(0f, -194f), new Vector2(1000f, 34f), 22f,
            new Color(0.72f, 0.8f, 0.84f));

        RectTransform track = CreateRect("Progress track", panel);
        track.anchorMin = new Vector2(0.5f, 1f);
        track.anchorMax = new Vector2(0.5f, 1f);
        track.pivot = new Vector2(0.5f, 1f);
        track.anchoredPosition = new Vector2(0f, -241f);
        track.sizeDelta = new Vector2(1000f, 5f);
        Image trackImage = track.gameObject.AddComponent<Image>();
        trackImage.color = new Color(0.28f, 0.31f, 0.33f);
        trackImage.raycastTarget = false;

        RectTransform fill = CreateRect("Progress fill", track);
        fill.anchorMin = Vector2.zero;
        fill.anchorMax = Vector2.one;
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
        progressFill = fill.gameObject.AddComponent<Image>();
        progressFill.color = new Color(1f, 0.74f, 0.35f);
        progressFill.type = Image.Type.Filled;
        progressFill.fillMethod = Image.FillMethod.Horizontal;
        progressFill.raycastTarget = false;
    }

    private static RectTransform CreateRect(string objectName, Transform parent)
    {
        GameObject child = new GameObject(objectName, typeof(RectTransform));
        child.transform.SetParent(parent, false);
        return child.GetComponent<RectTransform>();
    }

    private TMP_Text CreateText(string objectName, Transform parent, Vector2 position, Vector2 size,
        float fontSize, Color color)
    {
        RectTransform rect = CreateRect(objectName, parent);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.font = chineseFont;
        label.fontSize = fontSize;
        label.color = color;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.raycastTarget = false;
        return label;
    }
}
