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
        [SerializeField] public bool _playerMovementEnabled;

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
        [HideInInspector] private Vector3 _targetDirection;
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
        public bool _grounded = true;

        #endregion

        /*** COMPONENTS ***/
        #region Components
        [HideInInspector] private PlayerManager _manager;

        #endregion

        #endregion

        /******************************* METHODS ********************************/
        #region Methods

        /*** AWAKE & START ***/
        #region Awake & Start

        private void Awake()
        {
            //get manager
            _manager = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            _playerMovementEnabled = true;
            // reset our timeouts on start
            _fallTimeoutDelta = _manager._data.FallTimeout;
        }

        #endregion

        /*** UPDATE ***/
        #region Update
        private void Update()
        {
            if(_playerMovementEnabled)
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
            //switch case to handle what type of movement to do
            switch (_movementMode)
            {
                case MovementMode.Exploring:
                    GroundedCheck();
                    FallAndGravity();

                    ExploringController();
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

        //exploring controller handles the exploring state machine
        private void ExploringController()
        {
            #region Get Input
            //get input
            _inputMagnitude = _manager._input.move.magnitude;

            // normalise input direction
            _inputDirection = new Vector3(_manager._input.move.x, 0.0f, _manager._input.move.y).normalized;

            #endregion

            #region Exploring State
            switch (_exploringState)
            {
                case ExploringState.Stationary:
                    if(_inputMagnitude >= 0.01f)
                    {
                        Set_ExploringState(ExploringState.Walking);
                    }

                    break;

                case ExploringState.Walking:
                    if (_inputMagnitude >= _manager._data.RunThres)
                        Set_ExploringState(ExploringState.Running);

                    else if(_inputMagnitude <= 0.01f)
                        Set_ExploringState(ExploringState.Stationary);
                    
                    break;

                case ExploringState.Running:
                    if (_inputMagnitude <= _manager._data.RunThres)
                        Set_ExploringState(ExploringState.Walking);
                    break;
            }

            #endregion
        }


        //Move Method that controls the players movement
        private void Move_Exploring()
        {
            #region Find Target Speed
            
            //find target speed
            switch (_exploringState)
            {
                case ExploringState.Running:
                    _targetSpeed = _manager._data.RunningSpeed;
                    break;

                case ExploringState.Walking:
                    _targetSpeed = _manager._data.WalkingSpeed;
                    break;

                case ExploringState.Stationary:
                    _targetSpeed = 0f;
                    break;
            }

            #endregion

            #region Find Speed

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_manager._character.velocity.x, 0.0f, _manager._character.velocity.z).magnitude;

            //set speed offset to 0.1f
            float speedOffset = 0.1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < _targetSpeed - speedOffset ||
                currentHorizontalSpeed > _targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed * _inputMagnitude,
                    Time.deltaTime * _manager._data.SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                //set speed to target speed
                _speed = _targetSpeed;
            }

            #endregion

            #region Rotation

            //get input rotation by doing maths on the input direction
            _inputRotation = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg +
                                Camera.main.transform.eulerAngles.y;

            //set the target and smooth damp it
            _targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _inputRotation, ref _rotationVelocity,
                _manager._data.RotationSmoothTime);

            #endregion
            
            if(_exploringState != ExploringState.Stationary)
            {
                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);
            }

            //make direction vector3
            _targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _manager._character.Move(_targetDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
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

            _manager._character.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime));


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
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _manager._data.GroundedOffset,
                transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _manager._data.GroundedRadius, _manager._data.GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            //_manager._animation._animator.SetBool(_manager._animation._animIDGrounded, _manager._character.isGrounded);
            //_manager._animation.SetAnimation_Grounded(_grounded);
        }

        #endregion

        #endregion
    }
}