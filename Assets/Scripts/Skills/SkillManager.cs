using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;


    public Dash_Skill dash {  get; private set; }
    public Shadow_Skill shadow { get; private set; }
    public Blackhole_Skill blackhole { get; private set; }
    public Dragon_Skill dragon { get; private set; }
    public SlashEffect_Generator slashEffect { get; private set; }
    public Oath_Skill oath {  get; private set; }
    public SpatialCleaveSkill spatialCleave { get; private set; }
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
        shadow = GetComponent<Shadow_Skill>();
        blackhole = GetComponent<Blackhole_Skill>();
        dragon = GetComponent<Dragon_Skill>();
        slashEffect=GetComponent<SlashEffect_Generator>();
        oath = GetComponent<Oath_Skill>();
        spatialCleave = GetComponent<SpatialCleaveSkill>();
    }
}
