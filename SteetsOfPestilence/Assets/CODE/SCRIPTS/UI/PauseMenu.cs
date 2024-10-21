using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Management
{
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        /// name for the Main Menu Scene
        /// </summary>
        [SerializeField] private string m_MainMenuName = "MainMenu";
        private GameManager m_manager;

        private void Start()
        {
            m_manager = GetComponentInParent<GameManager>();

            EventSystem.current.SetSelectedGameObject(null);
            EnableChildren(false);
            Time.timeScale = 1;
            m_manager.SetGameState(GameState.Playing);
        }

        public void Pause()
        {
            bool pause = m_manager.m_Gamestate != GameState.Paused;
            if (pause)
            {
                EnableChildren(true);
                Button m_firstButton = GetComponentInChildren<Button>();
                EventSystem.current.SetSelectedGameObject(m_firstButton.gameObject);
                Time.timeScale = 0;
                m_manager.SetGameState(GameState.Paused);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                EnableChildren(false);
                Time.timeScale = 1;
                m_manager.SetGameState(GameState.Playing);
            }
        }
        
        private void DIsableMenu()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EnableChildren(false);
            Time.timeScale = 1;
            m_manager.SetGameState(GameState.Playing);
        }

        private void EnableChildren(bool _enable)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(_enable);
            }
        }


        public void RestartLevel()
        {
            SceneChanger.ResetScene();
        }

        public void GoToMainMenu()
        {
            SceneChanger.ChangeScene(m_MainMenuName);
        }
        public void QuitGame()
        {
            SceneChanger.QuitGame();
        }

    }
}
