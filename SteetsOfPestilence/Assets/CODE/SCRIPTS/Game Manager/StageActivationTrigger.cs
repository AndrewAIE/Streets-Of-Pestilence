using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageActivationTrigger : MonoBehaviour
{
    public GameObject StageToActivate;
    public GameObject StageToDeactivate;   

    private void OnTriggerEnter(Collider other)
    {        
        if(other.CompareTag("Player") && StageToActivate.activeInHierarchy == false)
        {
            StageToActivate.SetActive(true);
            StageToDeactivate.SetActive(false);
        }
    }
}
