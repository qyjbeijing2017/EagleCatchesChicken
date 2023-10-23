using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartButtonHandler()
    {
        Debug.Log("StartButtonHandler");
    }

    public void OptionButtonHandler()
    {
        Debug.Log("OptionButtonHandler");
    }

    public void ExitButtonHandler()
    {
        Application.Quit();
    }
}
