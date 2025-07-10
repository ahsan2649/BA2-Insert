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
    public Texture[] membraneMainTextures;
    public Color[] membraneTintColors;

    [Tooltip("Left & Right membrane renderers (Gradient=0, RippleMat=1).")]
    public Renderer[] membraneRenderers;
    public int rippleMaterialIndex = 1;
    public string mainTextureProperty = "_MainTex";
    public string tintColorProperty = "_TintColor";

    [Header("Lane Appearance")]
    [Tooltip("All your lane Renderers")]
    public Renderer[] laneRenderers;
    [Tooltip("Shader property names on your gradient shader")]
    public string[] laneColorProperties = new string[] { "_Color01", "_Color02", "_Color03" };

    // internals
    GameObject[] worlds;
    int currentIndex = 0;
    float defaultNoise, defaultStrength, defaultScan;

    void Start()
    {
        // 1) collect your worlds
        int n = worldsParent.childCount;
        worlds = new GameObject[n];
        for (int i = 0; i < n; i++)
        {
            var w = worldsParent.GetChild(i).gameObject;
            worlds[i] = w;
            w.SetActive(i == 0);
        }

        // 2) cache glitch defaults & force idle state
        if (glitchMaterial == null)
        {
            Debug.LogError("Assign your GlitchMat!");
            enabled = false;
            return;
        }
        defaultNoise = glitchMaterial.GetFloat("_NoiseAmount");
        defaultStrength = glitchMaterial.GetFloat("_GlitchStrength");
        defaultScan = glitchMaterial.GetFloat("_ScanLinesStrength");
        SetGlitchValues(defaultNoise, defaultStrength, defaultScan);

        // 3) apply initial appearance
        ApplyAppearance(0);

        // 4) start the auto‐loop
        StartCoroutine(AutoSwitchRoutine());
    }

    IEnumerator AutoSwitchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);

            // swap worlds
            worlds[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % worlds.Length;
            worlds[currentIndex].SetActive(true);

            // apply texture + tint
            ApplyAppearance(currentIndex);

            // flash glitch
            SetGlitchValues(glitchNoiseAmount, glitchStrength, glitchScanLinesStrength);
            yield return new WaitForSeconds(glitchDuration);
            SetGlitchValues(defaultNoise, defaultStrength, defaultScan);
        }
    }

    void ApplyAppearance(int worldIndex)
    {
        // --- Membrane: texture + tint ---
        if (membraneMainTextures != null
         && worldIndex < membraneMainTextures.Length
         && membraneTintColors != null
         && worldIndex < membraneTintColors.Length
         && membraneRenderers != null)
        {
            Texture newMain = membraneMainTextures[worldIndex];
            Color newTint = membraneTintColors[worldIndex];

            foreach (var rend in membraneRenderers)
            {
                var mats = rend.materials;
                if (rippleMaterialIndex < mats.Length)
                {
                    mats[rippleMaterialIndex].SetTexture(mainTextureProperty, newMain);
                    mats[rippleMaterialIndex].SetColor(tintColorProperty, newTint);
                    rend.materials = mats;
                }
            }
        }

        // --- Lanes: random HDR color ---
        Color randomHDR = Color.HSVToRGB(
            Random.value,    // random hue
            1f,              // full saturation
            Random.Range(1f, 5f),  // brightness >1 for HDR
            true             // HDR mode
        );

        if (laneRenderers != null && laneColorProperties != null)
        {
            foreach (var lr in laneRenderers)
            {
                var mat = lr.material;  // instance
                foreach (var prop in laneColorProperties)
                    mat.SetColor(prop, randomHDR);
            }
        }
    }

    void SetGlitchValues(float noise, float strength, float scan)
    {
        glitchMaterial.SetFloat("_NoiseAmount", noise);
        glitchMaterial.SetFloat("_GlitchStrength", strength);
        glitchMaterial.SetFloat("_ScanLinesStrength", scan);
    }
}
