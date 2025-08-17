using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using XRI_Socket = UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor; // alias

public class SocketWatcher : MonoBehaviour
{
    [Header("Assign the exact socket here (no auto-detect).")]
    public XRI_Socket socket;

    [Tooltip("Only invoke for objects with ANY of these tags. Leave empty to allow all.")]
    public string[] allowedTags = new[] { "Bowl" };

    [System.Serializable] public class ObjEvent : UnityEvent<GameObject> { }
    public ObjEvent onSnapped;
    public ObjEvent onUnsnapped;

    void OnEnable()
    {
        if (socket == null)
        {
            Debug.LogError($"[SocketWatcher:{name}] No socket assigned! Drag the correct XRSocketInteractor here.", this);
            return;
        }
        socket.selectEntered.AddListener(OnSelectEntered);
        socket.selectExited.AddListener(OnSelectExited);
        Debug.Log($"[SocketWatcher:{name}] Watching socket: {socket.gameObject.name}", this);
    }

    void OnDisable()
    {
        if (socket == null) return;
        socket.selectEntered.RemoveListener(OnSelectEntered);
        socket.selectExited.RemoveListener(OnSelectExited);
    }

    bool IsAllowed(GameObject go)
    {
        if (go == null) return false;
        if (allowedTags == null || allowedTags.Length == 0) return true;
        for (int i = 0; i < allowedTags.Length; i++)
        {
            var t = allowedTags[i];
            if (!string.IsNullOrWhiteSpace(t) && go.CompareTag(t))
                return true;
        }
        return false;
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        var go = args.interactableObject.transform.gameObject;
        Debug.Log($"[SocketWatcher:{name}] select ENTER on socket '{socket.gameObject.name}' -> {go.name}", this);
        if (IsAllowed(go)) onSnapped?.Invoke(go);
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        var go = args.interactableObject.transform.gameObject;
        Debug.Log($"[SocketWatcher:{name}] select EXIT on socket '{socket.gameObject.name}' -> {go.name}", this);
        if (IsAllowed(go)) onUnsnapped?.Invoke(go);
    }
}
