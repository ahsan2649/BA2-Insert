using UnityEngine;

public class World4Controller : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The quad/plane with your RippleShader material")]
    public Renderer backgroundRenderer;

    // --- these fields map 1:1 to your Shader Graph Blackboard ---

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
    [Range(0f, 1f)]
    public float crackStrength = 0.8f;

    [Tooltip("Color to tint the cracks")]
    public Color crackColor = Color.white;

    [Tooltip("Threshold for crack visibility")]
    [Range(0f, 1f)]
    public float threshold = 0f;

    [Tooltip("If true, updates the shared material as you tweak in Edit mode")]
    public bool liveEditInEditor = false;

    Material _instancedMat;

    void Awake()
    {
        if (backgroundRenderer != null)
        {
            // instantiate a copy so we don’t modify the asset
            _instancedMat = backgroundRenderer.material;
        }
    }

    /// <summary>
    /// Call this from your SphereMover.OnCollisionEnter,
    /// passing in the world-space contact point.
    /// </summary>
    public void TriggerHitEffect(Vector3 worldHitPos)
    {
        if (_instancedMat == null) return;

        // 1) convert world pos → viewport (0–1)
        var vp = Camera.main.WorldToViewportPoint(worldHitPos);
        _instancedMat.SetVector("_RippleCenter", new Vector4(vp.x, vp.y, 0f, 0f));

        // 2) write all your Inspector-driven values
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

#if UNITY_EDITOR
    // Live-edit in the Scene view when you tweak values in Edit mode
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
}
