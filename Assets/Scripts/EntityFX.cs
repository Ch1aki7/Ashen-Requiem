using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Element Colors")]
    [SerializeField] private Color chillvfx = Color.cyan;
    private Color originalHitVfxColor;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
        originalHitVfxColor = Color.white;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        yield return new WaitForSeconds(.2f);
        sr.material = originalMat;
    }
    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
            sr.color = Color.red;
    }
    private void CancelRedBlink()
    {
        CancelInvoke();
        sr.color = Color.white;
    }
}
