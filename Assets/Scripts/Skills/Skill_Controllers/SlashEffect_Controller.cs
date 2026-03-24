using Unity.VisualScripting;
using UnityEngine;

public class SlashEffect_Controller : MonoBehaviour
{
    private bool triggerCalled;
    private void Start()
    {
        
    }

    private void Update()
    {

        if (triggerCalled)
            Destroy(gameObject);
    }

    private void AnimationTrigger()
    {
        triggerCalled = true;
    }
}
