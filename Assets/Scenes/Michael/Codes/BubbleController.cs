using System.Collections;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    [HideInInspector] public World4Controller world4Controller;

    [Header("Animation Settings")]
    public float growDuration = 0.5f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;

    [Header("Lifetime Settings")]
    public float lifeTimeMin = 5f;
    public float lifeTimeMax = 10f;

    [Header("Scale Settings")]
    [Tooltip("Final bubble scale in world‐units will be chosen randomly between these")]
    public float minScale = 20f;
    public float maxScale = 40f;

    Vector3 _basePosition;
    float _targetScale;

    void Start()
    {
        // pick a random size for this bubble
        _targetScale = Random.Range(minScale, maxScale);

        // start invisible at 0, record position
        transform.localScale = Vector3.zero;
        _basePosition = transform.position;

        // kick off coroutines
        StartCoroutine(Grow());
        StartCoroutine(LifeCycle());
    }

    IEnumerator Grow()
    {
        float t = 0f;
        while (t < growDuration)
        {
            // s goes 0→1
            float s = Mathf.Lerp(0f, 1f, t / growDuration);
            // scale bubble proportionally up to targetScale
            transform.localScale = Vector3.one * (s * _targetScale);
            t += Time.deltaTime;
            yield return null;
        }
        // ensure exact final size
        transform.localScale = Vector3.one * _targetScale;
    }

    IEnumerator LifeCycle()
    {
        // live for random time...
        float life = Random.Range(lifeTimeMin, lifeTimeMax);
        yield return new WaitForSeconds(life);

        // notify controller and destroy
        world4Controller?.NotifyBubbleDestroyed(gameObject);
        Destroy(gameObject);
    }

    void Update()
    {
        // simple up/down bob around original position
        Vector3 p = _basePosition;
        p.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = p;
    }
}
