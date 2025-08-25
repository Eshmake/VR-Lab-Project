using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RockSample : MonoBehaviour
{
    [Header("Drying State")]
    public bool IsDry { get; private set; } = false;

    [Header("Drying Settings")]
    [Tooltip("Seconds of rubbing with the towel required to dry.")]
    public float dryingTime = 3f;

    [Header("Materials")]
    [Tooltip("Material to use when the rock is wet.")]
    public Material wetMaterial;
    [Tooltip("Material to use when the rock is dry.")]
    public Material dryMaterial;

    private float dryingProgress = 0f;
    private Renderer rockRenderer;

    void Awake()
    {
        rockRenderer = GetComponent<Renderer>();
        ApplyWet();
    }

    /// <summary>
    /// Called each frame while the towel is rubbing the rock.
    /// </summary>
    public void DryStep(float deltaTime)
    {
        if (IsDry) return;

        dryingProgress += deltaTime;
        if (dryingProgress >= dryingTime)
        {
            SetDry();
        }
    }

    private void ApplyWet()
    {
        if (rockRenderer != null && wetMaterial != null)
            rockRenderer.material = wetMaterial;

        IsDry = false;
        dryingProgress = 0f;
        gameObject.tag = "Sample"; // still "wet" sample
    }

    private void SetDry()
    {
        IsDry = true;

        if (rockRenderer != null && dryMaterial != null)
            rockRenderer.material = dryMaterial;

        gameObject.tag = "DryStone"; // mark dry for collectors
    }
}
