using UnityEngine;
using UnityEngine.SceneManagement;


public class WristUIActions : MonoBehaviour
{

    public GameObject quitPopup;
    public GameObject resetPopup;

    public GameObject quitButton;
    public GameObject resetButton;

    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenQuitPopup()
    {
        quitPopup.SetActive(true);
        resetButton.SetActive(false);
        quitButton.SetActive(false);
    }

    public void OpenResetPopup()
    {
        resetPopup.SetActive(true);
        resetButton.SetActive(false);
        quitButton.SetActive(false);
    }

    public void CloseQuitPopup()
    {
        quitPopup.SetActive(false);
        resetButton.SetActive(true);
        quitButton.SetActive(true);
    }

    public void CloseResetPopup()
    {
        resetPopup.SetActive(false);
        resetButton.SetActive(true);
        quitButton.SetActive(true);
    }
}
