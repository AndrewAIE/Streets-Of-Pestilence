using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    private bool m_Loading;
    [SerializeField] AudioMixer _mixerLoadingScene;
    public string exposedParameter = "Volume"; // Make sure this matches your exposed parameter name
    public float fadeDuration = 2.0f; // Time to fade in (seconds)
    public float targetVolume_ON = 0.0f; // Target volume in decibels (0 is default max in Unity)
    public float targetVolume_OFF = -80.0f; // Target volume in decibels (0 is default max in Unity)
    public float currentVolume;


    IEnumerator Start()
    {
        yield return null;
        StartCoroutine(FadeInAudio());
        StartCoroutine(LoadMainSceneAsync());
    }

    private IEnumerator FadeInAudio()
    {
        float currentTime = 0.0f;

        // Set the starting volume
        _mixerLoadingScene.SetFloat(exposedParameter, targetVolume_OFF);

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(targetVolume_OFF, targetVolume_ON, currentTime / fadeDuration);
            _mixerLoadingScene.SetFloat(exposedParameter, newVolume);
            yield return null;
        }

        // Ensure final value is set
        _mixerLoadingScene.SetFloat(exposedParameter, targetVolume_ON);
    }

    private IEnumerator FadeOutAudio()
    {
        float currentTime = 0f;

        // Set the starting volume
        _mixerLoadingScene.SetFloat(exposedParameter, targetVolume_ON);

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(targetVolume_ON, targetVolume_OFF, currentTime / fadeDuration);
            _mixerLoadingScene.SetFloat(exposedParameter, newVolume);
            currentVolume = newVolume;
            yield return null;
        }

        // Ensure final value is set
        currentVolume = targetVolume_OFF;
        _mixerLoadingScene.SetFloat(exposedParameter, targetVolume_OFF);
    }

    IEnumerator LoadMainSceneAsync()
    {
        m_Loading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);

        // Prevent the main scene from activating immediately
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {

            // Check if the scene has finished loading
            if (asyncLoad.progress >= 0.9f)
            {
                //turn down volume
                StartCoroutine(FadeOutAudio());

                //wait until volume 0
                yield return new WaitUntil(() => currentVolume == -80.00f);

                // Optional: Wait for user input or a delay before activation
                asyncLoad.allowSceneActivation = true;
            }

            yield return null; // Wait for the next frame
        }
    }
}
