using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RockSample : MonoBehaviour
{
    [Header("Drying State")]
    public bool IsDry { get; private set; } = false;

    [Header("Drying Settings")]
    [Tooltip("Seconds of effective rubbing to dry.")]
    public float dryingTime = 3f;

    [Header("Materials")]
    public Material wetMaterial;
    public Material dryMaterial;

    [Header("Audio (optional)")]
    [Tooltip("One-shot SFX played when the rock finishes drying.")]
    public AudioSource dryCompleteSfx;

    private float _progress;
    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        ApplyWet();
    }

    public void DryStep(float dt)
    {
        if (IsDry) return;
        _progress += dt;
        if (_progress >= dryingTime)
            SetDry();
    }

    private void ApplyWet()
    {
        if (_renderer && wetMaterial) _renderer.material = wetMaterial;
        IsDry = false;
        _progress = 0f;
        if (CompareTag("Untagged")) gameObject.tag = "Sample";
    }

    private void SetDry()
    {
        IsDry = true;
        if (_renderer && dryMaterial) _renderer.material = dryMaterial;

        //play the completion sound once
        if (dryCompleteSfx != null)
            dryCompleteSfx.Play();

        // Optional: switch tag if you want collectors to require "DryStone"
        // gameObject.tag = "DryStone";
    }
}

