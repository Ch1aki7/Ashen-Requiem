using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color lightenVfx = Color.yellow;
    private Color originalHitVfxColor;

    [Header("Sockets")]
    [SerializeField] private Transform groundSocket;

    [Header("Thunder Strike")]
    [SerializeField] private GameObject lightningStrikeVfx;

    [Header("Fire Burning")]
    [SerializeField] private GameObject burningVfx;

    [Header("Ice Burst")]
    [SerializeField] private GameObject iceBurstVfx;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
        originalHitVfxColor = Color.white;
    }

    // 元素叠层提示：闪烁对应颜色
    public void FlashElementHit(ElementType element)
    {
        sr.DOKill();
        Color targetColor = Color.white;

        if (element == ElementType.Fire) targetColor = burnVfx;
        else if (element == ElementType.Ice) targetColor = chillVfx;
        else if (element == ElementType.Lightning) targetColor = lightenVfx;

        sr.color = targetColor;
        sr.DOColor(Color.white, 0.5f); // 0.25秒平滑褪色
    }

    public void ThunderStrike()
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
    }

    public void FireBurning()
    {
        Instantiate(burningVfx, groundSocket.position, Quaternion.identity);
    }

    public void IceBurst()
    {
        Instantiate(iceBurstVfx, groundSocket.position, Quaternion.identity);
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
