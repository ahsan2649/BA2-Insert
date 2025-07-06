using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereMover : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection = Vector3.left;
    public float speed = 5f;

    [Header("Explosion")]
    public GameObject explosionPrefab;

    [Header("Background Shader Settings")]
    [Tooltip("Drag your Background4 quad (with the RippleShader) here")]
    public Renderer backgroundRenderer;

    [Range(0f, 10f)]
    public float rippleStrengthOnHit = 3f;

    [Range(0f, 1f)]
    public float crackStrengthOnHit = 0.5f;

    [Tooltip("If true, edits in Inspector will update the shared asset in Edit-mode")]
    public bool liveEditInEditor = false;

    Material _bgMat;

    void Awake()
    {
        // Grab an *instance* of the material so we don’t pollute the shared asset by default
        if (backgroundRenderer != null)
            _bgMat = backgroundRenderer.material;
    }

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.linearVelocity = moveDirection.normalized * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        // 1) Spawn your explosion
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.identity);

        // 2) Apply your Inspector-driven values to the shader
        if (_bgMat != null)
        {
            _bgMat.SetFloat("_RippleStrength", rippleStrengthOnHit);
            _bgMat.SetFloat("_CrackStrength", crackStrengthOnHit);
        }

        // 3) Destroy the sphere
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // This runs whenever you tweak a value in the Inspector (in Edit-mode)
    void OnValidate()
    {
        if (!liveEditInEditor || backgroundRenderer == null) return;

        // edit the sharedMaterial so you see it in Scene View immediately
        var mat = backgroundRenderer.sharedMaterial;
        mat.SetFloat("_RippleStrength", rippleStrengthOnHit);
        mat.SetFloat("_CrackStrength", crackStrengthOnHit);
    }
#endif
}
