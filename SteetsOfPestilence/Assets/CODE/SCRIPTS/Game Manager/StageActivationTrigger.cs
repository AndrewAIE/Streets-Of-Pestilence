using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageActivationTrigger : MonoBehaviour
{
    public GameObject[] StagesToActivate;
    public GameObject[] StagesToDeactivate;   

    private void OnTriggerEnter(Collider other)
    {        
        if(other.CompareTag("Player"))
        {
            foreach (GameObject go in StagesToActivate)
            {
                go.SetActive(true);
            }

            foreach (GameObject go in StagesToDeactivate)
            {
                go.SetActive(false);
            }
        }
    }
}
