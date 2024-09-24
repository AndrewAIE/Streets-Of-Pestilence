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

    private void Update()
    {
        
    }

    public void ForceEncounter()
    {
        m_player.EnterCombat(EncounterData, gameObject);
    }
}
