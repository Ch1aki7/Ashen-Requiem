using Unity.VisualScripting;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header("总体信息")]
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (OnCoolDown())
        {
            Debug.Log("技能冷却");
            return false;
        }
            return true;
    }

    public virtual void UseSkill()
    {

    }

    private bool OnCoolDown() => cooldownTimer > 0;
    public void SetSkillCD() => cooldownTimer = cooldown;
}
