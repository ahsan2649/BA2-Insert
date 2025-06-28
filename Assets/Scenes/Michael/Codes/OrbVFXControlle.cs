using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class OrbVFXControlle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect orbVFX;      // drag in your Orb’s VisualEffect

    [Header("Dark Flare")]
    [SerializeField] private float darkFlareSize = 5f;
    [SerializeField] private Gradient darkFlareGradient;
    [SerializeField] private Texture darkFlareTex;

    [Header("Bright Flare")]
    [SerializeField] private float brightFlareSize = 2f;
    [SerializeField] private Color brightFlareColor = Color.white;
    [SerializeField] private Texture brightFlareTex;

    [Header("Floating Particle")]
    [SerializeField] private float bigSizeParticle = 0.5f;
    [SerializeField] private Vector2 turbulenceMax = new Vector2(3, 3);
    [SerializeField] private Color colorParticle = Color.cyan;
    [SerializeField] private float smallSizeParticle = 0.1f;
    [SerializeField] private Vector2 turbulenceMin = new Vector2(4, 4);
    [SerializeField] private Texture particleTex;

    [Header("Orb Settings")]
    [SerializeField] private float duration = 30f;
    [SerializeField] private float clip = 0.02f;
    [SerializeField] private float texSpeedX = 0.85f;
    [SerializeField] private float texSpeedY = 0.01f;
    [SerializeField] private float texStrength = 1f;
    [SerializeField] private float size = 2f;
    [SerializeField] private Texture orbTexture;
    [SerializeField] private Color frontColor = Color.magenta;
    [SerializeField] private Color backColor = Color.blue;
    [SerializeField] private Mesh mesh;

    void Start()
    {
        UpdateAllParameters();
    }

    // Call this whenever you tweak values at runtime
    public void UpdateAllParameters()
    {
        // Dark Flare
        orbVFX.SetFloat("DarkFlareSize", darkFlareSize);
        orbVFX.SetGradient("DarkFlareGradient", darkFlareGradient);
        orbVFX.SetTexture("DarkFlareTex", darkFlareTex);

        // Bright Flare
        orbVFX.SetFloat("BrightFlareSize", brightFlareSize);
        orbVFX.SetVector4("BrightFlareColor", brightFlareColor);
        orbVFX.SetTexture("BrightFlareTex", brightFlareTex);

        // Floating Particle
        orbVFX.SetFloat("BigSizeParticle", bigSizeParticle);
        orbVFX.SetVector2("TurbulenceMax", turbulenceMax);
        orbVFX.SetVector4("ColorParticle", colorParticle);
        orbVFX.SetFloat("SmallSizeParticle", smallSizeParticle);
        orbVFX.SetVector2("TurbulenceMin", turbulenceMin);
        orbVFX.SetTexture("ParticleTex", particleTex);

        // Orb core
        orbVFX.SetFloat("Duration", duration);
        orbVFX.SetFloat("Clip", clip);
        orbVFX.SetFloat("TexSpeedX", texSpeedX);
        orbVFX.SetFloat("TexSpeedY", texSpeedY);
        orbVFX.SetFloat("TexStrength", texStrength);
        orbVFX.SetFloat("Size", size);
        orbVFX.SetTexture("Texture", orbTexture);
        orbVFX.SetVector4("FrontColor", frontColor);
        orbVFX.SetVector4("BackColor", backColor);
        orbVFX.SetMesh("Mesh", mesh);
    }
}
