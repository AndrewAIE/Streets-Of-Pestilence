using Interactables;
using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Interactables
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player == null) return;

            player.UnlockSpawn(transform.position, transform.rotation);
        }
    }
}