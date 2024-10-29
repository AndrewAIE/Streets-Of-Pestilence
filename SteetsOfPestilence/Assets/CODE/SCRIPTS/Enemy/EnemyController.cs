using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.AI;
using Pixelplacement.TweenSystem;

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
        public EnemyType m_EType;

        [SerializeField]private bool isDead = false;
        private bool m_combatEnding;
        public bool Recentering = false;
        #region Nav
        private NavMeshAgent m_agent;
        private EnemyDetector m_detector;
        private float m_defaultStoppingDistance;

        [SerializeField] private Vector3 m_targetPosition;
        [SerializeField] private Vector3[] m_patrolPositions;
        private Vector3 m_homeDestination;
        [SerializeReference] private int m_patrolNum; //which patrol position to go to

        [SerializeReference] private float m_waitTime = 10, m_timer;
        #endregion

        #region Mesh & Particles
        private ParticleSystem[] m_enemyPartycles;
        private SkinnedMeshRenderer m_enemyMesh;
        #endregion



        private void Awake()
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            m_agent = GetComponentInParent<NavMeshAgent>();
            m_detector = GetComponent<EnemyDetector>();
            m_defaultStoppingDistance = m_agent.stoppingDistance;
            m_enemyPartycles = transform.parent.GetComponentsInChildren<ParticleSystem>();
            m_enemyMesh = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();


            m_homeDestination = transform.position;
        }

        int died = 0;
        private void Update()
        {
            if (!isDead && !m_combatEnding)
            {
                if (Recentering)
                {
                    RecenterEnemy(m_player);
                }
                else if (!m_player.PlayerInCombat())
                {
                    if (m_detector.m_canSeePlayer)
                        m_timer = m_waitTime;

                    if (m_timer > 0)
                    {
                        GoToPlayer();
                        m_timer -= Time.deltaTime;
                    }
                    // else
                    //Standby();
                }
            }
        }
        public void KillEnemy()
        {
            m_agent.enabled = false;
            m_enemyMesh.enabled = false;
            for (int i = 0; i <= m_enemyPartycles.Length; i++) m_enemyPartycles[i].Play();
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            StartCoroutine(WaitForDestroy());
        }
        public void EndCombat()
        {
            m_combatEnding = true;
            StartCoroutine(WaitEndCombat());
        }
        private IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(10);
            Destroy(gameObject.transform.parent.gameObject);
        }
        
        private IEnumerator WaitEndCombat()
        {
            yield return new WaitForSeconds(5);
            m_combatEnding = false;
        }

        #region Nav
        private void GoToPlayer()
        {
            m_agent.stoppingDistance = m_defaultStoppingDistance;
            m_targetPosition = m_detector.LastPosition();
            if (m_detector.EnemyIsClose() && !m_player.PlayerInCombat())
            {
                FacePlayer();

                m_player.EnterCombat(m_EncounterData, this);
                return;
            }
            else
            {
                m_agent.destination = m_targetPosition;
            }
        }

        private void Standby()
        {
            m_agent.stoppingDistance = 0;
            if (m_patrolPositions.Length > 0)
            {
                float magnitude = (transform.position - m_patrolPositions[m_patrolNum]).magnitude;
                if (magnitude < 3)
                {
                    m_patrolNum = (m_patrolNum >= m_patrolPositions.Length - 1) ? 0 : m_patrolNum + 1; // either goes to next position or resets by going to first position
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
        #region Encounter

        public void ForceEncounter()
        {
            m_player.EnterCombat(m_EncounterData, this);
        }
        private void RecenterEnemy(PlayerManager player)
        {
            Vector3 centerETP = transform.position + ((player.transform.position - transform.position) / 2); // the center from enemy to player
            Vector3 castPosition = new(transform.position.x, transform.position.y + 1, transform.position.z);
            Vector3 playerPos = player.transform.position;
            Debug.DrawLine(transform.position, centerETP, Color.magenta);

            //make agent stop exactly at point;
            m_agent.stoppingDistance = 0;



            m_agent.destination = new(playerPos.x, playerPos.y, transform.position.z);





        }


        #endregion
    }
}