using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace PlayerController
{
    public class MovementController : MonoBehaviour
    {
        /******************************* VARIABLES ********************************/
        #region Variables

        /*** MOVEMENT ***/
        #region Movement

        /*** InputStruct\ State ***/
        #region Movement State
        [Header("InputStruct State")]
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

        #endregion

        // player

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

        #endregion

        /*** FALLING ***/
        #region Falling
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        [SerializeReference] protected bool _grounded = true;
        [SerializeField] private LayerMask _groundLayers;

        #endregion

        /*** COMPONENTS ***/
        #region Components
        protected PlayerManager _manager;
        protected CharacterController _characterController;
        internal Camera _camera { get; private set; }
        private Transform m_Mesh;
        #endregion

        #endregion
        #region Getters
        public Vector3 centerPoint => _characterController.center;
        #endregion
        /******************************* METHODS ********************************/
        #region Methods

        /*** AWAKE & START ***/
        #region Awake & Start

        private void Awake()
        {
            //get manager
            _manager = GetComponent<PlayerManager>();
            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;
            m_Mesh = GetComponentInChildren<Animator>().transform;
        }

        private void Start()
        {
            _canMove = true;
            // reset our timeouts on start
            _fallTimeoutDelta = _manager._data.FallTimeout;
        }

        #endregion

        /*** UPDATE ***/
        #region Update
        private void Update()
        {
            if(_canMove)
            {
                //call method that hnadles all player movement
                PlayerMovement();
            }
            else
            {
                //do nothing lol
            }            
        }

        #endregion

        /************************************ MOVEMENT ******************************/
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

        /*** Exploring ***/
        #region Exploring
        //Move Method that controls the players movement
        private void Move_Exploring()
        {
            _speed = _manager.m_input.sprint ? _manager._data.RunningSpeed : _manager._data.WalkingSpeed;
            Vector2 motion = Vector3.zero;
            Vector2 input = _manager.m_input.movement;
            if (input.x != 0) motion.x += input.x * _speed;
            else motion.x = 0;
            if (input.y != 0) motion.y += input.y * _speed;

            Vector3 forward = _camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            _motionDirection = (forward * motion.y) + (_camera.transform.right * motion.x);

            _motionDirection = Vector3.ClampMagnitude(_motionDirection, _speed);
            _manager._animation.SetAnimationFloat_InputMove(input.magnitude);
            _characterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            _characterController.Move(_motionDirection * Time.deltaTime);
        }

       
        private void Rotate_Exploring()
        {  
            if(_motionDirection.magnitude > 0)
                m_Mesh.transform.forward = _motionDirection.normalized;
        }

        #endregion



        /*** Debug Move ***/
        #region Debug


        #endregion

        #endregion

        /************ FALLING & GRAVITY ********/
        #region Falling & Gravity

        //falling method
        private void FallAndGravity()
        {
            if (_grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = _manager._data.FallTimeout;

                // update animator if using character
                //_manager._animation._animator.SetBool(_manager._animation._animIDFreeFall, false);
                //_manager._animation.SetAnimation_FreeFall(false);

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
            if (_verticalVelocity < _manager._data.TerminalVelocity)
            {
                _verticalVelocity += _manager._data.Gravity * Time.deltaTime;
            }
        }
        
        //Generic Ground Check
        private void GroundedCheck()
        {
            // set sphere position, with offset
            /*Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _manager._data.GroundedOffset,
                transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _manager._data.GroundedRadius, _manager._data.GroundLayers,
                QueryTriggerInteraction.Ignore);*/

            _grounded = Physics.SphereCast(_characterController.center, _characterController.radius, Vector3.down, out RaycastHit hitInfo, (_characterController.height / 5 + .1f), _groundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            //_manager._animation._animator.SetBool(_manager._animation._animIDGrounded, _manager._character.isGrounded);
            //_manager._animation.SetAnimation_Grounded(_grounded);
        }

        #endregion

        #endregion

        
    }
}