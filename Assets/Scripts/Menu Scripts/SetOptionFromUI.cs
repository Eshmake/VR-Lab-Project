using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SetOptionFromUI : MonoBehaviour
{
    public Scrollbar volumeSlider;
    public TMPro.TMP_Dropdown turnDropdown;
    public SetTurnType turnType;

    public TMPro.TMP_Dropdown avatarDropdown;
    public SetAvatarType avatarType;



    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
        turnDropdown.onValueChanged.AddListener(SetTurnPlayerPref);
        avatarDropdown.onValueChanged.AddListener(SetAvatar);

        if (PlayerPrefs.HasKey("turn"))
            turnDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("turn"));
        

        if (PlayerPrefs.HasKey("avatar"))
        {
            Debug.Log("test1");
            avatarDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("avatar"));
        }
            
        

    }

    public void SetGlobalVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void SetTurnPlayerPref(int value)
    {
        PlayerPrefs.SetInt("turn", value); 
        turnType.ApplyPlayerPref();
    }
   
    public void SetAvatar(int value)
    {
        PlayerPrefs.SetInt("avatar", value);
        avatarType.ApplyPlayerPref();
    }
    
}
