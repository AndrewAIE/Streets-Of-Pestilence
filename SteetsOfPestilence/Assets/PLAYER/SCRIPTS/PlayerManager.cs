using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTESystem;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class PlayerManager : MonoBehaviour
    {
        /*************************************** VARIABLES *****************************************/
        #region Variables

        #region Player Data
        [Header("Player Data")]
        [SerializeField] public PlayerData _data;
        #endregion

        #region Camera Components
        [Header("Camera Components")]
        [SerializeField] public Transform _playerTransform;
        [SerializeField] public Transform _freeLookTarget;
        #endregion

        #region Debugging
        [Header("Debugging")]
        [SerializeField] public bool _debugMode;
        #endregion

        #region Components
        [HideInInspector] public MovementController _movement;
        [HideInInspector] public AnimationController _animation;
        [HideInInspector] public InputController _input;
        [HideInInspector] public SFXController _sfx;
        [HideInInspector] public CameraController _camera;
        [HideInInspector] public CharacterController _character;
        private QTERunner m_qteRunner;
        private PlayerInput m_playerInput;
        #endregion

        #endregion

        /************************************* METHODS ********************************************/
        #region Methods

        /*** AWAKE AND UPDATE ***/
        #region Awake and Update
        private void Awake()
        {
            //get player scripts
            _movement = GetComponent<MovementController>();
            _animation = GetComponent<AnimationController>();
            _input = GetComponent<InputController>();
            _sfx = GetComponent<SFXController>();

            //get camera script
            _camera = FindObjectOfType<CameraController>();
            
            //get character controller
            _character = GetComponent<CharacterController>();

            m_qteRunner = GetComponent<QTERunner>();
            m_playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.BackQuote))
                _debugMode = !_debugMode;
            

            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            if (_debugMode)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    _movement._movementMode = MovementController.MovementMode.Debug;
                }
                else
                {
                    _movement._movementMode = MovementController.MovementMode.Exploring;
                }
            }
        }

        #endregion

        /*** Enter Combat ***/
        #region Enter Combat

        public void EnterCombat(QTEEncounterData _encounterData, GameObject _enemy)
        {
            m_playerInput.enabled = false;
            m_qteRunner.enabled = true;
            m_qteRunner.LoadEncounter(_encounterData, _enemy);
        }

        #endregion

        /*** ENABLE / DISABLE PLAYER ***/
        #region Enable / Disable Player

        public void SetPlayerActive()
        {
            _movement.enabled = true;
            _camera.SetFreeLookCam_Active();
        }

        public void SetPlayerInActive()
        {
            _movement.enabled = false;
            _camera.SetFreeLookCam_InActive();
        }


        #endregion

        #endregion
    }
}
