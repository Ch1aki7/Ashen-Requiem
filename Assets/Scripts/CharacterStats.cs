using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterStats : MonoBehaviour
{
    [Header("Major Stats")]
    public Stat strength;
    public Stat intelligence;

    public Stat damage;

    [Header("Defensive Stats")]
    public Stat maxHP;
    public Stat ElementResistance;

    [Header("Element Stats")]
    public ElementType elementType;
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat thunderDamage;

    [Header("Òì³£¼ÆÊý")]
    public bool isIgnited; // fire dot
    public bool isChilled; // freeze
    public bool isShocked; // paralysis

    private float igniteTimer;
    private float igniteDamageCD = 1;
    private float igniteDamageTimer;
    [SerializeField] public int curBurnCharge;
    [SerializeField] public int maxBurnCharge = 3;

    private float chilledTimer;
    [SerializeField] public int curFreezeCharge;
    [SerializeField] public int maxFreezeCharge = 3;

    private float shockedTimer;
    [SerializeField] public int curShockCharge;
    [SerializeField] public int maxShockCharge = 3;


    public int currentHP;
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
        int totalDamage = damage.GetValue() + strength.GetValue();
        _targetStats.TakeDamage(totalDamage);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _thunderDamage = thunderDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _thunderDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _thunderDamage) <= 0)
            return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _thunderDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _thunderDamage;
        bool canApplyShock = _thunderDamage > _fireDamage && _thunderDamage > _iceDamage;


        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock, out ElementType element);

    }


    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.ElementResistance.GetValue();
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
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

    public virtual void TakeDamage(int _damage)
    {
        currentHP -= _damage;

        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {

    }
}
