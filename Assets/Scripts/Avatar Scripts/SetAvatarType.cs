
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



public class SetAvatarType : MonoBehaviour
{
    public GameObject leftController;
    public GameObject rightController;
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject headTarget;
    public GameObject leftTarget;
    public GameObject rightTarget;
    public GameObject fullBody;



    // Start is called before the first frame update
    void Start()
    {
        ApplyPlayerPref();
    }

    public void ApplyPlayerPref()
    {

        if (PlayerPrefs.HasKey("avatar"))
        {
            int value = PlayerPrefs.GetInt("avatar");
            if (value == 0)
            {
                leftController.SetActive(true);
                rightController.SetActive(true);

                leftHand.SetActive(false);
                rightHand.SetActive(false);

                fullBody.SetActive(false);
                headTarget.SetActive(false);
                leftTarget.SetActive(false);
                rightTarget.SetActive(false);
            }
            else if (value == 1)
            {
                leftController.SetActive(false);
                rightController.SetActive(false);

                leftHand.SetActive(true);
                rightHand.SetActive(true);

                fullBody.SetActive(false);
                headTarget.SetActive(false);
                leftTarget.SetActive(false);
                rightTarget.SetActive(false);
            }

            else if(value == 2)
            {
                leftController.SetActive(false);
                rightController.SetActive(false);

                leftHand.SetActive(false);
                rightHand.SetActive(false);

                fullBody.SetActive(true);
                headTarget.SetActive(true);
                leftTarget.SetActive(true);
                rightTarget.SetActive(true);
            }
        }
    }
}
