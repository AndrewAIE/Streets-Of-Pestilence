using Interactables;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerController
{
    public class PlayerUI : MonoBehaviour
    {
        private PlayerManager m_player;
        [Header("Death")]
        [SerializeField] public Animator _deathScreen;

        [Header("Interaction")]
        [SerializeField] private Image m_interactPanel;
        [SerializeField]private TextMeshProUGUI m_interactPopup;


        private List<Interactable> currentInteractableActions;

        private void Awake()
        {
            m_player = GetComponentInParent<PlayerManager>();
        }
        private void Start()
        {
            currentInteractableActions = new List<Interactable>();
        }

        public void TriggerDeathScreenOn()
        {
            _deathScreen.SetTrigger("Death Screen On Trigger");
        }

        public void TriggerDeathScreenOff()
        {
            Debug.Log("Trigger Death Screen off");
            _deathScreen.SetTrigger("Death Screen Off Trigger");
        }

        public void CallRespawnPlayer()
        {
            GetComponentInParent<PlayerManager>().StartRespawn();
        }

        public void CallSetPlayerActive()
        {
            GetComponentInParent<PlayerManager>().SetPlayerActive();
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

                foreach(Interactable interact in currentInteractableActions)
                {
                    Vector3 otherPos = interact.transform.position;
                    Vector3 originPos = m_player.transform.position;
                    originPos.y += 1;
                    otherPos.y += 1;

                    Vector3 direction = otherPos - originPos;
                    float angle = Vector3.Angle(direction, transform.forward);

                }
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