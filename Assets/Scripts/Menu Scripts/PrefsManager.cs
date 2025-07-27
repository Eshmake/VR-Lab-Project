using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class PrefsManager : MonoBehaviour
{

    public GameObject turn;

    public GameObject leftController;
    public GameObject rightController;

    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject headTarget;
    public GameObject leftTarget;
    public GameObject rightTarget;
    public GameObject fullBody;


    void Start()
    {
        ApplyTurnPref();
        ApplyAvatarPref();
    }

    void ApplyTurnPref()
    {
        var snapTurn = turn.GetComponent<SnapTurnProvider>();
        var continuousTurn = turn.GetComponent<ContinuousTurnProvider>();

        if (PlayerPrefs.HasKey("turn"))
        {
            int turnIndex = PlayerPrefs.GetInt("turn");

            if (turnIndex == 0)
            {
                snapTurn.enabled = true;
                continuousTurn.enabled = false;
            }
            else if (turnIndex == 1)
            {
                snapTurn.enabled = false;
                continuousTurn.enabled = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("turn", 0);

            snapTurn.enabled = true;
            continuousTurn.enabled = false;
        }
    }

    void ApplyAvatarPref()
    {
        if (PlayerPrefs.HasKey("avatar"))
        {
            int avatarIndex = PlayerPrefs.GetInt("avatar");
            if (avatarIndex == 0)
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
            else if (avatarIndex == 1)
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

            else if (avatarIndex == 2)
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
        else
        {
            PlayerPrefs.SetInt("avatar", 0);

            leftController.SetActive(true);
            rightController.SetActive(true);

            leftHand.SetActive(false);
            rightHand.SetActive(false);

            fullBody.SetActive(false);
            headTarget.SetActive(false);
            leftTarget.SetActive(false);
            rightTarget.SetActive(false);
        }
    }
    
}
