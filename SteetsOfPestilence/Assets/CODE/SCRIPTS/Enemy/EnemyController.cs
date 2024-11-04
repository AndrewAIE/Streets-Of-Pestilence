using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.AI;
using Pixelplacement.TweenSystem;
using UnityEditorInternal;

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
        [SerializeField] private bool m_circlePath, m_forwardPath = true, m_waitingAtDestination = false;

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

        private void Update()
        {
            /*RecenterEnemy(m_player);
            FacePlayer();
            return;*/
            if (!isDead && !m_combatEnding)
            {
                if (Recentering)
                {
                    FacePlayer();
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
                    else
                        Standby();
                }else if (m_player.PlayerInCombat()) FacePlayer();
            }
        }
        public void KillEnemy()
        {
            m_agent.enabled = false;
            m_enemyMesh.enabled = false;
            for (int i = 0; i < m_enemyPartycles.Length; i++) m_enemyPartycles[i].Play();
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
            if (m_waitingAtDestination) return;
            if (m_patrolPositions.Length > 0)
            {
                if (m_circlePath)
                {
                    float magnitude = (transform.position - m_patrolPositions[m_patrolNum]).magnitude;
                    if (magnitude < 3)
                    {
                        m_patrolNum = (m_patrolNum >= m_patrolPositions.Length - 1) ? 0 : m_patrolNum + 1; // either goes to next position or resets by going to first position
                    }
                }
                else if (m_agent.remainingDistance < 2)
                {
                    m_patrolNum = (m_forwardPath) ? m_patrolNum + 1 : m_patrolNum - 1;

                    if (m_patrolNum >= m_patrolPositions.Length - 1) m_forwardPath = false;
                    else if (m_patrolNum <= 0) m_forwardPath = true;
                }
                float waitTime = Random.Range(0, 3);
                
                    m_waitingAtDestination = true;
                    StartCoroutine(SetPath(waitTime, m_agent.destination = m_patrolPositions[m_patrolNum]));
            }
            else
            {
                m_waitingAtDestination = true;
                StartCoroutine(SetPath(3, m_agent.destination = m_homeDestination));
            }
        }

        private IEnumerator SetPath(float _seconds, Vector3 _target)
        {
            yield return new WaitForSeconds(_seconds);
            m_agent.destination = _target;
            m_waitingAtDestination = false;
        }

        private void FacePlayer()
        {
            transform.parent.forward = (m_player.transform.position - transform.position).normalized;
        }
        #endregion
        #region Encounter

        public void ForceEncounter()
        {
            m_player.EnterCombat(m_EncounterData, this);
        }


        private void RecenterEnemy(PlayerManager player)
        {
            //make agent stop exactly at point;
            m_agent.stoppingDistance = 0;

            float distanceBuffer = 0.2f;

            Vector3 finalPos;
            Vector3 playerPos = player.transform.position;

            Vector3 castPosition = new(transform.position.x, transform.position.y + 1, transform.position.z);

            //Debug.DrawLine(transform.position, centerETP, Color.magenta);
            

            bool leftHit = Physics.Raycast(castPosition, -transform.right, out RaycastHit leftInfo, 2f);
            bool rightHit = Physics.Raycast(castPosition, transform.right, out RaycastHit rightInfo, 2f);

            Vector3 leftPos = leftInfo.point;
            Vector3 rightPos = rightInfo.point;

            float zDis = Mathf.Abs(playerPos.z - transform.position.z);
            float xDis = Mathf.Abs(playerPos.x - transform.position.x);

            if (leftHit && rightHit)
            {
                float halfdistance = Vector3.Distance(leftPos, rightPos) / 2f;
                if (leftInfo.distance < halfdistance - distanceBuffer && rightInfo.distance <  halfdistance - distanceBuffer) return;

                Vector3 centerPoint = (leftPos + rightPos) / 2;

                finalPos = centerPoint;
            }else if (leftHit)
            {
                Vector3 point = transform.position + (transform.right * 2);

                finalPos = point;
            }else if (rightHit)
            {
                Vector3 point = (xDis > zDis) ? new(transform.position.x, playerPos.y, playerPos.z - 1) : new(playerPos.x + 1, playerPos.y, transform.position.z);

                finalPos = point;
            }
            else
            {
                finalPos = (xDis > zDis) ? new(transform.position.x, playerPos.y, playerPos.z) : new(playerPos.x, playerPos.y, transform.position.z);
            }

            m_agent.destination = finalPos;
            Recentering = false;
        }


        #endregion
    }
}