using System.Collections.Generic;
using UnityEngine;
using QTESystem;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using EnemyAI;
using System.Collections;
using Pixelplacement.TweenSystem;

namespace PlayerController
{
    public enum PlayerState
    {
        Exploring,
        Combat,
        Resting
    }
    public struct SpawnPoint
    {
        public Vector3 position;
        public Quaternion rotation;

        public SpawnPoint(Vector3 _pos, Quaternion _rot)
        {
            position = _pos;
            rotation = _rot;
        }
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
        [SerializeField] public Transform m_playerTransform;
        [SerializeField] public Transform m_freeLookTarget;
        #endregion
        #region Debugging
        [Header("Debugging")]
        [SerializeField] private bool _debugMode;
        #endregion
        #region Components
        [HideInInspector] internal AnimationController _animation { get; private set; }
        internal CameraController m_cameraController { get; private set; }
        [HideInInspector] internal SFXController_Player _sfx { get; private set; }

        [HideInInspector] public PlayerUI m_playerUI;

        private QTEManager m_qteManager;


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
        Vector3 m_cameraDefaultPos;
        // timeout deltatime
        private float _fallTimeoutDelta;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        [SerializeReference] protected bool _grounded = true;
        [SerializeField] private LayerMask _groundLayers;


        private Transform m_recenterTarget;
        private bool m_moveRecenter;
        /*** COMPONENTS ***/
        #region Components
        protected CharacterController _characterController;
        internal Camera _camera { get; private set; }
        private Transform m_Mesh;
        #endregion

        #endregion

        private SpawnPoint m_spawnPoint;
        private List<SpawnPoint> m_unlockedCheckpoints;

        // Functions
        #region Startup
        private void Awake()
        {
            //get player scripts
            _animation = GetComponent<AnimationController>();

            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;
            m_cameraDefaultPos = m_freeLookTarget.localPosition;
            m_Mesh = GetComponentInChildren<Animator>().transform;

            _sfx = GetComponent<SFXController_Player>();

            //get camera script
            m_cameraController = FindObjectOfType<CameraController>();
            //get the merchant
            //_merchants = FindObjectsOfType<MerchantController>();
            m_playerUI = GetComponentInChildren<PlayerUI>();

            m_qteManager = GetComponentInChildren<QTEManager>();

            GetComponent<PlayerInput>().uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
            
        }
        private void Start()
        {
            m_canMove = true;
            m_unlockedCheckpoints = new List<SpawnPoint>();
            m_spawnPoint = new SpawnPoint(transform.position, transform.rotation);
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

            switch (m_playerState)
            {
                case PlayerState.Exploring:
                    if (m_canMove)
                        PlayerMovement();
                    break;
                case PlayerState.Resting:

                    break;
                case PlayerState.Combat:
                    if (m_recenterTarget != null) Recenter();
                    if (m_qteManager.enabled == false) m_playerState = PlayerState.Exploring;
                    MoveCameraPoint();
                    break;
            }



            AnimationHandler();
        }
        private void FixedUpdate()
        {

        }

        private void CheckStateChange()
        {
            if (m_playerState != m_previousState)
            {
                switch (m_playerState)
                {
                    case PlayerState.Exploring:
                        MoveCameraPoint();
                        break;
                    case PlayerState.Resting:

                        break;
                    case PlayerState.Combat:
                        MoveCameraPoint();
                        break;
                }
                m_previousState = m_playerState;
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

        private void MoveCameraPoint()
        {
            switch (m_playerState)
            {
                case PlayerState.Exploring:
                    m_cameraController.SetCameraFollow(m_playerTransform);
                    m_freeLookTarget.localPosition = m_cameraDefaultPos;
                    break;
                case PlayerState.Resting:

                    break;
                case PlayerState.Combat:
                    m_cameraController.SetCameraFollow(m_freeLookTarget);
                    Vector3 playerPos = m_cameraDefaultPos;
                    Vector3 enemyPos = m_recenterTarget.InverseTransformPoint(transform.position);

                    Vector3 centerPos = (enemyPos + playerPos) / 2;
                    centerPos.y = playerPos.y;
                    m_freeLookTarget.localPosition = centerPos;

                    Debug.DrawLine(playerPos, centerPos);
                    break;
            }
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
        private void AnimationHandler()
        {
            if (m_canMove)
            {
                _animation.SetAnimationFloat_Speed(_speed);
                _animation.SetAnimationFloat_MoveInput(m_input.movement.magnitude);
            }
            else
                _animation.Idle();
        }
        #endregion
        #region Combat


        public bool PlayerInCombat() { return (m_playerState == PlayerState.Combat); }


        public void EnterCombat(QTEEncounterData _encounterData, EnemyController _enemy)
        {
            m_playerState = PlayerState.Combat;
            m_canMove = false;
            _enemy.Recentering = true;
            m_recenterTarget = _enemy.transform;
            m_qteManager.enabled = true;
            m_qteManager.LoadEncounter(_encounterData, _enemy);

        }

        private void Recenter()
        {
            Vector3 faceDir = new(m_recenterTarget.position.x - transform.position.x, 0, m_recenterTarget.position.z - transform.position.z); // get face direction (ignore y)
            transform.forward = faceDir.normalized; // face the enemy transform normalized
            m_Mesh.localRotation = Quaternion.identity;

            if (!m_moveRecenter) return;

            float currentDist = Vector3.Distance(m_recenterTarget.position, transform.position);
            if (currentDist - m_recenterTarget.GetComponent<EnemyController>().m_distancebuffer < m_recenterTarget.GetComponent<EnemyController>().m_distanceToPlayer)
            {
                _characterController.Move(-transform.forward * Time.deltaTime);
            }
            else m_moveRecenter = false;

        }
        public void EnableRecenterMovement()
        {
            m_moveRecenter = true;
        }


        public void KillPlayer()
        {            
            m_playerUI.DeathTransition();            
            m_Mesh.rotation = new Quaternion(0, 0, 0, 0);
            StartCoroutine(RespawnPlayer());
        }

        private IEnumerator RespawnPlayer()
        {
            yield return new WaitForSeconds(m_playerUI.DeathScreenFadeDuration + 1f);
            m_qteManager.FadeOutUI();
            _animation.ResetAnimation();
            SetPlayerActive(false);
            SpawnPoint spawnpoint = GetClosestSpawnPoint();
            transform.position = m_spawnPoint.position;
            transform.rotation = m_spawnPoint.rotation;
            m_Mesh.localRotation = Quaternion.identity;
            yield return new WaitForSeconds(m_playerUI.DeathScreenFadeDuration);
            SetPlayerActive(true);
        }
        private SpawnPoint GetClosestSpawnPoint()
        {
            SpawnPoint closest = m_unlockedCheckpoints[0];
            foreach (SpawnPoint point in m_unlockedCheckpoints)
            {
                if (Vector3.Distance(point.position, transform.position) < Vector3.Distance(closest.position, transform.position))
                    closest = point;
            }

            return closest;
        }


        public void EndCombat()
        {
            m_playerState = PlayerState.Exploring;
            m_recenterTarget = null;
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
            if (m_interactInput.WasPressedThisFrame()) m_playerUI.Interact();
            if (m_recenterInput.WasPressedThisFrame()) m_cameraController.TriggerRecenter();
        }
        #endregion

        public void UnlockSpawn(Vector3 position, Quaternion rotation)
        {
            m_spawnPoint = new SpawnPoint(position, rotation);
            Debug.Log("spawn point set at: " + m_spawnPoint.position);
            if (!m_unlockedCheckpoints.Contains(m_spawnPoint))
            {
                m_unlockedCheckpoints.Add(m_spawnPoint);
                Debug.Log("unlocked checkpoint at: " + m_spawnPoint.position);
            }
        }
    }

    internal struct InputStruct
    {
        public Vector2 movement;
        public Vector2 look;
        public bool sprint;
    }
}
