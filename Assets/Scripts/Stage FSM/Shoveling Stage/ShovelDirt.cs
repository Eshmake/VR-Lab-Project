using UnityEngine;

public class ShovelDirt : MonoBehaviour
{
    public GameObject dirtVisual;
    public bool IsFull { get; private set; } = false;

    public void Fill()
    {
        IsFull = true;
        if (dirtVisual != null)
            dirtVisual.SetActive(true);
    }

    public void Empty()
    {
        IsFull = false;
        if (dirtVisual != null)
            dirtVisual.SetActive(false);
    }
}
