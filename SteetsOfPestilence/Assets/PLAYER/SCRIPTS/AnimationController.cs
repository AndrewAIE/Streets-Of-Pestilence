using UnityEngine;


namespace PlayerController
{
    public class AnimationController : MonoBehaviour
    {
        /**************************************** VARIABLES ******************************/
        #region Variables
        [HideInInspector] private Animator _animator;
        private PlayerManager m_Manager;
        internal float m_movementSpeed;

        /*************************** ANIMATION ID'S *************************/
        #region Animation ID's
        //floats
        [HideInInspector] private int _animID_Speed;
        [HideInInspector] private int _animID_Input_Move;
        
        //bools
        [HideInInspector] private int _animID_Grounded;
        [HideInInspector] private int _animID_FreeFall;
        private int _animID_Running;

        //triggers
        /*private int _animID_Idle_Inspect;
        private int _animID_Idle_Scared;*/


        /**************************** VARAIBLES ***********************/
        /*public float _idleTimer;
        public float _idleTriggerTime;
        public float IdleMinTimer;
        public float IdleMaxTimer;
        private int _lastIdle;*/

        #endregion

        #endregion

        /**************************************** METHODS ******************************/
        #region Methods

        /*** AWAKE & START ***/
        #region Awake & Start

        private void Awake()
        {
            //get components
            _animator = GetComponentInChildren<Animator>();
            m_Manager = GetComponent<PlayerManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            AssignAnimationIDs();
            //_idleTriggerTime = Random.Range(IdleMinTimer, IdleMaxTimer);
        }

        #endregion

        /*** UPDATE ***/
        #region Update
        // Update is called once per frame
        void Update()
        {
            Movement();
        }

        #endregion
        private void Movement()
        {
            _animator.SetFloat(_animID_Input_Move, m_movementSpeed);
            if (m_Manager._speed >= m_Manager._data.RunningSpeed)
            {
                _animator.SetBool(_animID_Running, true);
            }
            else _animator.SetBool(_animID_Running, false) ;
        }
        internal void Idle()
        {
            m_movementSpeed = 0;

        }


        private void AssignAnimationIDs()
        {
            //floats
            _animID_Speed = Animator.StringToHash("Speed");
            _animID_Input_Move = Animator.StringToHash("Move Input");
            
            //bool
            _animID_Grounded = Animator.StringToHash("Grounded");
            _animID_FreeFall = Animator.StringToHash("FreeFall");
            _animID_Running = Animator.StringToHash("Running");

            /*//triggers
            _animID_Idle_Inspect = Animator.StringToHash("Idle_Inspect");
            _animID_Idle_Scared = Animator.StringToHash("Idle_Scared");*/
        }

        //speed
        public void SetMovementSpeed(float _inputSpeed) => m_movementSpeed = _inputSpeed;

        
        //move input
        public void SetAnimationFloat_InputMove(float _inputFloat)
        {
            _animator.SetFloat(_animID_Input_Move, _inputFloat);
        }

        //bools
        public void SetAnimation_Grounded(bool _inputGrounded)
        {
            _animator.SetBool(_animID_Grounded, _inputGrounded);
        }

        public void SetAnimation_FreeFall(bool _inputFreeFall)
        {
            _animator.SetBool(_animID_FreeFall, _inputFreeFall);
        }
        #endregion

    }
}
