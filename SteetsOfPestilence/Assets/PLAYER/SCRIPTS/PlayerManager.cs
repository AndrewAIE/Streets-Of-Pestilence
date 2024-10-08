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
        internal CameraController _cameraController { get; private set; }
        [HideInInspector] internal SFXController _sfx { get; private set; }
        [HideInInspector] internal MerchantController[] _merchants { get; private set; }
        private QTEManager m_qteRunner;
        
        
        #endregion
        #region Input
        internal InputStruct m_input { get; private set; }
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

            _sfx = GetComponent<SFXController>();

            //get camera script
            _cameraController = FindObjectOfType<CameraController>();

            //get the merchant
            _merchants = FindObjectsOfType<MerchantController>();
            

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

        public void SetPlayerActive(bool active)
        {
            _movement._canMove = active;
            _cameraController.SetFreeLookCam_Active(active);
        }
       
        #endregion

        /*** Merchant ***/
        #region Interactions
        
        private void Interact()
        {
            CheckForMechants();
        }

        private void CheckForMechants()
        {
            foreach (MerchantController mechant in _merchants)
            {

            }
        }

        #endregion
        /*** Input Management ***/
        #region Inputs

        private void GatherInput()
        {
            m_input = new InputStruct {
                movement = m_moveInput.ReadValue<Vector2>(),
                look = m_lookInput.ReadValue<Vector2>(),
                sprint = m_sprintInput.IsPressed(),
                recenter = m_recenterInput.WasPressedThisFrame(),
            };
            if (m_interactInput.WasPressedThisFrame()) Interact();

        }

        #endregion

        #endregion
    }
    
    internal struct InputStruct
    {
        public Vector2 movement;
        public Vector2 look;
        public bool sprint;
        public bool recenter;
    }
}
