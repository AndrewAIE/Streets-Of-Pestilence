using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    public class Interactor : MonoBehaviour
    {
        PlayerManager m_player;

        private List<Interactable> currentInteractableActions;

        private void Awake()
        {
            m_player = GetComponent<PlayerManager>();
        }


    }

}