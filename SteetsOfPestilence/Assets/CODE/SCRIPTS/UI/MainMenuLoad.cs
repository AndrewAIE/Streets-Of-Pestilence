using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoad : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Start");
    }
}
