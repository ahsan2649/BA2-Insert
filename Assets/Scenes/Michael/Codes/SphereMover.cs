using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereMover : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.left;
    public float speed = 5f;
    public GameObject explosionPrefab;

    [Tooltip("Drag the World4 GameObject here")]
    public World4Controller world4Controller;

    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.freezeRotation = true;
    }

    void Start()
    {
        _rb.linearVelocity = moveDirection.normalized * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        // spawn explosion
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.identity);

        // notify World4Controller
        if (world4Controller != null)
            world4Controller.TriggerHitEffect(col.contacts[0].point);

        Destroy(gameObject);
    }
}
