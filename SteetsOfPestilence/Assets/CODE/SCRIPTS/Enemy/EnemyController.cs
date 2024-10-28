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
        public bool recenter;
        #region Nav
        private NavMeshAgent m_agent;
        private EnemyDetector m_detector;

        [SerializeField] private Vector3 m_targetPosition;
        [SerializeField] private Vector3[] m_patrolPositions;
        private Vector3 m_homeDestination;
        [SerializeReference]private int m_patrolNum; //which patrol position to go to

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

            m_enemyPartycles = transform.parent.GetComponentsInChildren<ParticleSystem>();
            m_enemyMesh = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();


            m_homeDestination = transform.position;
        }

        int died = 0;
        private void Update()
        {
            if (!isDead)
            {
                if (!m_player.PlayerInCombat())
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
            }
            else if (died < 1) KillEnemy();
        }
        public void KillEnemy()
        {
            died++;
            m_agent.enabled = false;
            m_enemyMesh.enabled = false;
            for (int i = 0; i <= m_enemyPartycles.Length; i++) m_enemyPartycles[i].Play();
            Collider[] colliders = GetComponents<Collider>();
            foreach(Collider col in colliders)
            {
                col.enabled = false;
            }
            StartCoroutine(WaitForDestroy());
        }

        private IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(10);
            Destroy(gameObject.transform.parent.gameObject);
        }


        #region Nav
        private void GoToPlayer()
        {
            m_targetPosition = m_detector.LastPosition();
            if (m_detector.EnemyIsClose() && !m_player.PlayerInCombat())
            {
                FacePlayer();
                RecenterEnemy();
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
        #region Encounter

        public void ForceEncounter()
        {
            m_player.EnterCombat(m_EncounterData, this);
        }
        public void RecenterEnemy()
        {
            float range = 2f;
            bool leftHit = Physics.SphereCast(transform.position, 1, Vector3.left, out RaycastHit leftInfo, range);
            bool rightHit = Physics.SphereCast(transform.position, 1, Vector3.right, out RaycastHit rightInfo, range);


        }


        #endregion
    }
}