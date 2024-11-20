using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Management
{
    public class MainMenu : MonoBehaviour
    {
        private void Awake()
        {

        }
        private Button button;
        private void Update()
        {
            if (button == null)
            {
                button = GetComponentInChildren<Button>();
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
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