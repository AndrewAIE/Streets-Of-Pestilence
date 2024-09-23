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
        [SerializeField] private bool _debugMode;
        #endregion
        
        #region Components
        [HideInInspector] internal MovementController _movement { get; private set; }
        [HideInInspector] internal AnimationController _animation { get; private set; }
        [HideInInspector] internal InputController _input { get; private set; }
        [HideInInspector] internal SFXController _sfx { get; private set; }
        [HideInInspector] internal CameraController _camera { get; private set; }
        [HideInInspector] internal MerchantController _merchant { get; private set; }
        private QTEManager m_qteRunner;
        
        
        #endregion
        #region Input
        internal Movement m_playerInputs { get; private set; }
        private MovementInputs m_movementInputs;
        private InputAction m_moveInput, m_lookInput, m_recenterInput, m_interactInput, m_sprintInput;

        #endregion


        #endregion

        /************************************* METHODS ********************************************/
        #region Methods

        /*** AWAKE AND UPDATE ***/
        #region Startup
        private void Awake()
        {
            //get player scripts
            _movement = GetComponent<MovementController>();
            _animation = GetComponent<AnimationController>();
            _input = GetComponent<InputController>();
            _sfx = GetComponent<SFXController>();

            //get camera script
            _camera = FindObjectOfType<CameraController>();

            //get the merchant
            _merchant = FindObjectOfType<MerchantController>();
            

            m_qteRunner = GetComponent<QTEManager>();
            
        }
        private void OnEnable()
        {
            m_movementInputs = new MovementInputs();
            m_moveInput = m_movementInputs.Player.Move;
            m_moveInput.Enable();
            m_lookInput = m_movementInputs.Player.Look;
            m_lookInput.Enable();
            m_recenterInput = m_movementInputs.Player.Recenter;
            m_recenterInput.Enable();
            m_interactInput = m_movementInputs.Player.Interact;
            m_interactInput.Enable();
            m_sprintInput = m_movementInputs.Player.Sprint;
            m_sprintInput.Enable();
        }
        private void OnDisable()
        {
            m_moveInput.Disable();
            m_lookInput.Disable();
            m_recenterInput.Disable();
            m_interactInput.Disable();
            m_sprintInput.Disable();
        }


        #endregion
        #region Updates
        private void Update()
        {
            GatherInput();

            if(Input.GetKeyDown(KeyCode.BackQuote))
                _debugMode = !_debugMode;

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
            
            m_qteRunner.enabled = true;
            m_qteRunner.LoadEncounter(_encounterData, _enemy);
        }

        #endregion

        /*** ENABLE / DISABLE PLAYER ***/
        #region Enable / Disable Player

        public void SetPlayerActive()
        {
            _movement._canMove = true;
            _camera.SetFreeLookCam_Active();
        }

        public void SetPlayerInactive()
        {
            _movement._canMove = false;
            _camera.SetFreeLookCam_InActive();
        }


        #endregion

        /*** Merchant ***/
        #region Merchant
        
        public bool CheckMerchantState(MerchantController.MerchantState inputState)
        {
            return _merchant.CheckState(inputState);
        }

        public void SetMerchantState(MerchantController.MerchantState inputState)
        {
            _merchant.SetMerchantState(inputState);
        }


        #endregion
        /*** Input Management ***/
        #region Inputs

        private void GatherInput()
        {
            m_playerInputs = new Movement {
                movement = m_moveInput.ReadValue<Vector2>(),
                lookMovement = m_lookInput.ReadValue<Vector2>(),
                sprint = m_sprintInput.IsPressed(),
                recenter = m_recenterInput.WasPressedThisFrame(),
                interact = m_interactInput.WasPressedThisFrame()
            };
        }

        #endregion

        #endregion
    }
    
    internal struct Movement
    {
        public Vector2 movement;
        public Vector2 lookMovement;
        public bool sprint;
        public bool recenter;
        public bool interact;
    }
}
