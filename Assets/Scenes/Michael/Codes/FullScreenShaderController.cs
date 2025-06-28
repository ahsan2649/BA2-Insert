using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FullScreenShaderController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material fullscreenMat;  // assign your FullScreenMat here

    [Header("Vignette Settings")]
    [SerializeField, Range(0, 5f)] private float vignettePower = 1.36f;
    [SerializeField, Range(0, 1f)] private float vignetteIntensity = 0.29f;

    [Header("Noise Settings")]
    [SerializeField] private Vector2 noiseTiling = new Vector2(1, 1);
    [SerializeField] private Vector2 noiseSpeed = new Vector2(0.5f, 0f);

    [Header("Breathing Effect")]
    [SerializeField] private float breathFigure = 4f;
    [SerializeField] private float breathIntensity = 2f;

    // Cache the shader property IDs for performance
    private static readonly int
        _VignettePowerID = Shader.PropertyToID("VignettePower"),
        _VignetteIntensityID = Shader.PropertyToID("VignetteIntensity"),
        _NoiseTilingID = Shader.PropertyToID("NoiseTiling"),
        _NoiseSpeedID = Shader.PropertyToID("NoiseSpeed"),
        _BreathFigureID = Shader.PropertyToID("BreathFigure"),
        _BreathIntensityID = Shader.PropertyToID("BreathIntensity");

    void Update()
    {
        fullscreenMat.SetFloat(_VignettePowerID, vignettePower);
        fullscreenMat.SetFloat(_VignetteIntensityID, vignetteIntensity);

        fullscreenMat.SetVector(_NoiseTilingID, noiseTiling);
        fullscreenMat.SetVector(_NoiseSpeedID, noiseSpeed);

        fullscreenMat.SetFloat(_BreathFigureID, breathFigure);
        fullscreenMat.SetFloat(_BreathIntensityID, breathIntensity);
    }
}
