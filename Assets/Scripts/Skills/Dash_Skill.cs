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
        if (!player.IsGroundDetected())
            return false;

        return base.CanUseSkill();
    }
}
