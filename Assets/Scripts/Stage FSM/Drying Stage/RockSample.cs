using UnityEngine;

public class RockSample : MonoBehaviour
{
    [Header("State")]
    public bool IsDry { get; private set; } = false;

    [Header("Visuals")]
    [Tooltip("Renderer (or object) shown when wet.")]
    public GameObject wetVisual;
    [Tooltip("Renderer (or object) shown when dry.")]
    public GameObject dryVisual;

    [Header("Drying Settings")]
    [Tooltip("Seconds of rubbing needed to fully dry.")]
    public float dryingTime = 3f;

    private float dryingProgress = 0f;

    private void Awake()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (wetVisual) wetVisual.SetActive(!IsDry);
        if (dryVisual) dryVisual.SetActive(IsDry);
    }

    /// <summary>
    /// Called repeatedly while the towel is rubbing the rock.
    /// </summary>
    public void DryStep(float deltaTime)
    {
        if (IsDry) return;

        dryingProgress += deltaTime;
        if (dryingProgress >= dryingTime)
        {
            IsDry = true;
            UpdateVisuals();
        }
    }
}
