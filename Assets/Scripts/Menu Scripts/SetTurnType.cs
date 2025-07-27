using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;



public class SetTurnType : MonoBehaviour
{
    public GameObject turn;


    // Start is called before the first frame update
    void Start()
    {
        ApplyPlayerPref();
    }

    public void ApplyPlayerPref()
    {

        var snapTurn = turn.GetComponent<SnapTurnProvider>();
        var continuousTurn = turn.GetComponent<ContinuousTurnProvider>();
        if (PlayerPrefs.HasKey("turn"))
        {
            int value = PlayerPrefs.GetInt("turn");
            if (value == 0)
            {
                snapTurn.enabled = true;
                continuousTurn.enabled = false;
            }
            else if (value == 1)
            {
                snapTurn.enabled = false;
                continuousTurn.enabled = true;
            }
        }
    }
}