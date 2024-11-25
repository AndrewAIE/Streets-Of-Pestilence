using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public Slider progressBar; // Reference to your UI loading bar slider
    private bool m_Loading;


    IEnumerator Start()
    {
        yield return null;
        StartCoroutine(LoadMainSceneAsync());
    }

    IEnumerator LoadMainSceneAsync()
    {
        m_Loading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);

        // Prevent the main scene from activating immediately
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // Update progress bar (value between 0 and 1)
            if(progressBar) progressBar.value = asyncLoad.progress / 0.9f;

            // Check if the scene has finished loading
            if (asyncLoad.progress >= 0.9f)
            {
                // Optional: Wait for user input or a delay before activation
                asyncLoad.allowSceneActivation = true;
            }

            yield return null; // Wait for the next frame
        }
    }
}
