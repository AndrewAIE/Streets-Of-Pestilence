using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;

public class EnemyController : MonoBehaviour
{
    public QTEEncounterData EncounterData;
    private PlayerManager m_player;

    private EnemyDetector m_detector;
    

    private void Awake()
    {
        m_detector = GetComponent<EnemyDetector>();
    }

    private void Update()
    {
        if (m_detector.m_canSeePlayer)
        {

        }
    }

    public void ForceEncounter()
    {
        m_player.EnterCombat(EncounterData, gameObject);
    }
}
