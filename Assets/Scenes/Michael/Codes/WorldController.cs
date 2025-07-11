using System.Collections;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    [Header("World Setup")]
    public Transform worldsParent;

    [Header("Timing")]
    public float switchInterval = 10f;
    public float glitchDuration = 2f;

    [Header("Full-Screen Glitch")]
    public Material glitchMaterial;
    public float glitchNoiseAmount = 100f;
    public float glitchStrength = 30f;
    public float glitchScanLinesStrength = 0f;

    [Header("Membrane Appearance")]
    [Tooltip("Main ripple texture per world, in same order as your Worlds.")]
    public Texture[] membraneMainTextures;
    public Renderer[] membraneRenderers;
    public int rippleMaterialIndex = 1;
    public string mainTextureProperty = "_MainTex";
    public string tintColorProperty = "_TintColor";

    [Tooltip("How much darker the membrane tint is relative to the lane color (0–1)")]
    [Range(0f, 1f)]
    public float membraneDimFactor = 0.8f;

    [Header("Lane Appearance")]
    [Tooltip("All your lane Renderers")]
    public Renderer[] laneRenderers;
    [Tooltip("Shader property names on your gradient shader")]
    public string[] laneColorProperties = new[] { "_Color01", "_Color02", "_Color03" };

    [Header("Circuit Appearance (World3)")]
    [Tooltip("Renderers on your circuit world (e.g. the big CircuitBackground quad)")]
    public Renderer[] circuitRenderers;
    [Tooltip("Which sub-material index contains the circuit/flow shader")]
    public int circuitMaterialIndex = 0;
    public string circuitColorProperty = "_CircuitColor";
    public string flowColorProperty = "_FlowColor";

    [Tooltip("0 = black, 1 = same brightness, >1 = even brighter")]
    [Range(0f, 12f)] public float circuitDimFactor = 0.5f;
    [Range(0f, 20f)] public float flowBrightFactor = 1.5f;

    // internals
    GameObject[] worlds;
    int currentIndex = 0;
    float defaultNoise,
            defaultStrength,
            defaultScan;

    void Start()
    {
        // gather worlds and activate only the first
        int n = worldsParent.childCount;
        worlds = new GameObject[n];
        for (int i = 0; i < n; i++)
        {
            worlds[i] = worldsParent.GetChild(i).gameObject;
            worlds[i].SetActive(i == 0);
        }

        // cache default glitch values & reset
        defaultNoise = glitchMaterial.GetFloat("_NoiseAmount");
        defaultStrength = glitchMaterial.GetFloat("_GlitchStrength");
        defaultScan = glitchMaterial.GetFloat("_ScanLinesStrength");
        SetGlitch(defaultNoise, defaultStrength, defaultScan);

        // apply initial appearance
        ApplyAppearance(0);

        // start automatic switching
        StartCoroutine(AutoSwitch());
    }

    IEnumerator AutoSwitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);

            // swap worlds
            worlds[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % worlds.Length;
            worlds[currentIndex].SetActive(true);

            // update colors/textures
            ApplyAppearance(currentIndex);

            // glitch flash
            SetGlitch(glitchNoiseAmount, glitchStrength, glitchScanLinesStrength);
            yield return new WaitForSeconds(glitchDuration);
            SetGlitch(defaultNoise, defaultStrength, defaultScan);
        }
    }

    void ApplyAppearance(int idx)
    {
        // 1) pick a base HDR color for lanes
        Color laneHDR = Color.HSVToRGB(
            Random.value,
            1f,
            Random.Range(1f, 5f),
            true
        );

        // 2) derive a slightly darker HDR for membranes
        Color.RGBToHSV(laneHDR, out float h, out float s, out float v);
        float vMem = v * membraneDimFactor;
        Color membraneHDR = Color.HSVToRGB(h, s, vMem, true);

        // 3) swap membrane ripple texture + tint
        if (membraneMainTextures != null && idx < membraneMainTextures.Length)
        {
            Texture newMain = membraneMainTextures[idx];
            foreach (var rend in membraneRenderers)
            {
                var mats = rend.materials;
                if (rippleMaterialIndex < mats.Length)
                {
                    mats[rippleMaterialIndex].SetTexture(mainTextureProperty, newMain);
                    mats[rippleMaterialIndex].SetColor(tintColorProperty, membraneHDR);
                    rend.materials = mats;
                }
            }
        }

        // 4) recolor lanes with the brighter HDR
        foreach (var lr in laneRenderers)
        {
            var mat = lr.material;
            foreach (var prop in laneColorProperties)
                mat.SetColor(prop, laneHDR);
        }

        // 5) if this is World3 (idx == 2), tint the circuit & flow as before
        if (idx == 2 && circuitRenderers != null)
        {
            Color darkCircuit = Color.HSVToRGB(h, s, v * circuitDimFactor, true);
            Color brightFlow = Color.HSVToRGB(h, s, v * flowBrightFactor, true);

            foreach (var cr in circuitRenderers)
            {
                var mats = cr.materials;
                if (circuitMaterialIndex < mats.Length)
                {
                    mats[circuitMaterialIndex].SetColor(circuitColorProperty, darkCircuit);
                    mats[circuitMaterialIndex].SetColor(flowColorProperty, brightFlow);
                    cr.materials = mats;
                }
            }
        }
    }

    void SetGlitch(float noise, float str, float scan)
    {
        glitchMaterial.SetFloat("_NoiseAmount", noise);
        glitchMaterial.SetFloat("_GlitchStrength", str);
        glitchMaterial.SetFloat("_ScanLinesStrength", scan);
    }
}
