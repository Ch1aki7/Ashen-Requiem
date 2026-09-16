using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oath_Skill : Skill
{
    private Player player;
    private SpriteRenderer playerSpriteRenderer;
    private Material playerMaterial;

    [Header("Element enchant colors (HDR)")]
    [ColorUsage(true, true)]
    [SerializeField] private Color fireSwordColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color iceSwordColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color lightningSwordColor;

    [SerializeField] private string shaderColorPropertyName = "_SwordColor";

    public ElementType CurrentElement { get; private set; } = ElementType.None;
    public int EnchantmentUseCount { get; private set; }

    private readonly Queue<ElementType> tutorialSequence = new Queue<ElementType>();
    private Color defaultSwordColor;
    private Coroutine resetColorCoroutine;
    private Coroutine tutorialEnchantCoroutine;
    private ElementType tutorialAppliedElement = ElementType.None;
    private bool tutorialMode;

    private void Start()
    {
        player = PlayerManager.instance.player;
        playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();

        if (playerSpriteRenderer != null)
        {
            playerMaterial = playerSpriteRenderer.sharedMaterial;
            defaultSwordColor = playerMaterial.GetColor(shaderColorPropertyName);
        }
    }

    public void ConfigureTutorialSequence(params ElementType[] elements)
    {
        tutorialSequence.Clear();
        foreach (ElementType element in elements)
            tutorialSequence.Enqueue(element);

        tutorialMode = tutorialSequence.Count > 0;
        cooldownTimer = 0f;
    }

    public void ReadyNextTutorialEnchant()
    {
        if (tutorialMode)
            cooldownTimer = 0f;
    }

    public override bool CanUseSkill() => base.CanUseSkill();

    public override void UseSkill()
    {
        base.UseSkill();

        ElementType selectedElement;
        if (tutorialSequence.Count > 0)
        {
            selectedElement = tutorialSequence.Dequeue();
        }
        else
        {
            System.Array elements = System.Enum.GetValues(typeof(ElementType));
            selectedElement = (ElementType)elements.GetValue(Random.Range(1, elements.Length));
        }

        CurrentElement = selectedElement;
        EnchantmentUseCount++;

        float enchantDuration = tutorialMode ? 4f : 3f;
        if (tutorialMode)
            ApplyTutorialEnchantment(selectedElement, enchantDuration);
        else
            player.stats.ApplyWeaponEnchantment(selectedElement, 10, enchantDuration);
        ChangeSwordColor(selectedElement);

        if (resetColorCoroutine != null)
            StopCoroutine(resetColorCoroutine);

        resetColorCoroutine = StartCoroutine(ResetColorAfterDelay(enchantDuration));
    }

    private void ApplyTutorialEnchantment(ElementType element, float duration)
    {
        if (tutorialEnchantCoroutine != null)
        {
            StopCoroutine(tutorialEnchantCoroutine);
            RemoveTutorialEnchantment();
        }

        tutorialAppliedElement = element;
        player.stats.GetElementDamageStat(element).AddModifier(10);
        tutorialEnchantCoroutine = StartCoroutine(RemoveTutorialEnchantmentAfter(duration));
    }

    private IEnumerator RemoveTutorialEnchantmentAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveTutorialEnchantment();
        tutorialEnchantCoroutine = null;
    }

    private void RemoveTutorialEnchantment()
    {
        if (tutorialAppliedElement == ElementType.None)
            return;

        player.stats.GetElementDamageStat(tutorialAppliedElement).RemoveModifier(10);
        tutorialAppliedElement = ElementType.None;
    }

    private void ChangeSwordColor(ElementType element)
    {
        if (playerMaterial == null || playerSpriteRenderer == null)
            return;

        Color targetColor = Color.white;
        switch (element)
        {
            case ElementType.Fire:
                targetColor = fireSwordColor;
                break;
            case ElementType.Ice:
                targetColor = iceSwordColor;
                break;
            case ElementType.Lightning:
                targetColor = lightningSwordColor;
                break;
        }

        playerSpriteRenderer.material.SetColor(shaderColorPropertyName, targetColor);
        playerMaterial.SetColor(shaderColorPropertyName, targetColor);
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerSpriteRenderer != null && playerMaterial != null)
        {
            playerSpriteRenderer.material.SetColor(shaderColorPropertyName, defaultSwordColor);
            playerMaterial.SetColor(shaderColorPropertyName, defaultSwordColor);
        }

        resetColorCoroutine = null;
    }

    private void OnApplicationQuit()
    {
        if (playerMaterial != null)
            playerMaterial.SetColor(shaderColorPropertyName, defaultSwordColor);
    }
}
