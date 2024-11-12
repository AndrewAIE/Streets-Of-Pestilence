using QTESystem;
using System.Collections;
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
        public EnemyType m_EType;

        private bool m_combatEnding;
        public bool Recentering = false;
        public bool m_dying { private set; get; } = false;
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
        [SerializeField] public float m_distanceToPlayer { private set; get; } = 2;
        [SerializeField] public float m_distancebuffer { private set; get; } = 0.02f;
        [SerializeField] private float m_minWallDistance = 1.5f;
        #endregion

        #region Mesh & Particles & Colliders Vars
        private bool m_particlesPlaying;
        private ParticleSystem[] m_enemyParticles;
        private SkinnedMeshRenderer m_enemyMesh;
        private CapsuleCollider m_mainCollider;
        #endregion



        private void Awake()
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            m_agent = GetComponentInParent<NavMeshAgent>();
            m_detector = GetComponent<EnemyDetector>();
            m_defaultStoppingDistance = m_agent.stoppingDistance;
            m_enemyParticles = transform.parent.GetComponentsInChildren<ParticleSystem>();
            m_enemyMesh = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
            m_mainCollider = GetComponent<CapsuleCollider>();
            m_homeDestination = transform.position;
        }
        public Vector3 destination;
        public float distance;
        private void Update()
        {
            destination = m_agent.destination;
            distance = Vector3.Distance(m_combatPos, m_player.transform.position);
            /*RecenterEnemy(m_player);
            FacePlayer();
            return;*/
            if (!m_dying && !m_combatEnding)
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
                }
                else if (m_player.PlayerInCombat()) { FacePlayer();
                    m_agent.destination = m_combatPos;
                }
            }else if (m_dying)
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
            m_dying = true;
            for (int i = 0; i < m_enemyParticles.Length; i++) m_enemyParticles[i].Play();
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
        /// <summary>
        /// hard coded to wait until particles/
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForDestroy()
        {
            yield return new WaitForSeconds(5);

            Debug.Log("Destroying Enemy", this);
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
            if (m_dying)
            {
                return;
            }
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
                float halfdistance = Vector3.Distance(leftPos, rightPos) / 2f;
                if (leftInfo.distance < halfdistance - distanceBuffer && rightInfo.distance <  halfdistance - distanceBuffer) return;
                Vector3 centerPoint = (leftPos + rightPos) / 2;
                m_combatPos = centerPoint;
            }else if (leftHit)
            {
                Vector3 point = transform.position + transform.right;
                
                m_combatPos = point;
            }else if (rightHit)
            {
                Vector3 point = transform.position - transform.right;
                
                m_combatPos = point;
            }
            else
            {
                m_combatPos = (xDis > zDis) ? new(transform.position.x, playerPos.y, playerPos.z) : new(playerPos.x, playerPos.y, transform.position.z);
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
                Debug.DrawRay(transform.position + m_mainCollider.center, -transform.forward, Color.blue,2f);
                bool againstWall = Physics.SphereCast(transform.position + m_mainCollider.center, m_mainCollider.radius, -transform.forward, out RaycastHit backHit, 2); //m_distanceToPlayer - (m_distanceToPlayer - distTotal));

                if (againstWall)
                {
                    Debug.LogWarning("Enemy against Wall", this);
                    player.EnableRecenterMovement();

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
        #endregion
    }
}