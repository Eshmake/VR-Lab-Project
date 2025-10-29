using UnityEngine;
using TMPro;

public class TextPanelController : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Assign either TextMeshProUGUI (UI) or TextMeshPro (3D). If left empty, will auto-find in children.")]
    public TMP_Text textElement;

    void Awake()
    {
        if (textElement == null)
            textElement = GetComponentInChildren<TMP_Text>(true);
    }

    // Old method you already had (kept for compatibility)
    public void setSSDText()
    {
        if (textElement != null)
            textElement.text = "313.313";
    }

    // New: set a numeric weight using a format string (e.g., \"{0:0.000} g\")
    public void setWeightText(float value, string format = "{0:0.000}")
    {
        if (textElement == null) return;
        textElement.text = string.Format(format, value);
    }

    // New: reset to zero (or any idle text you pass)
    public void setZeroText(string zeroDisplay = "0.000")
    {
        if (textElement == null) return;
        textElement.text = zeroDisplay;
    }

    public void setSubmergeText() { }
    public void setOvenText() { }
}
