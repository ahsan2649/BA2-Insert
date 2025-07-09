using System.Collections;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    [Header("World Setup")]
    [Tooltip("Parent of World1…WorldN")]
    public Transform worldsParent;

    [Header("Timing")]
    [Tooltip("Seconds between automatic world changes.")]
    public float switchInterval = 10f;
    [Tooltip("Seconds to hold the glitch values.")]
    public float glitchDuration = 2f;

    [Header("Glitch Material (Full-Screen)")]
    [Tooltip("Your GlitchMat asset with the three floats (_NoiseAmount, _GlitchStrength, _ScanLinesStrength).")]
    public Material glitchMaterial;
    [Tooltip("NoiseAmount while glitching.")]
    public float glitchNoiseAmount = 100f;
    [Tooltip("GlitchStrength while glitching.")]
    public float glitchStrength = 30f;
    [Tooltip("ScanLinesStrength while glitching.")]
    public float glitchScanLinesStrength = 0f;

    [Header("Membrane Textures")]
    [Tooltip("Main textures, one per world, in the same order as your Worlds.")]
    public Texture[] membraneMainTextures;

    [Tooltip("Left & Right membrane renderers (each must have Gradient mat at index 0, RippleMat at index 1).")]
    public Renderer[] membraneRenderers;

    [Tooltip("Which material slot on your renderer is the RippleMat?")]
    public int rippleMaterialIndex = 1;

    [Tooltip("Property name for the Ripple main texture (Reference from your Shader Graph).")]
    public string mainTextureProperty = "_MainTex";

    // internals
    GameObject[] worlds;
    int currentIndex = 0;
    float defaultNoise, defaultStrength, defaultScan;

    void Start()
    {
        // 1) cache world children & only show the first
        int count = worldsParent.childCount;
        worlds = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            var w = worldsParent.GetChild(i).gameObject;
            worlds[i] = w;
            w.SetActive(i == 0);
        }

        // 2) record your GlitchMat defaults
        if (glitchMaterial == null)
        {
            Debug.LogError("WorldController: assign GlitchMat in the inspector!");
            enabled = false;
            return;
        }
        defaultNoise = glitchMaterial.GetFloat("_NoiseAmount");
        defaultStrength = glitchMaterial.GetFloat("_GlitchStrength");
        defaultScan = glitchMaterial.GetFloat("_ScanLinesStrength");

        // 2a) force material into non-glitch state on game start
        glitchMaterial.SetFloat("_NoiseAmount", defaultNoise);
        glitchMaterial.SetFloat("_GlitchStrength", defaultStrength);
        glitchMaterial.SetFloat("_ScanLinesStrength", defaultScan);

        // 3) start the auto‐loop
        StartCoroutine(AutoSwitchRoutine());
    }

    IEnumerator AutoSwitchRoutine()
    {
        while (true)
        {
            // wait
            yield return new WaitForSeconds(switchInterval);

            // swap worlds
            worlds[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % worlds.Length;
            worlds[currentIndex].SetActive(true);

            // -----------------------
            //  swap membrane main-tex
            // -----------------------
            if (membraneMainTextures != null
             && currentIndex < membraneMainTextures.Length
             && membraneRenderers != null)
            {
                var newMain = membraneMainTextures[currentIndex];
                foreach (var rend in membraneRenderers)
                {
                    var mats = rend.materials;            // get a copy of the array
                    if (rippleMaterialIndex < mats.Length)
                    {
                        mats[rippleMaterialIndex]
                            .SetTexture(mainTextureProperty, newMain);
                        rend.materials = mats;           // assign back to update only that slot
                    }
                }
            }

            // -------------
            //  do glitch-on
            // -------------
            glitchMaterial.SetFloat("_NoiseAmount", glitchNoiseAmount);
            glitchMaterial.SetFloat("_GlitchStrength", glitchStrength);
            glitchMaterial.SetFloat("_ScanLinesStrength", glitchScanLinesStrength);

            yield return new WaitForSeconds(glitchDuration);

            // ------------------
            //  restore glitch-off
            // ------------------
            glitchMaterial.SetFloat("_NoiseAmount", defaultNoise);
            glitchMaterial.SetFloat("_GlitchStrength", defaultStrength);
            glitchMaterial.SetFloat("_ScanLinesStrength", defaultScan);
        }
    }
}
