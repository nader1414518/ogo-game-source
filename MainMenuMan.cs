using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMan : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject mainPanel;
    #endregion

    #region BtnCallbacks
    public void StartGameBtnCallback()
    {
        SceneManager.LoadScene("Level001");
    }

    public void ExitBtnCallback()
    {
        Application.Quit();
    }
    #endregion
}
