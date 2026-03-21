using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    public int skillPoint;

    public bool EnoughSkillPoints(int cost) => skillPoint >= cost;
    public void RemoveSkillPoints(int cost) => skillPoint -= cost;
}
