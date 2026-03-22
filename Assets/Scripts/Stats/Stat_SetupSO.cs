using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Default Stat Setup", fileName = "Default Stat Setup - ")] 
public class Stat_SetupSO : ScriptableObject
{
    [Header("Defence")]
    public float maxHP = 1000;
    public float fireResistance = 80;
    public float iceResistance = 80;
    public float thunderResistance = 80;

    [Header("Offense")]
    public float strength = 10;
    public float intelligence = 10;

    [Header("Element")]
    public float fireDamage = 10;
    public float iceDamage = 10;
    public float thunderDamage = 10;
}
