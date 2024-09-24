using PlayerController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;


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

    /*** Item Selection ***/
    #region Item Selection
    [Header("Item Selection")]
    [SerializeField] int _selectionIndex;
    [SerializeField] int minSelection;
    [SerializeField] int maxSelection;
    [SerializeField] Light[] _lights;
    [SerializeField] SpriteRenderer[] _sprites;



    #endregion


    /*** UI ***/
    #region UI
    [Header("UI")]
    [SerializeField] TMP_Text _interactText;
    [SerializeField] TMP_Text _exitText;
    #endregion

    /*** COMPONENTS ***/
    #region Components
    [HideInInspector] PlayerManager _player;
    [HideInInspector] CameraController _camera;
    #endregion



    #endregion

    /***************************** METHODS ****************************/
    #region Methods

    /*** Awake ***/
    #region Awake

    private void Awake()
    {
        _player = FindObjectOfType<PlayerManager>();
        _camera = FindObjectOfType<CameraController>();
    }

    #endregion

    /*** Trigger ***/
    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //set state
            SetMerchantState(MerchantState.InRange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Set Merchant State
            SetMerchantState(MerchantState.Idle);
        }
    }

    #endregion

    /*** MERCHANT STATE ***/
    #region Merchant State

    public void SetMerchantState(MerchantState inputState)
    {
        switch (inputState)
        {
            case MerchantState.Idle:
                To_Idle();
                break;

            case MerchantState.InRange:
                To_InRange();

                if(_merchantState == MerchantState.InShop)
                    To_InRangeFromInShop();
                
                break;

            case MerchantState.InShop:
                To_InShop();
                break;
        }    
        
        _merchantState = inputState;
    }

    public bool CheckState(MerchantState inputState)
    {
        if (inputState == _merchantState)
            return true;
        else
            return false;
    }

    #endregion

    /*** Transition Methods ***/
    #region Transistion Methods

    // -> InShop
    private void To_InShop()
    {
        //turn off text
        _interactText.gameObject.SetActive(false);

        //turn on exit text
        _exitText.gameObject.SetActive(true);

        //turn on selection ui
        ChangeSelection(0);

        _camera.SetCameraState(CameraController.CameraState.Merchant);
      //  _player.m_input.SetActionMap(InputController.InputMode.Merchant.ToString());
    }

    private void To_InRangeFromInShop()
    {
        _camera.SetCameraState(CameraController.CameraState.FreeLook);
    //    _player.m_input.SetActionMap(InputController.InputMode.Player.ToString());
    }

    private void To_Idle()
    {
        //turn off text
        _interactText.gameObject.SetActive(false);
    }

    private void To_InRange()
    {
        //turn on text
        _interactText.gameObject.SetActive(true);

        //turn off exit text
        _exitText.gameObject.SetActive(false);
    }


    #endregion

    /*** Item Selection ***/
    #region Item Selection

    public void ChangeSelection(int deltaSelection)
    {
        if (deltaSelection < 0 && _selectionIndex == minSelection ||
            deltaSelection > 0 && _selectionIndex == maxSelection)
            return;

        //turn off light and sprite
        _lights[_selectionIndex].gameObject.SetActive(false);
        _sprites[_selectionIndex].gameObject.SetActive(false);

        _selectionIndex += deltaSelection;

        //turn off light and sprite
        _lights[_selectionIndex].gameObject.SetActive(true);
        _sprites[_selectionIndex].gameObject.SetActive(true);
    }




    #endregion



    #endregion



}

