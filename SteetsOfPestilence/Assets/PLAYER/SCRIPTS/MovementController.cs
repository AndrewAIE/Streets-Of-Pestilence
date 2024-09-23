using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class MovementController : MonoBehaviour
    {
        /******************************* VARIABLES ********************************/
        #region Variables

        /*** MOVEMENT ***/
        #region Movement

        /*** Movement State ***/
        #region Movement State
        [Header("Movement State")]
        //bool that controls if movement is enabled or not
        [SerializeField] public bool _canMove;

        //Movement Mode Enum that controls what type of movement the player is doing
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
        [SerializeField] public ExploringState _exploringState;
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

        [Header("Movement Variables")]
        [SerializeField] private float _inputMagnitude;
        [HideInInspector] private Vector3 _inputDirection;
        [HideInInspector] private Vector3 _currentDirection;
        [Space]
        [SerializeField] private float _speed;
        [SerializeField] private float _targetSpeed;
        [Space]
        [SerializeField] private float _inputRotation = 0.0f;
        [SerializeField] private float _targetRotation;
        [HideInInspector] private Vector3 _frameMotion;
        [SerializeField] private float _deltaRotation;
        [Space]
        [HideInInspector] private float _rotationVelocity;
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
                    Move_Exploring();
                    break;

                case MovementMode.LockOn:

                    break;
                case MovementMode.Debug:
                    Move_Debug();
                    break;
            }
        }

        /*** Exploring ***/
        #region Exploring
        //Move Method that controls the players movement
        private void Move_Exploring()
        {
            float storeY = 
            _speed = _manager.m_playerInputs.sprint ? _manager._data.RunningSpeed : _manager._data.WalkingSpeed;
            Vector2 motion = Vector3.zero;
            if (_manager._input.move.x != 0) motion.x += _manager._input.move.x * _speed;
            else motion.x = 0;
            if (_manager._input.move.y != 0) motion.y += _manager._input.move.y * _speed;

            _frameMotion = (transform.forward * motion.y + transform.right * motion.x);

            _frameMotion = Vector3.ClampMagnitude(motion, _speed);

            _characterController.Move(_frameMotion);
        }

        #endregion



        /*** Debug Move ***/
        #region Debug
        private void Move_Debug()
        {
            float _inputMagnitude = _manager._input.move.magnitude;

            float targetSpeed;

            if (_inputMagnitude >= 0.5f)
            {
                targetSpeed = _manager._data.DebugSpeed;
            }
            else
            {
                targetSpeed = 0.0f;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_manager._input.move.x, 0.0f, _manager._input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_manager._input.move != Vector2.zero)
            {
                _inputRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  Camera.main.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _inputRotation, ref _rotationVelocity,
                    _manager._data.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _inputRotation, 0.0f) * Vector3.forward;

            _characterController.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime));


        }

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