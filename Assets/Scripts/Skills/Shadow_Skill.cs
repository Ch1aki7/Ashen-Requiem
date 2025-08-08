using UnityEngine;

public class Shadow_Skill : Skill
{
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private bool canAttack;

    public void CreateShadow(Transform _shadowPosition ,Vector3 _offset)
    {
        GameObject newShadow = Instantiate(shadowPrefab);
        newShadow.GetComponent<Shadow_Skill_Controller>().SetupShadow(_shadowPosition,canAttack,_offset);
    }

    public void CreateShadowNoIai(Transform _shadowPosition, Vector3 _offset)
    {
        GameObject newShadow = Instantiate(shadowPrefab);
        newShadow.GetComponent<Shadow_Skill_Controller>().SetupShadowNoIai(_shadowPosition, canAttack, _offset);
    }
}
