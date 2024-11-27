using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.AI;
using Pixelplacement.TweenSystem;
using System.Net;

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

        private bool m_combatEnding;
        public bool Recentering = false;
        public bool m_deactivated { private set; get; } = false;

        #region Nav
        private NavMeshAgent m_agent;
        private EnemyDetector m_detector;
        /// <summary>
        /// how far away the enemy stops from objects by default (set in NavMeshAgent)
        /// </summary>
        private float m_defaultStoppingDistance;
        /// <summary>
        /// <para>If true <c>Enemy</c> restarts <c>m_patrolPositions</c> from zero </para>
        /// <para>If false <c>Enemy</c> goes backwards through <c>m_patrolPositions</c> after completion</para>
        /// </summary>
        [SerializeField] private bool m_circlePath;
        /// <summary>
        /// the current direction through patrol route the enemy is going
        /// </summary>
        [SerializeField] private bool m_forwardPath = true;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private bool m_waitingAtDestination = false;

        /// <summary>
        /// position of current target
        /// </summary>
        [SerializeField] private Vector3 m_targetPosition;
        /// <summary>
        /// positions of the patrol route (set these manually)
        /// </summary>
        [SerializeField] private Vector3[] m_patrolPositions;
        /// <summary>
        /// where the enemy spawned
        /// </summary>
        private Vector3 m_homeDestination;
        private Vector3 m_homeRotation;

        /// <summary>
        /// the current target for patrol
        /// </summary>
        [SerializeReference] private int m_patrolNum; //which patrol position to go to

        [SerializeReference] private float m_waitTime = 10, m_timer;
        #endregion

        #region CombatVars
        /// <summary>
        /// position that the navmeshagent will try get to during combat
        /// </summary>
        [SerializeReference] private Vector3 m_combatPos;
        public float m_distanceToPlayer { private set; get; } = 2;
        public float m_distancebuffer { private set; get; } = 0.02f;
        [SerializeField] private float m_minWallDistance = 1.5f;
        private bool m_isInCombat = false;
        #endregion

        #region SFX Vars
        private SFXController_Enemy m_enemySFX;
        #endregion

        #region Mesh & Particles & Colliders Vars
        private bool m_particlesPlaying = false;
        private ParticleSystem[] m_enemyParticles;
        private SkinnedMeshRenderer m_enemyMesh;
        private CapsuleCollider m_mainCollider;
        #endregion

        private Animator m_EndGameAnimator;

        private void Awake()
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            m_agent = GetComponentInParent<NavMeshAgent>();
            m_detector = GetComponent<EnemyDetector>();
            m_defaultStoppingDistance = m_agent.stoppingDistance;
            m_enemyParticles = transform.parent.GetComponentsInChildren<ParticleSystem>();
            m_enemyMesh = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
            m_enemySFX = transform.parent.GetComponentInChildren<SFXController_Enemy>();
            m_mainCollider = GetComponent<CapsuleCollider>();
            m_homeDestination = transform.position;
            m_homeRotation = transform.parent.forward;

            m_EndGameAnimator = GameObject.FindGameObjectWithTag("Start and End Blackout").GetComponent<Animator>();
        }
        private void Update()
        {
            if (!m_deactivated && !m_combatEnding)
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
                    {
                        Standby();
                        if (m_waitingAtDestination)
                            transform.parent.forward = Vector3.MoveTowards(transform.parent.forward, m_homeRotation, 10 * Time.deltaTime);
                    }
                }
                else if (m_player.PlayerInCombat() && m_isInCombat) { 
                    FacePlayer();
                    m_agent.destination = m_combatPos;
                }

                
            }
            else if (m_deactivated)
            {
                m_particlesPlaying = false;
                for(int i = 0; i < m_enemyParticles.Length; i++)
                {
                    if (m_enemyParticles[i].isPlaying)
                    {
                        m_particlesPlaying = true;
                    }
                }
            }
        }

        /// <summary>
        /// disables mesh & agent then sets timer to kill parent
        /// </summary>
        public void KillEnemy()
        {
            m_agent.enabled = false;
            m_enemyMesh.enabled = false;
            m_detector.enabled = false;
            m_deactivated = true;
            for (int i = 0; i < m_enemyParticles.Length; i++) m_enemyParticles[i].Play();
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            StartCoroutine(WaitForDestroy());

            if(m_EType == EnemyType.Boss)
            {
                m_EndGameAnimator.SetTrigger("End Trigger");
            }
        }

        public void DisableEnemy()
        {
            m_agent.enabled = false;            
            m_detector.enabled = false;
            m_deactivated = true;
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            StartCoroutine(EnableEnemy());
        }

        public IEnumerator EnableEnemy()
        {
            yield return new WaitForSeconds(5);
            m_agent.enabled = true;            
            m_detector.enabled = true;
            m_deactivated = false;
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = true;
            }
        }
        public void EndCombat()
        {
            m_combatEnding = true;
            m_isInCombat = false;
            StartCoroutine(WaitEndCombat()); 
        }
        /// <summary>
        /// hard coded to wait until particles/
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject.transform.parent.gameObject);
        }
        
        private IEnumerator WaitEndCombat()
        {
            yield return new WaitForSeconds(10);
            m_combatEnding = false;
        }

        #region Nav
        private void GoToPlayer()
        {
            m_waitingAtDestination = false;
            if (m_deactivated)
            {
                return;
            }
            m_agent.stoppingDistance = m_defaultStoppingDistance;
            m_targetPosition = m_detector.LastPosition();
            if (m_detector.EnemyIsClose() && !m_player.PlayerInCombat())
            {
                m_player.EnterCombat(m_EncounterData, this);
                m_isInCombat = true;
                return;
            }
            else
            {
                m_agent.destination = m_targetPosition;
            }
        }

        private void Standby()
        {
            if (m_agent.remainingDistance < 1) m_waitingAtDestination = true;

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
                
                    StartCoroutine(SetPath(waitTime, m_agent.destination = m_patrolPositions[m_patrolNum]));
            }
            else
            {
                if(m_agent.destination != m_homeDestination) 
                    StartCoroutine(SetPath(3, m_agent.destination = m_homeDestination));
            }
        }

        /// <summary>
        /// waits to set path to target after <c>_seconds</c> have passed
        /// </summary>
        /// <param name="_seconds"></param>
        /// <param name="_target"></param>
        /// <returns></returns>
        private IEnumerator SetPath(float _seconds, Vector3 _target)
        {
            yield return new WaitForSeconds(_seconds);
            m_agent.destination = _target;
            m_waitingAtDestination = false;
        }

        /// <summary>
        /// Faces towards player transform
        /// </summary>
        private void FacePlayer()
        {
            // get direction to player ignoring y direction
            Vector3 faceDir = new(m_player.transform.position.x - transform.position.x, 0, m_player.transform.position.z - transform.position.z);
            // normalize
            faceDir = Vector3.Normalize(faceDir);
            transform.parent.forward = faceDir;
        }
        #endregion
        private void RecenterEnemy(PlayerManager player)
        {
            
            //make agent stop exactly at point;
            m_agent.stoppingDistance = 0;

            float distanceBuffer = 0.2f;

            /*Vector3 m_combatPos;*/
            Vector3 playerPos = player.transform.position;

            Vector3 castPosition = new(transform.position.x, transform.position.y + 1, transform.position.z);            

            bool leftHit = Physics.Raycast(castPosition, -transform.right, out RaycastHit leftInfo, m_minWallDistance);
            bool rightHit = Physics.Raycast(castPosition, transform.right, out RaycastHit rightInfo, m_minWallDistance);

            Vector3 leftPos = leftInfo.point;
            Vector3 rightPos = rightInfo.point;

            float zDis = Mathf.Abs(playerPos.z - transform.position.z);
            float xDis = Mathf.Abs(playerPos.x - transform.position.x);

            if (leftHit && rightHit)
            {
                float halfdistance = Vector3.Distance(leftPos, rightPos) / 2f; // get a distance halfway between both points the left and right raycasts hit
                if (leftInfo.distance < halfdistance - distanceBuffer && rightInfo.distance <  halfdistance - distanceBuffer) return; 
                Vector3 centerPoint = (leftPos + rightPos) / 2;  // the point between the two walls
                Debug.Log("enemy in tight space", this);
                m_combatPos = centerPoint; 
            }else if (leftHit)
            {
                Vector3 point = transform.position + transform.right; // a point slightly to the right of the position hit
                Debug.Log("enemy has wall on left", this);
                m_combatPos = point; 
            }else if (rightHit)
            {
                Vector3 point = transform.position - transform.right; // a point slightly left of the position hit
                Debug.Log("enemy has wall on right", this);
                m_combatPos = point;
            }
            else
            {
                m_combatPos = (xDis > zDis) ? new(transform.position.x, playerPos.y, playerPos.z) : new(playerPos.x, playerPos.y, transform.position.z); // if there are no walls, snaps the enemy to the x or y axis
            }

            float distTotal = Vector3.Distance(m_combatPos, playerPos);
            Vector3 Dir;
            if (distTotal > m_distanceToPlayer + m_distancebuffer)
            {
                float dist = (distTotal - m_distanceToPlayer);
                Dir = (playerPos - m_combatPos).normalized;
                m_combatPos += Dir * dist ;
            } 
            if (distTotal < m_distanceToPlayer - m_distancebuffer)
            {
                bool againstWall = Physics.SphereCast(transform.position + m_mainCollider.center, m_mainCollider.radius, -transform.forward, out RaycastHit backHit, 2);

                if (againstWall)
                {
                    Debug.LogWarning("Enemy's back against Wall", this);
                    player.EnableRecenterMovement(); // makes the player back up

                }
                else
                {
                    float dist = m_distanceToPlayer - distTotal;
                    Dir = -transform.forward;
                    m_combatPos += Dir * dist;
                }
                
            }

            m_combatPos.y = transform.position.y;
            m_agent.destination = m_combatPos;
            Recentering = false;
        }
    }
}