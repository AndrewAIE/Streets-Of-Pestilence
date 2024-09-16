using PlayerController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


    public class MerchantController : MonoBehaviour
    {
        /***************************** VARIABLES ****************************/
        #region Variables

        #region Merchant State
        public enum MerchantState
        {
            Idle,
            InRange,
            InShop
        }

        [Header("Merchant State")]
        public MerchantState _merchantState;

        #endregion




        /*** UI ***/
        #region UI
        [Header("UI")]
        [SerializeField] TMP_Text _interactText;

        #endregion

        /*** COMPONENTS ***/
        #region Components
        [HideInInspector] PlayerManager _player;
        [HideInInspector] CameraController _camera;

        #endregion



        #endregion

        private void Awake()
        {
            _player = FindObjectOfType<PlayerManager>();
            _camera = FindObjectOfType<CameraController>();
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //turn on text
                _interactText.gameObject.SetActive(true);

                //set state
                SetMerchantState(MerchantState.InRange);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //turn off text
                _interactText.gameObject.SetActive(false);

                //Set Merchant State
                SetMerchantState(MerchantState.Idle);
            }
        }

        /*** MERCHANT STATE ***/
        #region Merchant State

        public void SetMerchantState(MerchantState inputState)
        {
            _merchantState = inputState;
        }


        #endregion
    }

