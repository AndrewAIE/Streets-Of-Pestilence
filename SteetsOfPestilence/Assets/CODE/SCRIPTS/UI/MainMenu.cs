using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Management
{
    public class MainMenu : MonoBehaviour
    {
        private void Awake()
        {
            Button firstButton = GetComponentInChildren<Button>();
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }

        private void Update()
        {
            Button button = GetComponentInChildren<Button>();
            if (EventSystem.current == null) 
                EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        public void StartGame()
        {
            Debug.Log("Starting Game...");
            SceneChanger.ChangeScene(1);
        }
        public void QuitGame()
        {
            SceneChanger.QuitGame();
        }
    }
}