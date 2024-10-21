using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.AI;

namespace EnemyAI
{
    public enum EnemyType
    {
        Rabbit,
        Rat,
        Dog,
        Boss
    }


    public class EnemyController : Entity
    {
        public QTEEncounterData m_EncounterData;
        private PlayerManager m_player;
        public EnemyType m_EType { get; private set; }


        #region Nav
        private NavMeshAgent m_agent;
        private EnemyDetector m_detector;

        [SerializeField] private Vector3 m_targetPosition;
        [SerializeField] private Vector3[] m_patrolPositions;
        private Vector3 m_homeDestination;
        [SerializeReference]private int m_patrolNum; //which patrol position to go to

        [SerializeReference] private float m_waitTime = 10, m_timer;
        #endregion



        private void Awake()
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            m_agent = GetComponentInParent<NavMeshAgent>();
            m_detector = GetComponent<EnemyDetector>();

            m_homeDestination = transform.position;
        }

        private void Update()
        {
            if (m_detector.m_canSeePlayer)
                m_timer = m_waitTime;

            if (m_timer > 0)
            {
                GoToPlayer();
                m_timer -= Time.deltaTime;
            }
            else
                Standby();
        }
        private void OnDestroy()
        {
            Destroy(gameObject.transform.parent.gameObject);
        }

        #region Nav
        private void GoToPlayer()
        {
            m_targetPosition = m_detector.LastPosition();
            if (m_detector.EnemyIsClose() && !m_player.PlayerInCombat())
            {
                FacePlayer();
                m_player.EnterCombat(m_EncounterData, gameObject);
                return;
            }
            else if (!m_player.PlayerInCombat())
            {
                m_agent.destination = m_targetPosition;
            }
        }

        private void Standby()
        {
            if (m_patrolPositions.Length > 0)
            {
                float magnitude = (transform.position - m_patrolPositions[m_patrolNum]).magnitude;
                if(magnitude < 3)
                {
                    m_patrolNum = (m_patrolNum >= m_patrolPositions.Length-1) ? 0 : m_patrolNum+1;
                }

                m_agent.destination = m_patrolPositions[m_patrolNum];



            }
            else
                m_agent.destination = m_homeDestination;
        }

        private void FacePlayer()
        {

        }
        #endregion

        public void ForceEncounter()
        {
            m_player.EnterCombat(m_EncounterData, gameObject);
        }
    }
}