using UnityEngine;
public class Dash_Skill : Skill
{
    private Player player;

    private void Start()
    {
        player = PlayerManager.instance.player;
    }
    public override void UseSkill()
    {
        base.UseSkill();
    }

    public override bool CanUseSkill()
    {
        if (player.IsGroundDetected())
        {
            if (cooldownTimer <= 0)
            {
                cooldownTimer = cooldown;
                return true;
            }
            Debug.Log("冲多了导致的");
            return false;
        }
        return false;
    }
}
