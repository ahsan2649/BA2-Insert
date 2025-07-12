using System.Collections.Generic;
using UnityEngine;

public class World4Controller : MonoBehaviour
{
    // === ORIGINAL RIPPLE & CRACK FIELDS ===

    [Header("References")]
    [Tooltip("The quad/plane with your RippleShader material")]
    public Renderer backgroundRenderer;

    [Header("Ripple Settings")]
    [Tooltip("Where the ripple originates (0–1 in UV space)")]
    public Vector2 rippleCenter = new Vector2(0, 1);
    [Tooltip("How many concentric rings to spawn")]
    public int rippleCount = 1;
    [Tooltip("Speed at which ripples expand")]
    public float rippleSpeed = 5f;
    [Tooltip("Amplitude of the ripple distortion")]
    public float rippleStrength = 2f;
    [Tooltip("Tint color multiply for the whole background")]
    public Color tintColor = Color.white;

    [Header("Crack Settings")]
    [Tooltip("Texture to use for cracks")]
    public Texture crackTexture;
    [Tooltip("How intensely cracks are shown")]
    [Range(0f, 1f)] public float crackStrength = 0.8f;
    [Tooltip("Color to tint the cracks")]
    public Color crackColor = Color.white;
    [Tooltip("Threshold for crack visibility")]
    [Range(0f, 1f)] public float threshold = 0f;

    [Tooltip("If true, updates the shared material as you tweak in Edit mode")]
    public bool liveEditInEditor = false;

    // === BUBBLE SYSTEM FIELDS ===

    [Header("Bubble Settings")]
    [Tooltip("Prefab of your Bubble (must have BubbleController)")]
    public GameObject bubblePrefab;
    [Tooltip("Max simultaneous bubbles")]
    public int maxBubbles = 10;

    // === PRIVATE RUNTIME STATE ===

    private Material _instancedMat;
    private bool _bubblesActivated = false;
    private List<GameObject> _activeBubbles = new List<GameObject>();

    // === UNITY LIFECYCLE ===

    void Awake()
    {
        if (backgroundRenderer != null)
            _instancedMat = backgroundRenderer.material;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!liveEditInEditor || backgroundRenderer == null) return;

        var shared = backgroundRenderer.sharedMaterial;
        shared.SetVector("_RippleCenter", new Vector4(rippleCenter.x, rippleCenter.y, 0, 0));
        shared.SetInt("_RippleCount", rippleCount);
        shared.SetFloat("_RippleSpeed", rippleSpeed);
        shared.SetFloat("_RippleStrength", rippleStrength);
        shared.SetColor("_TintColor", tintColor);

        if (crackTexture != null)
            shared.SetTexture("_CrackTexture", crackTexture);

        shared.SetFloat("_CrackStrength", crackStrength);
        shared.SetColor("_CrackColor", crackColor);
        shared.SetFloat("_Threshold", threshold);
    }
#endif

    void OnDrawGizmosSelected()
    {
        if (backgroundRenderer == null) return;

        Bounds b = backgroundRenderer.bounds;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(b.center, b.size);
    }

    // === PUBLIC API ===

    /// <summary>
    /// Call this once when your two spheres collide.
    /// </summary>
    public void ActivateBubbles()
    {
        if (bubblePrefab == null)
        {
            Debug.LogWarning("World4Controller: bubblePrefab not assigned.");
            return;
        }

        if (_bubblesActivated) return;
        _bubblesActivated = true;

        for (int i = 0; i < maxBubbles; i++)
            SpawnBubble();
    }

    /// <summary>
    /// Call this from your SphereMover.OnCollisionEnter to trigger a ripple+crack shader effect.
    /// </summary>
    public void TriggerHitEffect(Vector3 worldHitPos)
    {
        if (_instancedMat == null) return;

        var vp = Camera.main.WorldToViewportPoint(worldHitPos);
        _instancedMat.SetVector("_RippleCenter", new Vector4(vp.x, vp.y, 0f, 0f));
        _instancedMat.SetInt("_RippleCount", rippleCount);
        _instancedMat.SetFloat("_RippleSpeed", rippleSpeed);
        _instancedMat.SetFloat("_RippleStrength", rippleStrength);
        _instancedMat.SetColor("_TintColor", tintColor);

        if (crackTexture != null)
            _instancedMat.SetTexture("_CrackTexture", crackTexture);

        _instancedMat.SetFloat("_CrackStrength", crackStrength);
        _instancedMat.SetColor("_CrackColor", crackColor);
        _instancedMat.SetFloat("_Threshold", threshold);
    }

    /// <summary>
    /// Called by each BubbleController when its lifetime ends.
    /// </summary>
    public void NotifyBubbleDestroyed(GameObject bubble)
    {
        _activeBubbles.Remove(bubble);
        SpawnBubble();
    }

    // === PRIVATE HELPERS ===

    private void SpawnBubble()
    {
        if (!_bubblesActivated || _activeBubbles.Count >= maxBubbles)
            return;

        if (backgroundRenderer == null)
            return;

        // 1) Get the exact world-space bounds of your quad
        Bounds b = backgroundRenderer.bounds;

        // 2) Pick a random point inside X/Y, just in front of the plane on Z
        Vector3 spawnPos = new Vector3(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y),
            b.center.z + 0.1f
        );

        // 3) Instantiate as a child of World4
        GameObject bub = Instantiate(
            bubblePrefab,
            spawnPos,
            Quaternion.identity,
            this.transform
        );

        // 4) Wire up the controller reference
        if (bub.TryGetComponent<BubbleController>(out var bc))
            bc.world4Controller = this;

        _activeBubbles.Add(bub);
    }
}
