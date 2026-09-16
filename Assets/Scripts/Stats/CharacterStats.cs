using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CharacterStats : MonoBehaviour
{
    public ElementType LastElementalBurst { get; private set; } = ElementType.None;
    public int ElementalBurstCount { get; private set; }

    public void RecordElementalBurst(ElementType element)
    {
        LastElementalBurst = element;
        ElementalBurstCount++;
    }
    public Stat_SetupSO defaultStatSetup;

    [Header("Major Stats")]
    public Stat strength;
    public Stat intelligence;

    public Stat damage;

    [Header("Defensive Stats")]
    public Stat maxHP;
    public Stat fireResistance;
    public Stat iceResistance;
    public Stat thunderResistance;

    [Header("Element Stats")]
    public ElementType elementType;
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat thunderDamage;

    [Header("异常计数")]
    public bool isIgnited; // fire dot
    public bool isChilled; // freeze
    public bool isShocked; // paralysis

    private float igniteTimer;
    private float igniteDamageCD = 1;
    private float igniteDamageTimer;
    [SerializeField] public int curBurnCharge;
    [SerializeField] public int maxBurnCharge = 3;
    private Coroutine igniteCoroutine;

    private float chilledTimer;
    [SerializeField] public int curFreezeCharge;
    [SerializeField] public int maxFreezeCharge = 3;

    private float shockedTimer;
    [SerializeField] public int curShockCharge;
    [SerializeField] public int maxShockCharge = 3;


    public float currentHP;
    protected virtual void Start()
    {
        currentHP = maxHP.GetValue();
    }

    protected virtual void Update()
    {
        igniteTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (igniteTimer < 0)
            isIgnited = false;

        if(chilledTimer < 0)
            isChilled = false;

        if(shockedTimer < 0)
            isShocked = false;

        if (igniteDamageTimer < 0 && isIgnited)
        {
            igniteDamageTimer = igniteDamageCD;
            TakeDamage(1);
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        float totalDamage = damage.GetValue() + strength.GetValue();
        _targetStats.TakeDamage(totalDamage);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        float _fireDamage = fireDamage.GetValue();
        float _iceDamage = iceDamage.GetValue();
        float _thunderDamage = thunderDamage.GetValue();

        _fireDamage = CheckTargetResistance(_targetStats, _fireDamage, ElementType.Fire);
        _iceDamage = CheckTargetResistance(_targetStats, _iceDamage, ElementType.Ice);
        _thunderDamage = CheckTargetResistance(_targetStats, _thunderDamage, ElementType.Lightning);

        float totalMagicalDamage = _fireDamage + _iceDamage + _thunderDamage + intelligence.GetValue();

        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _thunderDamage) <= 0)
            return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _thunderDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _thunderDamage;
        bool canApplyShock = _thunderDamage > _fireDamage && _thunderDamage > _iceDamage;


        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock, out ElementType element);

    }

    private static float CheckTargetResistance(CharacterStats _targetStats, float elementDamage, ElementType element)
    {
        float damageMultiplier = 1f;

        switch (element)
        {
            case ElementType.Fire:
                damageMultiplier = _targetStats.fireResistance.GetValue() / 100f;
                break;
            case ElementType.Ice:
                damageMultiplier = _targetStats.iceResistance.GetValue() / 100f;
                break;
            case ElementType.Lightning:
                damageMultiplier = _targetStats.thunderResistance.GetValue() / 100f;
                break;
            default:
                // 如果没有对应抗性，默认承受 100% 伤害
                damageMultiplier = 1f;
                break;
        }

        // 这里用 Mathf.Max 保证最少受 0 点伤害
        damageMultiplier = Mathf.Max(damageMultiplier, 0f);

        elementDamage = elementDamage * damageMultiplier;

        return elementDamage;
    }

    public void UpdateResistance(char op, ElementType element, float value)
    {
        Stat targetResistance = null;
        switch (element)
        {
            case ElementType.Fire:
                targetResistance = fireResistance;
                break;
            case ElementType.Ice:
                targetResistance = iceResistance;
                break;
            case ElementType.Lightning:
                targetResistance = thunderResistance;
                break;
            default:
                Debug.LogWarning("未知的元素类型，无法修改抗性！");
                return;
        }

        float currentValue = targetResistance.GetValue();
        float finalValue = currentValue;

        switch (op)
        {
            case '+':
                finalValue = currentValue + value;
                break;
            case '-':
                finalValue = currentValue - value;
                break;
            case '*':
                finalValue = currentValue * value;
                break;
            case '/':
                if (value != 0)
                {
                    finalValue = currentValue / value;
                }
                else
                {
                    Debug.LogWarning("抗性计算错误：除数不能为0！");
                }
                break;
            default:
                Debug.LogWarning($"未知的操作符: {op}");
                return;
        }

        targetResistance.SetBaseValue(finalValue);
    }

    public virtual void ApplyAilments(bool _ignite, bool _chill, bool _shock, out ElementType element)
    {
        if (_ignite)
        {
            isIgnited = _ignite;
            element = ElementType.Fire;
            igniteTimer = 2;
            return;
        }

        if (_chill)
        {
            isChilled = _chill;
            element = ElementType.Ice;
            chilledTimer = 2;
            return;
        }

        if (_shock)
        {
            isShocked = _shock;
            element = ElementType.Lightning;
            shockedTimer = 2;
            return;
        }

        element = ElementType.None;
        return;
    }

    public virtual void TakeDamage(float _damage)
    {
        currentHP -= _damage;

        if (currentHP <= 0)
            Die();
    }

    public Stat GetResistanceStat(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return fireResistance;
            case ElementType.Ice: return iceResistance;
            case ElementType.Lightning: return thunderResistance;
            default: return null;
        }
    }

    public Stat GetElementDamageStat(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return fireDamage;
            case ElementType.Ice: return iceDamage;
            case ElementType.Lightning: return thunderDamage;
            default: return null;
        }
    }

    public void ApplyResistanceBuff(ElementType element, int modifierValue, float duration)
    {
        StartCoroutine(ResistanceBuffRoutine(element, modifierValue, duration));
    }

    private IEnumerator ResistanceBuffRoutine(ElementType element, int modifierValue, float duration)
    {
        Stat targetStat = GetResistanceStat(element);
        if (targetStat == null) yield break;

        targetStat.AddModifier(modifierValue);

        yield return new WaitForSeconds(duration);

        targetStat.RemoveModifier(modifierValue);
    }

    #region 武器附魔协程
    public void ApplyWeaponEnchantment(ElementType element, int modifieralue, float duration)
    {
        StartCoroutine(EnchantmentRoutine(element, modifieralue, duration));
    }

    private IEnumerator EnchantmentRoutine(ElementType element, int modifierValue, float duration)
    {
        Stat targetStat = GetElementDamageStat(element);
        if (targetStat == null) yield break;

        targetStat.AddModifier(modifierValue);

        yield return new WaitForSeconds(duration);

        targetStat.RemoveModifier(modifierValue);
    }
    #endregion

    #region 火焰dot协程
    public void StartIgniteDoT(float duration, float tickRate, float damagePerTick)
    {
        if (igniteCoroutine != null)
            StopCoroutine(igniteCoroutine);

        igniteCoroutine = StartCoroutine(IgniteRoutine(duration, tickRate, damagePerTick));
    }

    private IEnumerator IgniteRoutine(float duration, float tickRate, float damagePerTick)
    {
        isIgnited = true;
        float timer = duration;

        while (timer > 0)
        {
            yield return new WaitForSeconds(tickRate);
            timer -= tickRate;

            TakeDamage(damagePerTick);

            Entity entity = GetComponent<Entity>();
            if (entity != null && entity.fx != null)
            {
                entity.fx.FireBurning();
                entity.fx.FlashElementHit(ElementType.Fire);
            }
        }

        isIgnited = false;
        igniteCoroutine = null;
    }
    #endregion
    protected virtual void Die()
    {
        StopAllCoroutines();
    }

    [ContextMenu("更新默认设置")]
    public void ApplyDefaultSetup()
    {
        if (!defaultStatSetup)
        {
            Debug.Log("没有默认属性设置");
            return;
        }

        maxHP.SetBaseValue(defaultStatSetup.maxHP);
        fireResistance.SetBaseValue(defaultStatSetup.fireResistance);
        iceResistance.SetBaseValue(defaultStatSetup.iceResistance);
        thunderResistance.SetBaseValue(defaultStatSetup.thunderResistance);

        strength.SetBaseValue(defaultStatSetup.strength);
        intelligence.SetBaseValue(defaultStatSetup.intelligence);

        fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        thunderDamage.SetBaseValue(defaultStatSetup.thunderDamage);
    }
}
