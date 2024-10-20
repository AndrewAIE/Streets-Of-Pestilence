using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }
    public static void QuitGame()
    {
        Application.Quit();
    }
}
