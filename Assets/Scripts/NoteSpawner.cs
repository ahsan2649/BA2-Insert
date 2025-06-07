using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] SplineContainer[] splineContainer;

    [SerializeField] GameObject m_NotePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnNote(0);   
        }
    }

    void SpawnNote(int index)
    {
        var note = Instantiate(m_NotePrefab, splineContainer[index].transform.position, splineContainer[index].transform.rotation);
        var splineAnimate = note.GetComponent<SplineAnimate>();
        splineAnimate.Container = splineContainer[index];
        splineAnimate.Play();
    }
}