using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        }

        public void Pause(bool pause)
        {
            if (pause)
            {
                Transform[] children = GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    child.gameObject.SetActive(true);
                }
                m_manager.SetGameState(GameState.Paused);
            }
            else
            {
                Transform[] children = GetComponentsInChildren<Transform>();
                foreach (Transform child in children) {
                    child.gameObject.SetActive(false);
                }

                m_manager.SetGameState(GameState.Playing);
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
