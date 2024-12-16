using Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;


public class StartEndController : MonoBehaviour
{
    [SerializeField] PauseMenu _pauseMenu;
    PlayerManager m_player;
    private void Start()
    {
        _pauseMenu = FindObjectOfType<PauseMenu>();
    }

    public void DisablePlayer()
    {
        m_player.SetPlayerActive(false);
    }
    public void Call_LoadMainMenu()
    {
        _pauseMenu.GoToMainMenu();
    }
}
