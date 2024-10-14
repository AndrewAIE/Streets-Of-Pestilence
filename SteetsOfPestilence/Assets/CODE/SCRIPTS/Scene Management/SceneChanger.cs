using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class SceneChanger : MonoBehaviour
    {

        internal static void ResetScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }

        internal static void ChangeScene(string _sceneName)
        {
            SceneManager.LoadScene(_sceneName);
        }
       internal static void QuitGame()
        {
            Application.Quit();
        }
    }
}
