using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;

    public List<float> modifiers;

    public float GetValue()
    {
        float finalValue = baseValue;

        foreach (float modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void AddModifier(float _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(float _modifier)
    {
        modifiers.Remove(_modifier);
    }
    public void SetBaseValue(float newValue)
    {
        baseValue = newValue;
    }
}
