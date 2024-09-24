using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
	public class InputController : MonoBehaviour
	{
		/**************************************** VARIABLES ************************************/
		#region Variables
		[Header("Input Mode")]
		[SerializeField] public InputMode _inputMode;
		public enum InputMode
		{
			Player,
			Merchant
		}

        [Header("Player")]
        #region Player
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

        #endregion

        [Header("Merchant")]
        #region Merchant

        [SerializeField] public bool exit;
		[SerializeField] public Vector2 moveSelection;

        #endregion

        [Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;

		[HideInInspector] PlayerManager _manager;
		[HideInInspector] 

        #endregion

        /**************************************** METHODS ****************************************/
        #region Methods

        private void Awake()
        {
			_manager = GetComponent<PlayerManager>();
			
        }


        /*** Player ***/
        #region Player

        /*** ON ____ ***/
        #region On ____

        public void OnMove(InputValue value)
		{
			if(_manager._movement._canMove)
				MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			LookInput(value.Get<Vector2>());
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);

			//if (_manager.CheckMerchantState(MerchantController.MerchantState.InRange))
			//	_manager.SetMerchantState(MerchantController.MerchantState.InShop);
		}

		public void OnRecenter(InputValue value)
		{
			RecenterInput(value.isPressed);
		}

        #endregion

        /*** ____ Input ***/
        #region ____ Input

        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;

			//animation
			_manager._animation.SetAnimationFloat_InputMove(move.magnitude);
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

			//if (recenter)
				//_manager._camera.TriggerRecenter();
		}


        #endregion

        #endregion

        /*** Merchant ***/
        #region Merchant

        /*** On ___ ***/
        #region On ___

        public void OnExit(InputValue value)
        {
			ExitInput(value.isPressed);

			
        }

		public void OnMoveSelection(InputValue value)
		{
			MoveSelectionInput(value.Get<Vector2>());
		}

        #endregion

        /*** ___ Input ***/
        #region ___ Input

        public void ExitInput(bool newInput)
		{
			exit = newInput;

          //  _manager.SetMerchantState(MerchantController.MerchantState.InRange);
        }

		public void MoveSelectionInput(Vector2 newMoveSelection)
		{
			moveSelection = newMoveSelection;

            if (Mathf.Abs(moveSelection.x) > 0.9f)
            {
                //_manager._merchants.ChangeSelection(Mathf.RoundToInt(moveSelection.x));
            }
        }



        #endregion

        #endregion

        /*** INFRASTRUCTURE ***/
        #region Infrastructure

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		public void SetActionMap(string newMap)
		{
			//_playerInput.SwitchCurrentActionMap(newMap);

			if(newMap == "Player")
			{
				_inputMode = InputMode.Player;
			}
			else if(newMap == "Merchant")
			{
				_inputMode = InputMode.Merchant;
			}
		}

        #endregion

        #endregion
    }
}