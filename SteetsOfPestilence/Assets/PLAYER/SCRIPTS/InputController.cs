using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
	public class InputController : MonoBehaviour
	{
        /**************************************** VARIABLES ************************************/
        #region Variables
        [Header("Character Input Values")]
        //vector 2 showing the players movement input
        [Tooltip("vector 2 showing the players movement input")]
        [SerializeField] public Vector2 move;

		//vector 2 showing the players look input
		[Tooltip("vector 2 showing the players look input")]
		[SerializeField] public Vector2 look;

		[Tooltip("A bool that controls the interact function")]
		[SerializeField] public bool interact;

		[Tooltip("")]
		[SerializeField] public bool recenter;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;

		[HideInInspector] PlayerManager _manager;

        #endregion

        /**************************************** METHODS ************************************/
        #region Methods

        private void Awake()
        {
			_manager = GetComponent<PlayerManager>();
        }

        /**************************************** ON ____ *************************************/
        #region On ____

        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			LookInput(value.Get<Vector2>());
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}

		public void OnRecenter(InputValue value)
		{
			RecenterInput(value.isPressed);
		}

        #endregion

        /************************************** ____ Input ************************************/
        #region ____ Input

        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;

			//animation
			/*_manager._animation.SetAnimationFloat_InputMove(move.magnitude);

			if(move.magnitude > 0)
			{
				_manager._animation._roamingState = AnimationController.RoamingState.Locomotion;
			}
			else
			{
                _manager._animation._roamingState = AnimationController.RoamingState.Idle;
            }*/
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		} 

		public void InteractInput(bool newInput)
		{
			interact = newInput;
		}

		public void RecenterInput(bool newInput)
		{
			recenter = newInput;

			if (recenter)
				_manager._camera.TriggerRecenter();
		}

        #endregion

        /**************************************** INFRASTRUCTURE ************************************/
        #region Infrastructure

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

        #endregion

        #endregion
    }
}