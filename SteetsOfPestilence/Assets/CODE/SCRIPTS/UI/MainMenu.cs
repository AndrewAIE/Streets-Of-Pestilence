using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Management
{
    public class MainMenu : MonoBehaviour
    {
        private void Awake()
        {
            Button m_firstButton = GetComponentInChildren<Button>();
            EventSystem.current.SetSelectedGameObject(m_firstButton.gameObject);
        }

        public void StartGame()
        {
            SceneChanger.ChangeScene(1);
        }
        public void QuitGame()
        {
            SceneChanger.QuitGame();
        }
    }
}