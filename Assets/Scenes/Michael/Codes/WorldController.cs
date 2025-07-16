using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Ahsan.ScriptableObjects;
using Ahsan;

public class WorldController : MonoBehaviour
{
    // Singleton reference for easy access from spawned objects
    public static WorldController Instance { get; private set; }

    [Header("Music System Reference")]
    [Tooltip("Reference to the NewConductor to listen for world changes")]
    public NewConductor newConductor;

    [Header("World Setup - Drag & Drop")]
    [Tooltip("Drag the Anthropocene world GameObject here")]
    public GameObject anthropoceneWorld;
    [Tooltip("Drag the Post Human Biome world GameObject here")]
    public GameObject postHumanBiomeWorld;
    [Tooltip("Drag the Signal world GameObject here")]
    public GameObject signalWorld;
    [Tooltip("Drag the Chaos world GameObject here")]
    public GameObject chaosWorld;

    [Header("World Activation Toggles")]
    [Tooltip("Toggle to activate Anthropocene world")]
    public bool activateAnthropocene = false;
    [Tooltip("Toggle to activate Post Human Biome world")]
    public bool activatePostHumanBiome = false;
    [Tooltip("Toggle to activate Signal world")]
    public bool activateSignal = false;
    [Tooltip("Toggle to activate Chaos world")]
    public bool activateChaos = false;

    [Header("Testing")]
    [Tooltip("Enable automatic world switching for testing")]
    public bool enableAutoSwitching = false;
    public float switchInterval = 5f;

    [Header("Music-Driven World Changes")]
    [Tooltip("Enable automatic world switching based on music segments")]
    public bool enableMusicDrivenSwitching = true;

    [Header("Glitch Effect")]
    public Material glitchMaterial;
    public float glitchDuration = 2f;
    public float glitchNoiseAmount = 100f;
    public float glitchStrength = 30f;
    public float glitchScanLinesStrength = 0f;

    [Header("Lane Appearance")]
    [Tooltip("All your lane Renderers")]
    public Renderer[] laneRenderers;
    [Tooltip("Shader property names on your gradient shader")]
    public string[] laneColorProperties = new[] { "_Color" };

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

    // Internal state
    private GameObject[] worlds;
    private bool[] previousToggleStates = new bool[4];
    private WorldVariant currentWorldVariant = WorldVariant.Intro;
    private float defaultNoise, defaultStrength, defaultScan;
    private Color currentLaneColor = Color.white;

    void Start()
    {
        // Set singleton instance
        Instance = this;

        // Initialize worlds
        InitializeWorlds();

        // Cache default glitch values
        CacheGlitchDefaults();

        // Initialize toggle states
        InitializeToggleStates();

        // Subscribe to music events if enabled
        if (enableMusicDrivenSwitching && newConductor != null)
        {
            newConductor.OnNewSongStarted += OnMusicWorldChange;
            Debug.Log("WorldController: Subscribed to music-driven world changes");
        }

        // Start automatic switching only if enabled (for testing purposes)
        if (enableAutoSwitching)
        {
            Debug.Log("WorldController: Auto-switching is ENABLED - starting auto switch");
            StartCoroutine(AutoSwitch());
        }
        else
        {
            Debug.Log("WorldController: Toggle-based world control active");
        }
        
        SetGlitch(defaultNoise, defaultStrength, defaultScan);
        
    }

    void Update()
    {
        // Only check toggles if auto-switching and music-driven switching are both disabled
        if (!enableAutoSwitching && !enableMusicDrivenSwitching)
        {
            CheckToggleChanges();
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events to prevent memory leaks
        if (newConductor != null)
        {
            newConductor.OnNewSongStarted -= OnMusicWorldChange;
        }
    }

    #region Music Event Handling

    private void OnMusicWorldChange(Segment segment, WorldVariant worldVariant)
    {
        Debug.Log($"WorldController: Music event received - switching to {worldVariant}");

        // Update current world variant
        currentWorldVariant = worldVariant;

        // Switch based on the world variant
        switch (worldVariant)
        {
            case WorldVariant.Intro:
                // For intro, deactivate all worlds (empty state)
                DeactivateAllWorlds();
                ResetAllToggles();
                Debug.Log("WorldController: Intro - all worlds deactivated");
                break;

            case WorldVariant.Anthropocene:
                ActivateWorldByIndex(0);
                break;

            case WorldVariant.PostHumanBiome:
                ActivateWorldByIndex(1);
                break;

            case WorldVariant.Signal:
                ActivateWorldByIndex(2);
                break;

            case WorldVariant.Chaos:
                ActivateWorldByIndex(3);
                break;

            default:
                Debug.LogWarning($"WorldController: Unknown WorldVariant: {worldVariant}");
                break;
        }
    }

    #endregion

    private void InitializeWorlds()
    {
        // Create array from the dragged world references
        worlds = new GameObject[4];
        worlds[0] = anthropoceneWorld;
        worlds[1] = postHumanBiomeWorld;
        worlds[2] = signalWorld;
        worlds[3] = chaosWorld;

        // Check for missing references
        for (int i = 0; i < worlds.Length; i++)
        {
            if (worlds[i] == null)
            {
                Debug.LogError($"WorldController: {GetWorldName(i)} world GameObject is not assigned! Please drag it to the inspector.");
            }
            else
            {
                worlds[i].SetActive(false); // All worlds start deactivated
                Debug.Log($"World {i}: {worlds[i].name} ({GetWorldName(i)})");
            }
        }
    }

    private void CacheGlitchDefaults()
    {
        if (glitchMaterial != null)
        {
            defaultNoise = glitchMaterial.GetFloat("_NoiseAmount");
            defaultStrength = glitchMaterial.GetFloat("_GlitchStrength");
            defaultScan = glitchMaterial.GetFloat("_ScanLinesStrength");
            SetGlitch(defaultNoise, defaultStrength, defaultScan);
        }
    }

    private void InitializeToggleStates()
    {
        previousToggleStates[0] = activateAnthropocene;
        previousToggleStates[1] = activatePostHumanBiome;
        previousToggleStates[2] = activateSignal;
        previousToggleStates[3] = activateChaos;
    }

    private void CheckToggleChanges()
    {
        bool[] currentStates = {
            activateAnthropocene,
            activatePostHumanBiome,
            activateSignal,
            activateChaos
        };

        // Check if any toggle changed to true
        for (int i = 0; i < 4; i++)
        {
            if (currentStates[i] && !previousToggleStates[i])
            {
                // This toggle was just activated
                ActivateWorldByIndex(i);
                break; // Only activate one world at a time
            }
        }

        // Update previous states
        System.Array.Copy(currentStates, previousToggleStates, 4);
    }

    private void ActivateWorldByIndex(int worldIndex)
    {
        if (worlds == null || worldIndex < 0 || worldIndex >= worlds.Length)
        {
            Debug.LogError($"WorldController: Invalid world index {worldIndex}");
            return;
        }

        // Deactivate all worlds first
        DeactivateAllWorlds();

        // Reset all toggles except the one being activated
        ResetTogglesExcept(worldIndex);

        // Activate the selected world
        worlds[worldIndex].SetActive(true);

        // Update current world variant
        currentWorldVariant = IndexToWorldVariant(worldIndex);

        // Apply visual effects
        ApplyColorTheme();

        // Trigger glitch effect
        StartCoroutine(GlitchEffect());

        Debug.Log($"WorldController: Activated {GetWorldName(worldIndex)} (index {worldIndex}) - {currentWorldVariant}");
    }

    private void DeactivateAllWorlds()
    {
        if (worlds == null) return;

        for (int i = 0; i < worlds.Length; i++)
        {
            if (worlds[i] != null)
            {
                worlds[i].SetActive(false);
            }
        }
    }

    private void ResetTogglesExcept(int activeIndex)
    {
        activateAnthropocene = (activeIndex == 0);
        activatePostHumanBiome = (activeIndex == 1);
        activateSignal = (activeIndex == 2);
        activateChaos = (activeIndex == 3);

        // Update previous states to match current
        InitializeToggleStates();
    }

    private void ResetAllToggles()
    {
        activateAnthropocene = false;
        activatePostHumanBiome = false;
        activateSignal = false;
        activateChaos = false;

        // Update previous states to match current
        InitializeToggleStates();
    }

    private string GetWorldName(int index)
    {
        return index switch
        {
            0 => "Anthropocene",
            1 => "Post Human Biome",
            2 => "Signal",
            3 => "Chaos",
            _ => "Unknown"
        };
    }

    private WorldVariant IndexToWorldVariant(int index)
    {
        return index switch
        {
            0 => WorldVariant.Anthropocene,
            1 => WorldVariant.PostHumanBiome,
            2 => WorldVariant.Signal,
            3 => WorldVariant.Chaos,
            _ => WorldVariant.Intro
        };
    }

    #region Color Theme (Shared Elements)

    private void ApplyColorTheme()
    {
        // Generate a unified color theme for elements that span across worlds
        Color themeColor = Color.HSVToRGB(
            Random.value,
            1f,
            Random.Range(1f, 5f),
            true
        );

        currentLaneColor = themeColor;

        // Apply to lanes
        ApplyLaneColors(themeColor);

        // Apply to particle effects
        ApplyParticleEffects();

        // Apply to orb effects
        ApplyOrbColors();

        Debug.Log($"Applied color theme: {themeColor} for world: {currentWorldVariant}");
    }

    private void ApplyLaneColors(Color laneColor)
    {
        if (laneRenderers != null)
        {
            foreach (var laneRenderer in laneRenderers)
            {
                if (laneRenderer != null && laneRenderer.material != null)
                {
                    var material = laneRenderer.material;
                    foreach (var property in laneColorProperties)
                    {
                        if (material.HasProperty(property))
                        {
                            material.SetColor(property, laneColor);
                        }
                    }
                }
            }
        }
    }

    private void ApplyParticleEffects()
    {
        if (bubbleMaterial != null && bubbleMaterial.HasProperty(fresnelColorProperty))
        {
            Color fresnelColor = currentLaneColor * particlesMembraneIntensity;
            bubbleMaterial.SetColor(fresnelColorProperty, fresnelColor);
        }
    }

    private void ApplyOrbColors()
    {
        if (orbVisualEffect == null || orbColorProperties == null) return;

        for (int i = 0; i < orbColorProperties.Length; i++)
        {
            if (orbVisualEffect.HasVector4(orbColorProperties[i]))
            {
                float intensity = i < orbColorIntensities.Length ? orbColorIntensities[i] : 1.0f;
                Color orbColor = currentLaneColor * intensity;
                orbVisualEffect.SetVector4(orbColorProperties[i], orbColor);
            }
        }
    }

    #endregion

    #region Effects

    private IEnumerator GlitchEffect()
    {
        if (glitchMaterial)
        {
            SetGlitch(glitchNoiseAmount, glitchStrength, glitchScanLinesStrength);
            yield return new WaitForSeconds(glitchDuration);
            SetGlitch(defaultNoise, defaultStrength, defaultScan);
        }
    }

    private void SetGlitch(float noise, float strength, float scanLines)
    {
        if (glitchMaterial != null)
        {
            glitchMaterial.SetFloat("_NoiseAmount", noise);
            glitchMaterial.SetFloat("_GlitchStrength", strength);
            glitchMaterial.SetFloat("_ScanLinesStrength", scanLines);
        }
    }

    #endregion

    #region Auto-Switching (Testing)

    private IEnumerator AutoSwitch()
    {
        Debug.Log("Starting auto-switch mode for testing");

        int currentIndex = 0;

        while (true)
        {
            // Activate current world
            ActivateWorldByIndex(currentIndex);

            yield return new WaitForSeconds(switchInterval);

            // Move to next world (cycle through 0-3)
            currentIndex = (currentIndex + 1) % 4;
        }
    }

    #endregion

    #region Public Interface

    [ContextMenu("Activate Anthropocene World")]
    public void ActivateAnthropoceneWorld()
    {
        if (anthropoceneWorld == null)
        {
            Debug.LogError("WorldController: Anthropocene world is not assigned!");
            return;
        }
        activateAnthropocene = true;
        ActivateWorldByIndex(0);
    }

    [ContextMenu("Activate Post Human Biome World")]
    public void ActivatePostHumanBiomeWorld()
    {
        if (postHumanBiomeWorld == null)
        {
            Debug.LogError("WorldController: Post Human Biome world is not assigned!");
            return;
        }
        activatePostHumanBiome = true;
        ActivateWorldByIndex(1);
    }

    [ContextMenu("Activate Signal World")]
    public void ActivateSignalWorld()
    {
        if (signalWorld == null)
        {
            Debug.LogError("WorldController: Signal world is not assigned!");
            return;
        }
        activateSignal = true;
        ActivateWorldByIndex(2);
    }

    [ContextMenu("Activate Chaos World")]
    public void ActivateChaosWorld()
    {
        if (chaosWorld == null)
        {
            Debug.LogError("WorldController: Chaos world is not assigned!");
            return;
        }
        activateChaos = true;
        ActivateWorldByIndex(3);
    }

    [ContextMenu("Deactivate All Worlds")]
    public void DeactivateAllWorldsPublic()
    {
        DeactivateAllWorlds();
        ResetAllToggles();
        currentWorldVariant = WorldVariant.Intro;
        Debug.Log("All worlds deactivated");
    }

    public void ApplyCurrentColorToParticle(GameObject particleObject)
    {
        ApplyParticleEffects();
    }

    public void UpdateOrbColors()
    {
        ApplyOrbColors();
    }

    public WorldVariant GetCurrentWorldVariant()
    {
        return currentWorldVariant;
    }

    public Color GetCurrentLaneColor()
    {
        return currentLaneColor;
    }

    // Helper method to get world GameObject by variant
    public GameObject GetWorldGameObject(WorldVariant variant)
    {
        return variant switch
        {
            WorldVariant.Anthropocene => anthropoceneWorld,
            WorldVariant.PostHumanBiome => postHumanBiomeWorld,
            WorldVariant.Signal => signalWorld,
            WorldVariant.Chaos => chaosWorld,
            _ => null
        };
    }

    #endregion
}