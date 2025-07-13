using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World4Controller : MonoBehaviour
{
    // === REFERENCES ===
    [Header("References")]
    [Tooltip("The quad/plane with your RippleShader material")]
    public Renderer backgroundRenderer;

    // === RIPPLE SETTINGS ===
    [Header("Ripple Settings")]
    public Vector2 rippleCenter = new Vector2(0, 1);
    public int rippleCount = 1;
    public float rippleSpeed = 5f;
    public float rippleStrength = 2f;
    public Color tintColor = Color.white;

    // === CRACK SETTINGS ===
    [Header("Crack Settings")]
    public Texture crackTexture;
    [Range(0f, 5f)]
    public float crackStrength = 0.8f;

    // <— tell Unity this is HDR so the picker shows an intensity slider
    [ColorUsage(true, true)]
    public Color crackColor = Color.white;

    [ColorUsage(true, true)]
    [Tooltip("HDR color for crack texture background - will be overridden by WorldController")]
    public Color crackBackgroundColor = Color.black;

    [Range(0f, 5f)]
    public float threshold = 0f;

    [Tooltip("If true, updates the shared material as you tweak in Edit mode")]
    public bool liveEditInEditor = false;

    // === CRACK TEXTURE PARAMETERS ===
    [Header("Crack Texture Params")]
    public Vector2 crackTextureTiling = Vector2.one;
    public Vector2 crackTextureOffset = Vector2.zero;

    // === COLLISION OVERRIDES ===
    [Header("Collision Crack Settings")]
    public float collisionCrackStrength = 3f;
    public float collisionCrackColorIntensity = 3f;
    public float crackResetDelay = 1f;

    // === BUBBLE SYSTEM ===
    [Header("Bubble Settings")]
    public GameObject bubblePrefab;
    public int maxBubbles = 10;

    // === PRIVATE STATE ===
    private Material _instancedMat;
    private bool _bubblesActivated;
    private List<GameObject> _activeBubbles = new List<GameObject>();

    void Awake()
    {
        if (backgroundRenderer == null) return;
        // use .material so we don't clobber the shared asset
        _instancedMat = backgroundRenderer.material;

        // apply baseline crack‐texture UVs
        _instancedMat.SetTextureScale("_CrackTexture", crackTextureTiling);
        _instancedMat.SetTextureOffset("_CrackTexture", crackTextureOffset);

        // set initial crack background color
        if (_instancedMat.HasProperty("_CrackColor"))
        {
            _instancedMat.SetColor("_CrackColor", crackBackgroundColor);
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!liveEditInEditor || backgroundRenderer == null) return;
        var shared = backgroundRenderer.sharedMaterial;

        // ripple
        shared.SetVector("_RippleCenter", new Vector4(rippleCenter.x, rippleCenter.y, 0, 0));
        shared.SetInt("_RippleCount", rippleCount);
        shared.SetFloat("_RippleSpeed", rippleSpeed);
        shared.SetFloat("_RippleStrength", rippleStrength);
        shared.SetColor("_TintColor", tintColor);

        // crack
        if (crackTexture != null) shared.SetTexture("_CrackTexture", crackTexture);
        shared.SetFloat("_CrackStrength", crackStrength);
        shared.SetColor("_CrackColor", crackColor);
        if (shared.HasProperty("_CrackColor"))
        {
            shared.SetColor("_CrackBackgroundColor", crackBackgroundColor);
        }
        shared.SetFloat("_Threshold", threshold);

        // crack‐texture UVs
        shared.SetTextureScale("_CrackTexture", crackTextureTiling);
        shared.SetTextureOffset("_CrackTexture", crackTextureOffset);
    }
#endif

    void OnDrawGizmosSelected()
    {
        if (backgroundRenderer == null) return;
        var b = backgroundRenderer.bounds;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(b.center, b.size);
    }

    public void ActivateBubbles()
    {
        if (bubblePrefab == null)
        {
            Debug.LogWarning("World4Controller: bubblePrefab not assigned.");
            return;
        }
        if (_bubblesActivated) return;
        _bubblesActivated = true;
        for (int i = 0; i < maxBubbles; i++) SpawnBubble();
    }

    public void TriggerHitEffect(Vector3 worldHitPos)
    {
        if (_instancedMat == null) return;

        // 1) ripple origin
        var vp = Camera.main.WorldToViewportPoint(worldHitPos);
        _instancedMat.SetVector("_RippleCenter", new Vector4(vp.x, vp.y, 0, 0));
        _instancedMat.SetInt("_RippleCount", rippleCount);
        _instancedMat.SetFloat("_RippleSpeed", rippleSpeed);
        _instancedMat.SetFloat("_RippleStrength", rippleStrength);
        _instancedMat.SetColor("_TintColor", tintColor);

        // 2) crack
        if (crackTexture != null)
            _instancedMat.SetTexture("_CrackTexture", crackTexture);

        // reapply your UVs (just to be sure)
        _instancedMat.SetTextureScale("_CrackTexture", crackTextureTiling);
        _instancedMat.SetTextureOffset("_CrackTexture", crackTextureOffset);

        // spike strength + HDR color
        _instancedMat.SetFloat("_CrackStrength", collisionCrackStrength);
        _instancedMat.SetColor("_CrackColor", crackColor * collisionCrackColorIntensity);
        _instancedMat.SetFloat("_Threshold", threshold);

        // preserve current crack background color during hit effect
        if (_instancedMat.HasProperty("_CrackColor"))
        {
            _instancedMat.SetColor("_CrackColor", crackBackgroundColor);
        }

        // schedule reset
        StopCoroutine(nameof(ResetCrackEffect));
        StartCoroutine(ResetCrackEffect());
    }

    public void NotifyBubbleDestroyed(GameObject bubble)
    {
        _activeBubbles.Remove(bubble);
        SpawnBubble();
    }

    private IEnumerator ResetCrackEffect()
    {
        yield return new WaitForSeconds(crackResetDelay);
        // restore baseline
        _instancedMat.SetFloat("_CrackStrength", crackStrength);
        _instancedMat.SetColor("_CrackColor", crackColor);
        _instancedMat.SetTextureScale("_CrackTexture", crackTextureTiling);
        _instancedMat.SetTextureOffset("_CrackTexture", crackTextureOffset);

        // restore crack background color
        if (_instancedMat.HasProperty("_CrackColor"))
        {
            _instancedMat.SetColor("_CrackColor", crackBackgroundColor);
        }
    }

    private void SpawnBubble()
    {
        if (!_bubblesActivated || _activeBubbles.Count >= maxBubbles) return;
        if (backgroundRenderer == null) return;

        var b = backgroundRenderer.bounds;
        var pos = new Vector3(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y),
            b.center.z + 0.1f
        );
        var bub = Instantiate(bubblePrefab, pos, Quaternion.identity, transform);
        if (bub.TryGetComponent<BubbleController>(out var bc))
            bc.world4Controller = this;
        _activeBubbles.Add(bub);
    }
}