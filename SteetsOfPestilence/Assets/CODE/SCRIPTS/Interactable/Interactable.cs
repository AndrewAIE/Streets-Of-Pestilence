using UnityEngine;
using PlayerController;

namespace Interactables
{

    /// <summary>
    /// base class for all interactable object (doors, levers, checkpoints)
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public string interactablePrompt;
        public float m_angle;
        /// <summary>
        /// the interactable trigger of the GameObject
        /// </summary>
        [SerializeField] protected Collider interactableTrigger;
        
        protected virtual void Awake()
        {
            if (interactableTrigger == null)
                interactableTrigger = GetComponent<Collider>();
        }

        public virtual void Interact(PlayerManager player)
        {

        }

        public virtual void OnTriggerEnter (Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player == null) return;

            player.m_playerUI.addInteraction(this);
        }
        public virtual void OnTriggerExit (Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player == null) return;

            player.m_playerUI.removeInteraction(this);
        }


    }
}