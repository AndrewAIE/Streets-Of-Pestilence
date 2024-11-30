using Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEndController : MonoBehaviour
{
    [SerializeField] PauseMenu _pauseMenu;

    private void Start()
    {
        _pauseMenu = FindObjectOfType<PauseMenu>();
    }

    public void Call_LoadMainMenu()
    {
        _pauseMenu.GoToMainMenu();
    }
}
