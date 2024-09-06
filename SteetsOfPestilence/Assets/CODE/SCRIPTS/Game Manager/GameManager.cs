using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    #endregion




    private void Awake()
    {
        //set this game object to do not destroy on load
        DontDestroyOnLoad(gameObject);

        

    }

    // Start is called before the first frame update
    void Start()
    {
        SetGameState(GameState.Cutscene);
        
    }

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


}
