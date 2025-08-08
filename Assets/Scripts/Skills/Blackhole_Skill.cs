using UnityEngine;

public class Blackhole_Skill : Skill
{
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [Space]
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float shadowAttackCD;
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackhole =Instantiate(blackholePrefab,PlayerManager.instance.player.transform.position,Quaternion.identity);

        Blackhole_Skill_Controller newBlackholeScript =newBlackhole.GetComponent<Blackhole_Skill_Controller>();

        newBlackholeScript.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,amountOfAttacks,shadowAttackCD);
    }

    protected override void Update()
    {
        base.Update();
    }
}
