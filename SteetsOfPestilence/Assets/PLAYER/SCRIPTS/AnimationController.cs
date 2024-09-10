using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerController
{
    public class AnimationController : MonoBehaviour
    {
        /**************************************** VARIABLES ******************************/
        #region Variables
        [HideInInspector] private Animator _animator;

        /*** Animation State ***/
        #region Animation State
        public enum RoamingState
        {
            Idle,
            Locomotion,
        }
        public RoamingState _roamingState;

        #endregion

        /*** Idle State ***/
        /*
        #region Idle State
        public enum IdleState
        {
            Default,
            Inspect,
            Scared
        }
        public IdleState _idleState;

        #endregion
        */

        /*** Locomotion State ***/
        #region Locomotion
        [SerializeField] public float _locomotionBlend;


        #endregion

        /*************************** ANIMATION ID'S *************************/
        #region Animation ID's
        //floats
        [HideInInspector] private int _animID_Speed;
        [HideInInspector] private int _animID_Input_Move;
        
        //bools
        [HideInInspector] private int _animID_Grounded;
        [HideInInspector] private int _animID_FreeFall;

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
            _animator = GetComponent<Animator>();
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
            //Idle();
            switch (_roamingState)
            {
                case RoamingState.Idle:
                    //Idle();
                    break;

                case RoamingState.Locomotion:
                    break;
            }
        }

        #endregion

        /*** IDLE ***/
        /*
        private void Idle()
        {
            if (_idleState == IdleState.Default)
            {
                _idleTimer += Time.deltaTime;

                if (_idleTimer >= _idleTriggerTime)
                {
                    if (_lastIdle == _animID_Idle_Inspect)
                    {
                        _animator.SetTrigger("Idle_Scared");
                        _lastIdle = _animID_Idle_Scared;
                    }
                    else
                    {
                        _animator.SetTrigger("Idle_Inspect");
                        _lastIdle = _animID_Idle_Inspect;
                    }


                    _idleTimer = 0;
                    _idleTriggerTime = Random.Range(IdleMinTimer, IdleMaxTimer);
                }
            }
        }

        public void SetIdleState(IdleState state)
        {
            _idleState = state;
        }
        */

        private void AssignAnimationIDs()
        {
            //floats
            _animID_Speed = Animator.StringToHash("Speed");
            _animID_Input_Move = Animator.StringToHash("Move Input");
            
            //bool
            _animID_Grounded = Animator.StringToHash("Grounded");
            _animID_FreeFall = Animator.StringToHash("FreeFall");

            /*//triggers
            _animID_Idle_Inspect = Animator.StringToHash("Idle_Inspect");
            _animID_Idle_Scared = Animator.StringToHash("Idle_Scared");*/
        }

        //speed
        public void SetAnimationFloat_Speed(float _inputSpeed)
        {
            _animator.SetFloat(_animID_Speed, _inputSpeed);
        }
        
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
