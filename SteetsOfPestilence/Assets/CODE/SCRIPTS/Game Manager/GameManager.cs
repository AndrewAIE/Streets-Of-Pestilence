using PlayerController;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Management
{
    public enum GameState
    {
        Cutscene,
        Playing,
        Paused
    }

    public class GameManager : MonoBehaviour
    {
        //*************************************** VARAIBLES **********************************************//
        #region Variables
        internal GameState m_Gamestate;


        #region OtherObjects
        public PlayerManager m_PlayerManager { get; private set; }

        public BossDoor m_BossDoor { get; private set; }

        #endregion
        #region Menus
        [SerializeField] public PauseMenu m_PauseMenu;
        #endregion

        private PlayerInputMap m_input;
        private InputAction m_pauseInput, m_exitInput;


        //*** CUTSCENE ***//
        #region Cutscene
        //[SerializeField] CutsceneManager _cutsceneManager;
        //[SerializeField] bool _hasTriggeredCutscene;

        #endregion

        #endregion

        //*************************************** METHODS **********************************************//
        #region Methods

        /*** AWAKE & START ***/
        #region Awake & Start
        private void Awake()
        {
            m_input = new PlayerInputMap();
            /*m_PauseMenu = GetComponentInChildren<PauseMenu>();*/
            //make sure theres only 1 game manager
            GameManager[] gameManagers = FindObjectsOfType<GameManager>();
            if (gameManagers.Length == 1)
            {
                //set this game object to do not destroy on load
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            //Set frame rate to 60
            Application.targetFrameRate = 60;
            //assign scripts
            AssignScripts();
        }


        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            //trigger cutscene if not played yet
            /*if (!_hasTriggeredCutscene)
                Debug.Log("Triggered Cutscene");
                TriggerCutscene();*/
        }
        private void OnEnable()
        {
            m_pauseInput = m_input.UI.Pause;
            m_pauseInput.Enable();
            m_exitInput = m_input.UI.Exit;
            m_exitInput.Enable();
        }
        private void OnDisable()
        {
            m_pauseInput.Disable();
            m_exitInput.Disable();
        }


        private void AssignScripts()
        {
            m_PlayerManager = FindObjectOfType<PlayerManager>();
            //_cutsceneManager = FindObjectOfType<CutsceneManager>();
        }
        #endregion

        //*** Update ***//
        #region Update

        private void Update()
        {
            if (SceneChanger.CurrentScene != 0)
            {
                if (m_pauseInput.WasPressedThisFrame())
                {
                    m_PauseMenu.Pause();
                }
                else if (m_exitInput.WasPressedThisFrame() && m_Gamestate == GameState.Paused)
                {
                    m_PauseMenu.Pause();
                }
            }
            else if(m_Gamestate == GameState.Paused)
            {
                m_PauseMenu.Pause();
            }
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
                    m_PlayerManager.SetPlayerActive(false);
                    break;
                case GameState.Playing:
                    if (!m_PlayerManager.PlayerInCombat())
                    {
                        m_PlayerManager.SetPlayerActive(true);
                        //_cutsceneManager.TurnOff_PressAtoSkip();
                    }
                    break;
                case GameState.Paused:
                    m_PlayerManager.SetPlayerActive(false);
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
        /*
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
        */

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
}