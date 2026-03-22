using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private ElementType currentEffect = ElementType.None;

    public bool CanBeApplied(ElementType element)
    {
        return currentEffect == ElementType.None;
    }

}
