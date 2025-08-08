using UnityEngine;

public class Dragon_Skill : Skill
{
    public override bool CanUseSkill()
    {
        if (cooldownTimer <= 0)
        {
            cooldownTimer = cooldown;
            return true;
        }
        Debug.Log("Dragon ain't awaking.");
        return false;
    }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void Update()
    {
        base.Update();
    }
}
