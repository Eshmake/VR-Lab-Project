using UnityEngine;
using System;

public class RockLifecycleNotifier : MonoBehaviour
{
    public Action onRockGone;
    void OnDestroy() => onRockGone?.Invoke();
}
