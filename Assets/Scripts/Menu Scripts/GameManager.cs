using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public int turnIndex;
    public int avatarIndex;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadPlayerPrefs()
    {
        turnIndex = PlayerPrefs.GetInt("turn", 0);
        avatarIndex = PlayerPrefs.GetInt("avatar", 0);
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt("turn", turnIndex);
        PlayerPrefs.SetInt("avatar", avatarIndex);

        PlayerPrefs.Save();
    }
}
