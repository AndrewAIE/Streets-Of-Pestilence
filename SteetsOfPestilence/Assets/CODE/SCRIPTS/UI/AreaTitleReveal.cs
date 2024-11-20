using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTitleReveal : MonoBehaviour
{
    [SerializeField] GameObject _areaTitleCanvas;
    [SerializeField] bool _showAreaTitle = true;
    [SerializeField] bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && _showAreaTitle && !_hasTriggered)
        {
            _areaTitleCanvas.SetActive(true);
            _hasTriggered = true;
        }
    }
}
