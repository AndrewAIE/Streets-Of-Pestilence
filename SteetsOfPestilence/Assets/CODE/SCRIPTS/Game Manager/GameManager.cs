using PlayerController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //*************************************** VARAIBLES **********************************************//
    #region Variables

    //*** GAME STATE ***//
    #region Game State
    public enum GameState
    {
        Cutscene,
        Playing,
        Paused
    }

    public GameState m_Gamestate;


    #endregion

    /*** Player Manager ***/
    #region Player Manager
    public PlayerManager m_PlayerManager;

    #endregion

    //*** CUTSCENE ***//
    #region Cutscene
    [SerializeField] CutsceneManager _cutsceneManager;
    [SerializeField] bool _hasTriggeredCutscene;

    #endregion

    #endregion


    //*************************************** METHODS **********************************************//
    #region Methods

    /*** AWAKE & START ***/
    #region Awake & Start

    private void Awake()
    {
        //make sure theres only 1 game manager
        GameManager[] gameManagers = FindObjectsOfType<GameManager>();

        if (gameManagers.Length == 1)
        {
            //set this game object to do not destroy on load
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        //assign scripts
        AssignScripts();
    }


    // Start is called before the first frame update
    void Start()
    {
        //trigger cutscene if not played yet
        if (!_hasTriggeredCutscene)
            Debug.Log("Triggered Cutscene");
            TriggerCutscene();
    }

    private void AssignScripts()
    {
        m_PlayerManager = FindObjectOfType<PlayerManager>();
        _cutsceneManager = FindObjectOfType<CutsceneManager>();
    }
    #endregion

    //*** Update ***//
    #region Update

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(ReloadScene());
    }


    #endregion

    /*** GAME STATE ***/
    #region Game State
    public void SetGameState(GameState state)
    {
        m_Gamestate = state;

        switch (m_Gamestate)
        {
            case GameState.Cutscene:
                m_PlayerManager.SetPlayerInActive();
                break;
            case GameState.Playing:
                m_PlayerManager.SetPlayerActive();
                _cutsceneManager.TurnOff_PressAtoSkip();
                break;
            case GameState.Paused:
                break;
        }
    }

    public void SetGameState_Cutscene()
    {
        SetGameState(GameState.Cutscene);
    }

    public void SetGameState_Playing()
    {
        SetGameState(GameState.Playing);
    }

    #endregion

    /*** Cutscene ***/
    #region Cutscene
    public void TriggerCutscene()
    {
        //set game state to cutscene
        //this also disables player movement and camera control
        SetGameState(GameState.Cutscene);

        //set trigger cutscene to false
        _hasTriggeredCutscene = true;

        //start cutscene
        _cutsceneManager.TriggerCutscene();
    }

    #endregion

    /*** RELOAD SCENE ***/
    #region Reload Scene
    public IEnumerator ReloadScene()
    {
        SceneManager.LoadScene(0);

        yield return new WaitForEndOfFrame();

        AssignScripts();
    }

    #endregion

    #endregion
}
