using UnityEngine;

public class ShovelDirt : MonoBehaviour
{
    public GameObject dirtVisual;
    public bool IsFull { get; private set; } = false;

    public AudioSource shovelDig;
    public AudioSource shovelPour;

    public void Fill()
    {
        IsFull = true;
        if (dirtVisual != null)
            dirtVisual.SetActive(true);

        shovelDig.Play();
    }

    public void Empty()
    {
        IsFull = false;
        if (dirtVisual != null)
            dirtVisual.SetActive(false);

        shovelPour.Play();
    }
}
