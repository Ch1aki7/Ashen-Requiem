using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpatialCleaveSkill : Skill
{
    [Header("空间裂隙表现")]
    [SerializeField] private Material screenSliceMaterial;
    [SerializeField, Min(0.2f)] private float sliceDuration = 1.2f;
    [SerializeField, Range(0.005f, 0.12f)] private float maxOffset = 0.06f;
    [SerializeField, Range(0.02f, 0.2f)] private float anticipationDuration = 0.08f;
    [SerializeField, Range(0.04f, 0.3f)] private float ruptureHoldDuration = 0.12f;
    [SerializeField, Range(0.001f, 0.02f)] private float fractureWidth = 0.005f;
    [SerializeField, Range(0f, 2f)] private float chromaticAberration = 0.9f;
    [SerializeField, Range(0f, 0.01f)] private float jitterAmount = 0.0025f;

    [Header("攻击与伤害判定")]
    [FormerlySerializedAs("sliceAngle")]
    [SerializeField] private float baseSliceAngle = 45f;
    [SerializeField, Range(0f, 180f)] private float angleRandomRange = 180f;
    [SerializeField, Min(0.1f)] private float damageThickness = 2.5f;
    [SerializeField] private int massiveDamage = 9999;
    [SerializeField] private LayerMask enemyLayer;

    private bool isSlicing;
    private Camera mainCam;

    private static readonly int SliceAngleID = Shader.PropertyToID("_SliceAngle");
    private static readonly int SliceCenterID = Shader.PropertyToID("_SliceCenter");
    private static readonly int SliceOffsetID = Shader.PropertyToID("_SliceOffset");
    private static readonly int EdgeColorID = Shader.PropertyToID("_EdgeGlowColor");
    private static readonly int EffectStrengthID = Shader.PropertyToID("_EffectStrength");
    private static readonly int FractureWidthID = Shader.PropertyToID("_FractureWidth");
    private static readonly int ChromaticAberrationID = Shader.PropertyToID("_ChromaticAberration");
    private static readonly int JitterID = Shader.PropertyToID("_Jitter");
    private static readonly int SliceSeedID = Shader.PropertyToID("_SliceSeed");

    private static readonly Color RuptureColor = new Color(1.8f, 5.5f, 8f, 1f);
    private static readonly Color SettleColor = new Color(0.1f, 1.8f, 3.5f, 1f);

    private void Awake()
    {
        mainCam = Camera.main;
        ResetVisual();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ResetVisual();
        isSlicing = false;
    }

    public override bool CanUseSkill()
    {
        return !isSlicing && base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (!isSlicing)
            StartCoroutine(ExecuteSpatialCleave());
    }

    private IEnumerator ExecuteSpatialCleave()
    {
        isSlicing = true;

        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam == null || screenSliceMaterial == null)
        {
            Debug.LogError("次元斩缺少主摄像机或全屏裂隙材质。", this);
            isSlicing = false;
            yield break;
        }

        float currentAngle = baseSliceAngle + Random.Range(-angleRandomRange, angleRandomRange);
        Vector3 mousePosition = Input.mousePosition;
        Vector2 shaderCenter = new Vector2(
            mousePosition.x / Mathf.Max(Screen.width, 1),
            mousePosition.y / Mathf.Max(Screen.height, 1));

        float cameraDistance = Mathf.Abs(mainCam.transform.position.z);
        Vector2 worldCenter = mainCam.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, cameraDistance));

        float angleRadians = currentAngle * Mathf.Deg2Rad;
        Vector2 offsetDirection = new Vector2(-Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));

        screenSliceMaterial.SetFloat(SliceAngleID, currentAngle);
        screenSliceMaterial.SetVector(SliceCenterID, shaderCenter);
        screenSliceMaterial.SetFloat(FractureWidthID, fractureWidth);
        screenSliceMaterial.SetFloat(ChromaticAberrationID, chromaticAberration);
        screenSliceMaterial.SetFloat(SliceSeedID, Random.Range(0f, 1000f));

        float elapsed = 0f;
        while (elapsed < anticipationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / anticipationDuration);
            float pulse = Mathf.SmoothStep(0f, 1f, t);

            screenSliceMaterial.SetFloat(EffectStrengthID, pulse * 0.18f);
            screenSliceMaterial.SetVector(SliceOffsetID, offsetDirection * (maxOffset * pulse * 0.08f));
            screenSliceMaterial.SetColor(EdgeColorID, SettleColor * (0.15f + pulse * 0.25f));
            yield return null;
        }

        ApplyDamage(worldCenter, currentAngle, offsetDirection);

        AttackScene attackScene = AttackScene.Instance;
        if (attackScene != null)
        {
            attackScene.HitPause(10);
            attackScene.CameraShake(0.22f, 3.2f);
        }

        elapsed = 0f;
        while (elapsed < ruptureHoldDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / ruptureHoldDuration);
            float pulse = 1f + Mathf.Sin(t * Mathf.PI * 5f) * 0.08f;
            Vector2 jitter = Random.insideUnitCircle * jitterAmount * (1f - t * 0.35f);

            screenSliceMaterial.SetFloat(EffectStrengthID, 1f);
            screenSliceMaterial.SetVector(SliceOffsetID, offsetDirection * (maxOffset * pulse));
            screenSliceMaterial.SetVector(JitterID, jitter);
            screenSliceMaterial.SetColor(EdgeColorID, Color.Lerp(RuptureColor, SettleColor * 2f, t));
            yield return null;
        }

        float settleDuration = Mathf.Max(0.08f, sliceDuration - anticipationDuration - ruptureHoldDuration);
        elapsed = 0f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / settleDuration);
            float decay = Mathf.Pow(1f - t, 2.2f);
            float recoil = 1f + Mathf.Sin(t * Mathf.PI * 3f) * 0.12f * (1f - t);
            Vector2 jitter = Random.insideUnitCircle * jitterAmount * decay;

            screenSliceMaterial.SetFloat(EffectStrengthID, decay);
            screenSliceMaterial.SetVector(SliceOffsetID, offsetDirection * (maxOffset * decay * recoil));
            screenSliceMaterial.SetVector(JitterID, jitter);
            screenSliceMaterial.SetColor(EdgeColorID, SettleColor * decay);
            yield return null;
        }

        ResetVisual();
        isSlicing = false;
    }

    private void ApplyDamage(Vector2 worldCenter, float angle, Vector2 knockbackDirection)
    {
        Vector2 boxSize = new Vector2(100f, damageThickness);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            worldCenter,
            boxSize,
            angle,
            Vector2.zero,
            0f,
            enemyLayer);

        HashSet<EnemyStats> damagedEnemies = new HashSet<EnemyStats>();
        foreach (RaycastHit2D hit in hits)
        {
            EnemyStats enemy = hit.collider.GetComponentInParent<EnemyStats>();
            if (enemy == null || !damagedEnemies.Add(enemy))
                continue;

            enemy.TakeDamage(massiveDamage);

            Rigidbody2D body = hit.rigidbody;
            if (body != null)
                body.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }
    }

    private void ResetVisual()
    {
        if (screenSliceMaterial == null)
            return;

        screenSliceMaterial.SetFloat(EffectStrengthID, 0f);
        screenSliceMaterial.SetVector(SliceOffsetID, Vector4.zero);
        screenSliceMaterial.SetVector(JitterID, Vector4.zero);
        screenSliceMaterial.SetColor(EdgeColorID, Color.black);
    }
}