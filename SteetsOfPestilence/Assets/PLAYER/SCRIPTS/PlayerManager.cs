using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTESystem;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.InputSystem.UI;

namespace PlayerController
{
    public class PlayerManager : Entity
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
        [HideInInspector] internal AnimationController _animation { get; private set; }
        internal CameraController _cameraController { get; private set; }
        [HideInInspector] internal SFXController _sfx { get; private set; }
        [HideInInspector] internal MerchantController[] _merchants { get; private set; }
        private QTEManager m_qteRunner;
        
        
        #endregion
        #region Input
        internal InputStruct m_input { get; private set; }
        private PlayerInputMap m_movementInputs;
        private InputAction m_moveInput, m_lookInput, m_recenterInput, m_interactInput, m_sprintInput;

        #endregion
        #region Movement
        //bool that controls if movement is enabled or not
        [SerializeField] public bool _canMove;

        //InputStruct\ Mode Enum that controls what type of movement the player is doing
        [SerializeField] public MovementMode _movementMode;
        public enum MovementMode
        {
            Exploring,
            LockOn,
            Combat,
            Merchant,
            Debug
        }

        //ENum that displays what the player is doing while exploring
        [SerializeField] private ExploringState _exploringState;
        public enum ExploringState
        {
            Stationary,
            Walking,
            Running
        }

        public void Set_ExploringState(ExploringState _newState)
        {
            _exploringState = _newState;
        }

        [Header("InputStruct Variables")]
        [Space]
        [SerializeField] private float _speed;

        [Space]
        [SerializeField] private float _targetRotation;
        [SerializeReference] private Vector3 _motionDirection;
        [Space]
        [HideInInspector] private float _verticalVelocity;

        // timeout deltatime
        private float _fallTimeoutDelta;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        [SerializeReference] protected bool _grounded = true;
        [SerializeField] private LayerMask _groundLayers;

        /*** COMPONENTS ***/
        #region Components
        protected CharacterController _characterController;
        internal Camera _camera { get; private set; }
        private Transform m_Mesh;
        #endregion

        #endregion
        #region Getters
        public Vector3 centerPoint => _characterController.center;
        #endregion


#endregion

        /************************************* METHODS ********************************************/
        #region Methods

        /*** AWAKE AND UPDATE ***/
        #region Startup
        private void Awake()
        {
            //get player scripts
            _animation = GetComponent<AnimationController>();

            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;
            m_Mesh = GetComponentInChildren<Animator>().transform;

            _sfx = GetComponent<SFXController>();

            //get camera script
            _cameraController = FindObjectOfType<CameraController>();
            //get the merchant
            _merchants = FindObjectsOfType<MerchantController>();
            m_qteRunner = GetComponent<QTEManager>();

            GetComponent<PlayerInput>().uiInputModule = FindObjectOfType<InputSystemUIInputModule>();

        }
        private void Start()
        {
            _canMove = true;

        }


        private void OnEnable()
        {
            m_movementInputs = new PlayerInputMap();
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

                DebugStuff();

                if (_canMove)
                {
                    //call method that hnadles all player movement
                    PlayerMovement();
                }
                else
                {
                    //do nothing lol
                }
                AnimationHandler();
        }

        private void DebugStuff()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                _debugMode = !_debugMode;

            if (_debugMode)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                   _movementMode = MovementMode.Debug;
                }
                else
                {
                    _movementMode = MovementMode.Exploring;
                }
            }
        }

        #endregion
        #region Movement
        private void PlayerMovement()
        {
            GroundedCheck();
            FallAndGravity();
            //switch case to handle what type of movement to do
            switch (_movementMode)
            {
                case MovementMode.Exploring:
                    Rotate_Exploring();
                    Move_Exploring();
                    break;

                case MovementMode.LockOn:

                    break;
                case MovementMode.Debug:
                    //Move_Debug();
                    break;
            }
        }
        #region Exploring
        //Move Method that controls the players movement
        private void Move_Exploring()
        {
            _speed = m_input.sprint ? _data.RunningSpeed : _data.WalkingSpeed;
            Vector2 motion = Vector3.zero;
            Vector2 input = m_input.movement;
            if (input.x != 0) motion.x += input.x * _speed;
            else motion.x = 0;
            if (input.y != 0) motion.y += input.y * _speed;

            Vector3 forward = _camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            _motionDirection = (forward * motion.y) + (_camera.transform.right * motion.x);

            _motionDirection = Vector3.ClampMagnitude(_motionDirection, _speed);
            
            _characterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            _characterController.Move(_motionDirection * Time.deltaTime);
        }


        private void Rotate_Exploring()
        {
            if (_motionDirection.magnitude > 0)
                m_Mesh.transform.forward = _motionDirection.normalized;
        }

        #endregion
        #region Falling & Gravity

        //falling method
        private void FallAndGravity()
        {
            if (_grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = _data.FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }
            else
            {
                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    //_manager._animation._animator.SetBool(_manager._animation._animIDFreeFall, true);
                    //_manager._animation.SetAnimation_FreeFall(true);
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _data.TerminalVelocity)
            {
                _verticalVelocity += _data.Gravity * Time.deltaTime;
            }
        }

        //Generic Ground Check
        private void GroundedCheck()
        {
            _grounded = Physics.SphereCast(_characterController.center, _characterController.radius, Vector3.down, out RaycastHit hitInfo, (_characterController.height / 5 + .1f), _groundLayers, QueryTriggerInteraction.Ignore);
        }

        #endregion
        #endregion
        #region Animation Handlin
        // guess what it does..... i'll give you a hint, it handles combat
        private void AnimationHandler()
        {
            if (_canMove)
                _animation.SetAnimationFloat_InputMove(m_input.movement.magnitude);
            else
                _animation.Idle();
        }

        #endregion
        /*** Enter Combat ***/
        #region Enter Combat

        public bool PlayerInCombat() => m_qteRunner.enabled;

        public void EnterCombat(QTEEncounterData _encounterData, GameObject _enemy)
        {
            _canMove = false;
            m_qteRunner.enabled = true;
            m_qteRunner.LoadEncounter(_encounterData, _enemy);
        }

        #endregion

        /*** ENABLE / DISABLE PLAYER ***/
        #region Enable / Disable Player

        public void SetPlayerActive(bool active)
        {
            _canMove = active;
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
