using System.Collections;
using System.Collections.Generic;
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


        }
        public virtual void OnTriggerExit (Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player == null) return;



        }


    }
}