using UnityEngine;

[ExecuteAlways]
public class SkyBackgroundController : MonoBehaviour
{
    [Header("Assign your SkyBackground material here")]
    [SerializeField] private Material skyBackgroundMat;

    [Header("Gradient Colors")]
    [SerializeField] private Color color01 = new Color(0.11f, 0.53f, 1f);
    [SerializeField, Range(0, 1)] private float color01Influence = 0.16f;
    [SerializeField] private Color color02 = new Color(1f, 0.45f, 0.2f);
    [SerializeField, Range(0, 1)] private float color02Influence = 0.25f;
    [SerializeField] private Color color03 = new Color(0.7f, 0.95f, 1f);

    // Cache IDs for speed
    private static readonly int
        _Color01ID = Shader.PropertyToID("Color01"),
        _Color01InfluenceID = Shader.PropertyToID("Color01Influence"),
        _Color02ID = Shader.PropertyToID("Color02"),
        _Color02InfluenceID = Shader.PropertyToID("Color02Influence"),
        _Color03ID = Shader.PropertyToID("Color03");

    private void OnEnable()
    {
        UpdateShader();
    }

    private void OnValidate()
    {
        // So changes in the inspector update in Edit mode
        UpdateShader();
    }

    /// <summary>
    /// Push all current field values into the material
    /// </summary>
    public void UpdateShader()
    {
        if (skyBackgroundMat == null) return;

        skyBackgroundMat.SetColor(_Color01ID, color01);
        skyBackgroundMat.SetFloat(_Color01InfluenceID, color01Influence);

        skyBackgroundMat.SetColor(_Color02ID, color02);
        skyBackgroundMat.SetFloat(_Color02InfluenceID, color02Influence);

        skyBackgroundMat.SetColor(_Color03ID, color03);
    }
}
