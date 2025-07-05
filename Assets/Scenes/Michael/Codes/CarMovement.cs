using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CarMovement : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float speed = 3f;

    private Vector2 _startPos;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        // Record spawn position for looping
        _startPos = _rb.position;

        // Make sure we can hit triggers and have no gravity
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0f;
    }

    void Update()
    {
        // Determine direction: flipX==true means sprite faces left
        float dirSign = _sr.flipX ? -1f : +1f;

        // Move in X
        Vector2 delta = Vector2.right * dirSign * speed * Time.deltaTime;
        _rb.MovePosition(_rb.position + delta);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Respawn"))
        {
            // Snap back to start
            _rb.position = _startPos;
        }
    }
}
