using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Entity entity;
    private CharacterStats cs;
    private RectTransform rt;
    private Slider slider;

    private void Start()
    {
        slider=GetComponentInChildren<Slider>();
        rt = GetComponent<RectTransform>();
        entity=GetComponentInParent<Entity>();
        cs=GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;
    }

    private void Update()
    {
        UpdateHealthUI();
    }
    private void UpdateHealthUI()
    {
        slider.maxValue = cs.maxHP.GetValue();
        slider.value = cs.currentHP;
    }


    private void FlipUI()
    {
        rt.Rotate(0, 180, 0);
    }


}
