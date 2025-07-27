

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;


public class ControllerVisibility : MonoBehaviour
{

    [SerializeField] private NearFarInteractor nearFarInt;
    [SerializeField] private GameObject controllerVisual;

    private bool isGrabbed = false;

    private void Start()
    {
        nearFarInt.selectEntered.AddListener(OnGrab);
        nearFarInt.selectExited.AddListener(OnLetGo);
    }

    private void Update()
    {
        if (isGrabbed)
        {
            if (nearFarInt.selectionRegion.Value == NearFarInteractor.Region.Near)
            {
                if (controllerVisual.activeSelf)
                {
                    controllerVisual.SetActive(false);
                }
            }
        }
        else
        {
            if (!controllerVisual.activeSelf && PlayerPrefs.GetInt("avatar") == 0)
            {
                controllerVisual.SetActive(true);
            }
        }
    }

    private void OnLetGo(SelectExitEventArgs arg0)
    {
        isGrabbed = false;
    }

    private void OnGrab(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
    }
}
