using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;

public class EnemyDetector : MonoBehaviour
{
    EnemyController m_enemy;

    private void Awake()
    {
        m_enemy = transform.parent.GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerManager player = other.gameObject.GetComponent<PlayerManager>();
            
            player.EnterCombat(m_enemy.EncounterData, m_enemy.transform.parent.gameObject);                
        }
    }
}
