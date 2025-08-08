using Unity.VisualScripting;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer <= 0)
        {
            cooldownTimer = cooldown;
            return true;
        }
        Debug.Log("Skill is on CD.");
        return false;
    }

    public virtual void UseSkill()
    {

    }
}
