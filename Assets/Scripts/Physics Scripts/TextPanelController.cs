using UnityEngine;
using TMPro;

public class TextPanelController : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Assign either TextMeshProUGUI (UI) or TextMeshPro (3D).")]
    public TMP_Text textElement; // works for both UGUI and 3D

    [Header("Format (optional)")]
    public string format = "{0} kg"; // e.g., "Weight: {0} g"

    public void setSSDText()
    {
        if (textElement == null)
        {
            Debug.LogWarning("[TextPanelController] textElement is not assigned.");
            return;
        }

        var value = "313.313";
        textElement.text = string.Format(format, value);
        // Debug to confirm it ran
        // Debug.Log("[TextPanelController] setSSDText applied: " + textElement.text);
    }

    public void setSubmergeText() { /* add later */ }
    public void setOvenText() { /* add later */ }
}
