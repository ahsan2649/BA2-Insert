using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereMover : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.left;
    public float speed = 5f;
    public GameObject explosionPrefab;

    // no longer set in the Inspector
    private World4Controller _world4Controller;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.freezeRotation = true;

        // look upward in the hierarchy for a World4Controller
        _world4Controller = GetComponentInParent<World4Controller>();
    }

    void Start()
    {
        _rb.linearVelocity = moveDirection.normalized * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        // 1) only if this sphere is under a World4Controller...
        if (_world4Controller != null &&
            col.gameObject.TryGetComponent<SphereMover>(out _))
        {
            _world4Controller.ActivateBubbles();
        }

        // 2) always show your explosion & ripple/crack
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.identity);

        _world4Controller?.TriggerHitEffect(col.contacts[0].point);

        Destroy(gameObject);
    }
}
