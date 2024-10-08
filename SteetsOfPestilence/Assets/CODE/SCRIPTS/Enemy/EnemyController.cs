using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.AI;

namespace EnemyAI
{
    public class EnemyController : MonoBehaviour
    {
        public QTEEncounterData EncounterData;
        private PlayerManager m_player;

        private NavMeshAgent m_agent;
        private EnemyDetector m_detector;

        [SerializeField] private Vector3 m_targetPosition;

        private void Awake()
        {
            m_agent = GetComponentInParent<NavMeshAgent>();
            m_detector = GetComponent<EnemyDetector>();
        }

        private void Update()
        {
            m_targetPosition = m_detector.LastPosition();

            if (m_detector.m_canSeePlayer)
            {
                m_agent.destination = m_targetPosition;
            }


        }

        public void ForceEncounter()
        {
            m_player.EnterCombat(EncounterData, gameObject);
        }
    }
}