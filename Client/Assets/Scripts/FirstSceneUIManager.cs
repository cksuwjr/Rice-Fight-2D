using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FirstSceneUIManager : MonoBehaviour
{
    public void BtnPlayClick()
    {
        SceneManager.LoadScene("Main");
    }
    public void BtnHelpClick()
    {
    }
    public void BtnSettingClick()
    {
    }
    public void BtnExitClick()
    {
    }
}
