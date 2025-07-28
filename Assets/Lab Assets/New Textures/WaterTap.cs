using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTap : MonoBehaviour
{

    public Animator Tap;
    public GameObject openText;
    public GameObject closeText;

    public ParticleSystem RunningWater;

    public AudioSource openSound;

    private bool inReach;
    private bool isOpen;
    private bool isClosed;

    public void ReachSet(bool inUse)
    {
        if (inUse)
        {
            if (isClosed)
            {
                inReach = true;
                openText.SetActive(true);
            }

            if (isOpen)
            {
                inReach = true;
                closeText.SetActive(true);
            }
        }
        else
        {
            inReach = false;
            openText.SetActive(false);
            closeText.SetActive(false);
        }
        

    }

    void Start()
    {
        inReach = false;
        isClosed = true;
        isOpen = false;
        closeText.SetActive(false);
        openText.SetActive(false);
        RunningWater.Stop();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2.5f/*, layerMask*/))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
            if (hit.collider.CompareTag("Interactable") && hit.distance <= 2.5f)
            {
                ReachSet(true);
                Debug.Log("Object is Interactable");

            }
            else
            {
                ReachSet(false);
            }

        }
        else
        {
            ReachSet(false);
    
        }

        if (inReach && isClosed && Input.GetButtonDown("Interact"))
        {
            Tap.SetBool("Open", true);
            Tap.SetBool("Closed", false);
            openText.SetActive(false);
            openSound.Play();
            isOpen = true;
            isClosed = false;
            RunningWater.Play();
        }

        else if (inReach && isOpen && Input.GetButtonDown("Interact"))
        {
            Tap.SetBool("Open", false);
            Tap.SetBool("Closed", true);
            closeText.SetActive(false);
            openSound.Pause();
            isClosed = true;
            isOpen = false;
            RunningWater.Stop();
        }
    }
}
