using UnityEngine;

[ExecuteAlways]  // so you can see changes in the Editor too
[RequireComponent(typeof(Renderer))]
public class RippleShaderController : MonoBehaviour
{
    [Header("Ripple Settings")]
    [Tooltip("Center of the ripple in UV space (0–1)")]
    public Vector2 rippleCenter = new Vector2(0.5f, 0.5f);

    [Tooltip("How many rings")]
    public float rippleCount = 2.55f;
    [Tooltip("Speed at which the rings expand")]
    public float rippleSpeed = 3.38f;
    [Tooltip("Overall strength of the ripple")]
    public float rippleStrength = 0f;

    [Tooltip("Base texture tint")]
    public Color tintColor = Color.white;

    [Space]
    [Header("Crack Overlay")]
    public Texture crackTexture;
    [Tooltip("Strength of the crack mask")]
    public float crackStrength = 0f;
    [Tooltip("Color of the crack mask")]
    public Color crackColor = Color.white;
    [Range(0f, 1f), Tooltip("Threshold for both ripple & cracks")]
    public float threshold = 0f;

    // internal
    Renderer rend;
    MaterialPropertyBlock mpb;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        UpdateShaderProperties();
    }

    // also run in the editor when you tweak values
    void OnValidate()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        if (mpb == null) mpb = new MaterialPropertyBlock();
        UpdateShaderProperties();
    }

    void Update()
    {
        // if you only need runtime changes you can remove OnValidate and
        // call this here — or cache last‐set values to avoid redundant updates.
        UpdateShaderProperties();
    }

    void UpdateShaderProperties()
    {
        // grab the current block
        rend.GetPropertyBlock(mpb);

        // set all of your properties by name
        mpb.SetVector("_RippleCenter", rippleCenter);
        mpb.SetFloat("_RippleCount", rippleCount);
        mpb.SetFloat("_RippleSpeed", rippleSpeed);
        mpb.SetFloat("_RippleStrength", rippleStrength);
        mpb.SetColor("_TintColor", tintColor);

        if (crackTexture != null)
            mpb.SetTexture("_CrackTex", crackTexture);

        mpb.SetFloat("_CrackStrength", crackStrength);
        mpb.SetColor("_CrackColor", crackColor);
        mpb.SetFloat("_Threshold", threshold);

        // push it back onto the renderer
        rend.SetPropertyBlock(mpb);
    }
}
