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
        internal IEnumerator AsyncLoadScene(int _sceneIndex)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        internal IEnumerator AsyncDestroyScene(int _sceneIndex)
        {
            AsyncOperation asyncDestroy = SceneManager.UnloadSceneAsync(_sceneIndex);
            while(!asyncDestroy.isDone)
            yield return null;
        }

        public static void QuitGame()
        {
            Application.Quit();
        }
    }
}
