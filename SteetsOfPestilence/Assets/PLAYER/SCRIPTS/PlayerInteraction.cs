using Interactables;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerController
{
    public class PlayerInteraction : MonoBehaviour
    {
        private PlayerManager m_player;


        private Image m_interactPanel;
        [SerializeField]private TextMeshProUGUI m_interactPopup;


        private List<Interactable> currentInteractableActions;

        private void Awake()
        {
            m_player = GetComponent<PlayerManager>();
            m_interactPanel = GetComponentInChildren<Image>();
            m_interactPopup = GetComponentInChildren<TextMeshProUGUI>();
            
        }
        private void Start()
        {
            currentInteractableActions = new List<Interactable>();
        }

        public void Interact()
        {
            if (currentInteractableActions.Count == 0)
            {
                return;
            }


            if (currentInteractableActions[0] == null)
            {
                currentInteractableActions.RemoveAt(0);

                return;
            }
            else
            {
                currentInteractableActions[0].Interact(m_player);
            }
        }

        internal void CheckForInteractable()
        {
            if (currentInteractableActions.Count == 0)
            {
                m_interactPopup.text = "";
                m_interactPanel.enabled = false;
                return;
            }
                

            if (currentInteractableActions[0] == null)
            {
                currentInteractableActions.RemoveAt(0);
                
                return;
            }else
            {
                m_interactPanel.enabled = true;
                m_interactPopup.text = currentInteractableActions[0].interactablePrompt;
            }


        }
        private void RefreshInteractionList()
        {
            for(int i = currentInteractableActions.Count -1; i>-1; i--)
            {
                if (currentInteractableActions[i] == null)
                    currentInteractableActions.RemoveAt(i);
            }
        }

        public void addInteraction(Interactable _interact)
        {
            RefreshInteractionList();
            if (!currentInteractableActions.Contains(_interact))
                currentInteractableActions.Add(_interact);

        }
        public void removeInteraction(Interactable _interact)
        {
            RefreshInteractionList();
            if (currentInteractableActions.Contains(_interact))
                currentInteractableActions.Remove(_interact);
        }

    }

}