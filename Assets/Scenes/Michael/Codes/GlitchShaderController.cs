using UnityEngine;

[ExecuteAlways]             // so tweaks show up in the Editor without entering Play mode
[RequireComponent(typeof(Renderer))]
public class GlitchShaderController : MonoBehaviour
{
    [Header("Glitch Shader Settings")]
    [Tooltip("How much noise distortion to apply")]
    public float noiseAmount = 100f;

    [Tooltip("Overall strength of the glitch offsets")]
    public float glitchStrength = 5f;

    [Tooltip("Intensity of the moving scan-line flicker")]
    public float scanLinesStrength = 0f;

    // internal
    Renderer _renderer;
    MaterialPropertyBlock _mpb;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        UpdateShaderProperties();
    }

    // Called in the Editor whenever you tweak a serialized field
    void OnValidate()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        UpdateShaderProperties();
    }

    void Update()
    {
        // if you only ever tweak at runtime you can remove OnValidate()
        UpdateShaderProperties();
    }

    void UpdateShaderProperties()
    {
        _renderer.GetPropertyBlock(_mpb);

        // These names must match your shader’s property names exactly.
        // In ShaderGraph they’ll typically be "_NoiseAmount", "_GlitchStrength", "_ScanLinesStrength"
        _mpb.SetFloat("_NoiseAmount", noiseAmount);
        _mpb.SetFloat("_GlitchStrength", glitchStrength);
        _mpb.SetFloat("_ScanLinesStrength", scanLinesStrength);

        _renderer.SetPropertyBlock(_mpb);
    }
}
