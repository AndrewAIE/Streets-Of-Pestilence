using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class SceneChanger : MonoBehaviour
    {
        internal static int CurrentScene => SceneManager.GetActiveScene().buildIndex;
        internal static void ResetScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }

        internal static int ChangeScene(string _sceneName)
        {
            SceneManager.LoadScene(_sceneName);
            return SceneManager.GetActiveScene().buildIndex;
        }
        internal static int ChangeScene(int _sceneIndex)
        {
            SceneManager.LoadScene(_sceneIndex);
            return SceneManager.GetActiveScene().buildIndex;
        }
        public static void QuitGame()
        {
            Application.Quit();
        }
    }
}
