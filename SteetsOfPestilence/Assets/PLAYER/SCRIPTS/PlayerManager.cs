using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTESystem;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.InputSystem.UI;
using System.Data;

namespace PlayerController
{
    public enum PlayerState
    {
        Exploring,
        Combat,
        Resting
    }


    public class PlayerManager : Entity
    {

        // Variables
        #region Player Data
        [Header("Player Data")]
        [SerializeField] public PlayerData _data;

        

        /// <summary>
        /// if the user can currently control their avatar
        /// </summary>
        [SerializeField] public bool m_canMove;

        /// <summary>
        /// the current player state
        /// </summary>
        [SerializeReference] private PlayerState m_playerState;
        private PlayerState m_previousState;
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
        internal CameraController m_cameraController { get; private set; }
        [HideInInspector] internal SFXController _sfx { get; private set; }

        [HideInInspector] public PlayerInteraction m_Interact;

        private QTEManager m_qteRunner;


        #endregion
        #region Input
        internal InputStruct m_input { get; private set; }
        private PlayerInputMap m_movementInputs;
        private InputAction m_moveInput, m_lookInput, m_recenterInput, m_interactInput, m_sprintInput;

        #endregion
        #region Movement
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

        private Vector3 m_spawnPoint; private Quaternion m_spawnRotation;

        // Functions
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
            m_cameraController = FindObjectOfType<CameraController>();
            //get the merchant
            //_merchants = FindObjectsOfType<MerchantController>();


            m_qteRunner = GetComponent<QTEManager>();

            GetComponent<PlayerInput>().uiInputModule = FindObjectOfType<InputSystemUIInputModule>();

        }
        private void Start()
        {
            m_canMove = true;

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
            CheckStateChange();

            DebugStuff();

            if (m_canMove)
            {
                switch (m_playerState)
                {
                    case PlayerState.Exploring:
                        PlayerMovement();
                        break;
                    case PlayerState.Resting:

                        break;
                    case PlayerState.Combat:

                        break;
                }
            }


           
            AnimationHandler();
        }
        private void FixedUpdate()
        {
            
        }

        private void CheckStateChange()
        {
            if(m_playerState != m_previousState)
            {
                switch (m_playerState)
                {
                    case PlayerState.Exploring:

                        break;
                    case PlayerState.Resting:

                        break;
                    case PlayerState.Combat:

                        break;
                }
            }
        }

        private void DebugStuff()
        {

        }

        #endregion
        #region Movement
        private void PlayerMovement()
        {
            GroundedCheck();
            FallAndGravity();

            Rotate_Exploring();
            Move_Exploring();
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
            if (m_canMove)
                _animation.SetAnimationFloat_InputMove(m_input.movement.magnitude);
            else
                _animation.Idle();
        }

        #endregion
        #region Combat

        public bool PlayerInCombat() => m_qteRunner.enabled;

        public void EnterCombat(QTEEncounterData _encounterData, GameObject _enemy)
        {
            m_canMove = false;
            m_qteRunner.enabled = true;
            m_qteRunner.LoadEncounter(_encounterData, _enemy);
        }
        public void SetSpawn(Vector3 position, Quaternion rotation)
        {
            m_spawnPoint = position;
            m_spawnRotation = rotation;
        }
        public void KillPlayer()
        {
            transform.position = m_spawnPoint;
            transform.rotation = m_spawnRotation;
        }

        #endregion
        #region Enable / Disable Player

        public void SetPlayerActive(bool active)
        {
            m_canMove = active;
            m_cameraController.SetFreeLookCam_Active(active);
        }

        #endregion
        #region Inputs

        private void GatherInput()
        {
            m_input = new InputStruct
            {
                movement = m_moveInput.ReadValue<Vector2>(),
                look = m_lookInput.ReadValue<Vector2>(),
                sprint = m_sprintInput.IsPressed(),
            };
            if (m_interactInput.WasPressedThisFrame()) m_Interact.Interact();
            if (m_recenterInput.WasPressedThisFrame()) m_cameraController.TriggerRecenter();
        }
        #endregion
    }

    internal struct InputStruct
    {
        public Vector2 movement;
        public Vector2 look;
        public bool sprint;
    }
}
