using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class MainMenu : MonoBehaviour
{
    public void StartButtonHandler()
    {
        GameManager.instance.LoadScene("Room");
    }

    public void OptionButtonHandler()
    {
        Debug.Log("OptionButtonHandler");
    }

    public void ExitButtonHandler()
    {
        Application.Quit();
    }

    public void DevelopmentEnter()
    {
         GameManager.instance.LoadScene("Practice");
    }
}
