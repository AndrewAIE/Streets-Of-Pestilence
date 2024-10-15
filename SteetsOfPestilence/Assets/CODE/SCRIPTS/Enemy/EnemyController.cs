using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.AI;

namespace EnemyAI
{
    public class EnemyController : Entity
    {
        public QTEEncounterData m_EncounterData;
        private PlayerManager m_player;

        private NavMeshAgent m_agent;
        private EnemyDetector m_detector;

        [SerializeField] private Vector3 m_targetPosition;

        private void Awake()
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            m_agent = GetComponentInParent<NavMeshAgent>();
            m_detector = GetComponent<EnemyDetector>();
        }

        private void Update()
        {
            GoToPlayer();
        }

        private void GoToPlayer()
        {
            m_targetPosition = m_detector.LastPosition();

            if (m_detector.m_canSeePlayer)
            {
                m_agent.destination = m_targetPosition;

                if (m_detector.EnemyIsClose() && !m_player.PlayerInCombat())
                {
                    m_player.EnterCombat(m_EncounterData, gameObject);
                }
            }
        }

        public void ForceEncounter()
        {
            m_player.EnterCombat(m_EncounterData, gameObject);
        }
    }
}