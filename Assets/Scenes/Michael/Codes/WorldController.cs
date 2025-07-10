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

        // start the auto-switch coroutine
        StartCoroutine(AutoSwitch());
    }

    IEnumerator AutoSwitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);

            // deactivate current, advance index, activate next
            worlds[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % worlds.Length;
            worlds[currentIndex].SetActive(true);

            // apply all appearance changes
            ApplyAppearance(currentIndex);

            // flash glitch
            SetGlitch(glitchNoiseAmount, glitchStrength, glitchScanLinesStrength);
            yield return new WaitForSeconds(glitchDuration);
            SetGlitch(defaultNoise, defaultStrength, defaultScan);
        }
    }

    void ApplyAppearance(int idx)
    {
        // 1) pick one HDR color
        Color rndHDR = Color.HSVToRGB(
            Random.value,
            1f,
            Random.Range(1f, 5f),
            true
        );

        // 2) swap membrane ripple texture + tint
        if (membraneMainTextures != null && idx < membraneMainTextures.Length)
        {
            Texture newMain = membraneMainTextures[idx];
            foreach (var rend in membraneRenderers)
            {
                var mats = rend.materials;
                if (rippleMaterialIndex < mats.Length)
                {
                    mats[rippleMaterialIndex].SetTexture(mainTextureProperty, newMain);
                    mats[rippleMaterialIndex].SetColor(tintColorProperty, rndHDR);
                    rend.materials = mats;
                }
            }
        }

        // 3) recolor lanes
        foreach (var lr in laneRenderers)
        {
            var mat = lr.material;
            foreach (var prop in laneColorProperties)
                mat.SetColor(prop, rndHDR);
        }

        // 4) if this is World3 (idx==2), tint the circuit & flow
        if (idx == 2 && circuitRenderers != null)
        {
            // convert to HSV
            Color.RGBToHSV(rndHDR, out float h, out float s, out float v);

            // compute darker and brighter variants
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
