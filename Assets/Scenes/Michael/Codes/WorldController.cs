using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Ahsan.ScriptableObjects; // Add this for WorldVariant enum
using Ahsan; // Add this for NewConductor

public class WorldController : MonoBehaviour
{
    // Singleton reference for easy access from spawned objects
    public static WorldController Instance { get; private set; }

    [Header("Conductor Integration")]
    [SerializeField] private Ahsan.NewConductor newConductor;

    [Header("World Setup")]
    public Transform worldsParent;
    [Tooltip("Enable automatic world switching for testing (disable for event-driven switching)")]
    public bool enableAutoSwitching = false;

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
    public string[] laneColorProperties = new[] { "_Color" };

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

    [Header("World4 Crack Background")]
    [Tooltip("Reference to World4Controller to sync crack background color")]
    public World4Controller world4Controller;
    [Tooltip("Shader property name for crack texture background color")]
    public string crackBackgroundProperty = "_CrackColor";
    [Tooltip("Fixed HDR intensity for crack background (independent of other colors)")]
    [Range(1f, 5f)]
    public float crackBackgroundIntensity = 2f;

    [Header("Particles Membrane Appearance")]
    [Tooltip("The Bubble material used by ParticlesMembrane objects")]
    public Material bubbleMaterial;
    [Tooltip("Shader property name for Fresnel color")]
    public string fresnelColorProperty = "_FresnelColor";
    [Tooltip("Fixed HDR intensity for particles membrane fresnel color")]
    [Range(1f, 5f)]
    public float particlesMembraneIntensity = 3f;

    [Header("Orb Visual Effect")]
    [Tooltip("The Orb Visual Effect to sync colors with")]
    public VisualEffect orbVisualEffect;
    [Tooltip("VFX property names for colors - add all color properties you want to sync")]
    public string[] orbColorProperties = new[] { "BrightFlareColor", "DarkFlareGradient", "ColorParticle", "FrontColor", "BackColor" };
    [Tooltip("Intensity multipliers for each orb color property (same order as orbColorProperties)")]
    public float[] orbColorIntensities = new[] { 1.5f, 0.8f, 1.0f, 1.3f, 0.6f };

    // internals
    GameObject[] worlds;
    int currentIndex = 0;
    float defaultNoise,
            defaultStrength,
            defaultScan;

    // Store current lane color for dynamically spawned particles
    private Color currentLaneColor = Color.white;

    void Start()
    {
        // Set singleton instance
        Instance = this;

        // gather worlds and deactivate all initially (intro should have no active worlds)
        int n = worldsParent.childCount;
        worlds = new GameObject[n];
        for (int i = 0; i < n; i++)
        {
            worlds[i] = worldsParent.GetChild(i).gameObject;
            worlds[i].SetActive(false); // All worlds start deactivated during intro
        }

        // cache default glitch values & reset
        defaultNoise = glitchMaterial.GetFloat("_NoiseAmount");
        defaultStrength = glitchMaterial.GetFloat("_GlitchStrength");
        defaultScan = glitchMaterial.GetFloat("_ScanLinesStrength");
        SetGlitch(defaultNoise, defaultStrength, defaultScan);

        // Don't apply any appearance during intro - wait for event-driven switching

        // start automatic switching only if enabled (for testing purposes)
        if (enableAutoSwitching)
        {
            StartCoroutine(AutoSwitch());
        }
    }

    private void OnEnable()
    {
        // Subscribe to the conductor's event
        if (newConductor != null)
        {
            newConductor.OnNewSongStarted += ApplyAppearanceOnSongStart;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the conductor's event
        if (newConductor != null)
        {
            newConductor.OnNewSongStarted -= ApplyAppearanceOnSongStart;
        }
    }

    // Event handler that matches the conductor's event signature
    private void ApplyAppearanceOnSongStart(Segment segment, WorldVariant worldVariant)
    {
        ApplyAppearance(worldVariant);

        // Activate the corresponding world GameObject
        ActivateWorld(worldVariant);

        // Trigger glitch effect
        StartCoroutine(GlitchEffect());
    }

    private void ActivateWorld(WorldVariant worldVariant)
    {
        // Get the index for the new world
        int newIndex = WorldVariantToIndex(worldVariant);

        // If it's intro or invalid, deactivate all worlds
        if (newIndex < 0 || newIndex >= worlds.Length)
        {
            for (int i = 0; i < worlds.Length; i++)
            {
                worlds[i].SetActive(false);
            }
            currentIndex = -1; // No world active
            return;
        }

        // Deactivate current world if one is active
        if (currentIndex >= 0 && currentIndex < worlds.Length)
        {
            worlds[currentIndex].SetActive(false);
        }

        // Activate new world
        currentIndex = newIndex;
        worlds[currentIndex].SetActive(true);
    }

    private int WorldVariantToIndex(WorldVariant worldVariant)
    {
        return worldVariant switch
        {
            WorldVariant.Intro => 0,
            WorldVariant.Anthropocene => 1,
            WorldVariant.PostHumanBiome => 2,
            WorldVariant.Signal => 3,
            WorldVariant.Chaos => 4,
            _ => 0
        };
    }

    private IEnumerator GlitchEffect()
    {
        SetGlitch(glitchNoiseAmount, glitchStrength, glitchScanLinesStrength);
        yield return new WaitForSeconds(glitchDuration);
        SetGlitch(defaultNoise, defaultStrength, defaultScan);
    }

    IEnumerator AutoSwitch()
    {
        // Start with first world (Anthropocene) if auto-switching is enabled
        currentIndex = 0;
        worlds[currentIndex].SetActive(true);
        ApplyAppearance(WorldVariant.Anthropocene);

        while (true)
        {
            yield return new WaitForSeconds(switchInterval);

            // swap worlds (cycle through the 4 actual worlds, not intro)
            worlds[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % 4; // Only cycle through 4 worlds
            worlds[currentIndex].SetActive(true);

            // Convert index back to WorldVariant for the refactored method
            WorldVariant variant = IndexToWorldVariant(currentIndex);
            ApplyAppearance(variant);

            // glitch flash
            SetGlitch(glitchNoiseAmount, glitchStrength, glitchScanLinesStrength);
            yield return new WaitForSeconds(glitchDuration);
            SetGlitch(defaultNoise, defaultStrength, defaultScan);
        }
    }

    private WorldVariant IndexToWorldVariant(int index)
    {
        return index switch
        {
            0 => WorldVariant.Intro,
            1 => WorldVariant.Anthropocene,
            2 => WorldVariant.PostHumanBiome,
            3 => WorldVariant.Signal,
            4 => WorldVariant.Chaos,
            _ => WorldVariant.Intro
        };
    }

    void ApplyAppearance(WorldVariant worldVariant)
    {
        // Convert WorldVariant to index for array access
        int idx = WorldVariantToIndex(worldVariant);

        // 1) pick a base HDR color for lanes
        Color laneHDR = Color.HSVToRGB(
            Random.value,
            1f,
            Random.Range(1f, 5f),
            true
        );

        // Store current lane color for dynamically spawned particles
        currentLaneColor = laneHDR;

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

        // 5) if this is Signal (World 3), tint the circuit & flow as before
        if (worldVariant == WorldVariant.Signal && circuitRenderers != null)
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

        // 6) if this is Chaos (World 4), update crack background to match membrane color
        if (worldVariant == WorldVariant.Chaos && world4Controller != null && world4Controller.backgroundRenderer != null)
        {
            var material = world4Controller.backgroundRenderer.material;
            if (material.HasProperty(crackBackgroundProperty))
            {
                // Create bright, saturated crack background with fixed HDR intensity
                // Position closer to top-left (higher saturation, higher brightness)
                Color brightCrackBackground = Color.HSVToRGB(h, 0.9f, crackBackgroundIntensity, true);

                material.SetColor(crackBackgroundProperty, brightCrackBackground);

                // Update the World4Controller's crackBackgroundColor field so it shows in inspector
                world4Controller.crackBackgroundColor = brightCrackBackground;
            }
        }

        // 7) Update particles membrane fresnel color to match current lane color
        if (bubbleMaterial != null && bubbleMaterial.HasProperty(fresnelColorProperty))
        {
            Color fresnelColor = currentLaneColor * particlesMembraneIntensity;
            bubbleMaterial.SetColor(fresnelColorProperty, fresnelColor);
        }

        // 8) Update Orb Visual Effect colors to match current lane color
        ApplyOrbColors();
    }

    void ApplyOrbColors()
    {
        if (orbVisualEffect == null || orbColorProperties == null) return;

        for (int i = 0; i < orbColorProperties.Length; i++)
        {
            if (orbVisualEffect.HasVector4(orbColorProperties[i]))
            {
                // Get intensity multiplier for this property (default to 1.0 if not enough intensities specified)
                float intensity = i < orbColorIntensities.Length ? orbColorIntensities[i] : 1.0f;

                // Apply the current lane color with the specified intensity
                Color orbColor = currentLaneColor * intensity;
                orbVisualEffect.SetVector4(orbColorProperties[i], orbColor);
            }
        }
    }

    void SetGlitch(float noise, float str, float scan)
    {
        glitchMaterial.SetFloat("_NoiseAmount", noise);
        glitchMaterial.SetFloat("_GlitchStrength", str);
        glitchMaterial.SetFloat("_ScanLinesStrength", scan);
    }

    // Public method to apply current lane color to newly spawned ParticlesMembrane objects
    public void ApplyCurrentColorToParticle(GameObject particleObject)
    {
        // Update the shared Bubble material's Fresnel color with current lane color
        if (bubbleMaterial != null && bubbleMaterial.HasProperty(fresnelColorProperty))
        {
            // Apply the current lane color with the specified HDR intensity for particles membrane
            Color fresnelColor = currentLaneColor * particlesMembraneIntensity;
            bubbleMaterial.SetColor(fresnelColorProperty, fresnelColor);
        }
    }

    // Public method to manually update orb colors (useful for testing or external calls)
    public void UpdateOrbColors()
    {
        ApplyOrbColors();
    }

    // Public method to manually apply appearance with WorldVariant (useful for testing)
    public void ManualApplyAppearance(WorldVariant worldVariant)
    {
        ApplyAppearance(worldVariant);
        ActivateWorld(worldVariant);
    }
}