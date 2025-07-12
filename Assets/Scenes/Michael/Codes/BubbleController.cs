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

    Vector3 _basePosition;

    void Start()
    {
        transform.localScale = Vector3.zero;
        _basePosition = transform.position;
        StartCoroutine(Grow());
        StartCoroutine(LifeCycle());
    }

    IEnumerator Grow()
    {
        float t = 0f;
        while (t < growDuration)
        {
            float s = Mathf.Lerp(0f, 1f, t / growDuration);
            transform.localScale = Vector3.one * s;
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    IEnumerator LifeCycle()
    {
        float life = Random.Range(lifeTimeMin, lifeTimeMax);
        yield return new WaitForSeconds(life);

        // tell World4Controller we popped
        world4Controller?.NotifyBubbleDestroyed(gameObject);

        Destroy(gameObject);
    }

    void Update()
    {
        Vector3 p = _basePosition;
        p.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = p;
    }
}
