using UnityEngine;

public class SlashEffect_Generator : Skill
{
    private Player player;
    private void Start()
    {
        player = PlayerManager.instance.player;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Quaternion slashRotation = player.transform.rotation;
        Vector3 slashPosition = player.attackCheck.transform.position;
        if (player.facingRight)
            slashPosition -= new Vector3(0.72f, 0, 0);
        else
            slashPosition += new Vector3(0.72f, 0, 0);

        GameObject newSlash = Instantiate(player.slashEffectPrefab,slashPosition,slashRotation,player.transform);

    }

    protected override void Update()
    {
        base.Update();
    }

}
