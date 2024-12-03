using UnityEngine;
using PlayerController;

namespace EnemyAI
{
    public class EnemyDetector : MonoBehaviour
    {
        /// <summary>
        /// how far the enemy can see in the viewAngle
        /// </summary>
        [SerializeField] private float m_viewRadius = 10;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private float m_viewAngle = 90;
        [SerializeField] private float combatDistance = 2f;
        [SerializeField] float m_angle;
        public LayerMask m_obstructMask;
        [SerializeField]public bool m_canSeePlayer;// { get; private set; }

        private GameObject m_player;
        private Vector3 m_lastKnownPos;

        private void Awake()
        {
            m_player = FindObjectOfType<PlayerManager>().gameObject;
        }
        private void Update()
        {
            m_canSeePlayer = FieldOfViewCheck();              
        }

        private bool FieldOfViewCheck()
        {
            Vector3 originPos = transform.position;
            originPos.y = 0;
            Vector3 otherPos = m_player.transform.position;
            otherPos.y = 0;

            float distance = Vector3.Distance(originPos, otherPos);
            if (distance > m_viewRadius) return false;
            
            Vector3 direction = (otherPos - originPos).normalized;
            
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), direction, Color.yellow);
            m_angle = Vector3.Angle(direction, transform.forward);
            if (m_angle < m_viewAngle && m_player)
            {
                originPos.y = transform.position.y + 1;
                Ray ray = new Ray(originPos, direction);
                Physics.Raycast(ray, out RaycastHit hitInfo, m_viewRadius, m_obstructMask);

                Debug.DrawRay(ray.origin, ray.direction * m_viewRadius, Color.yellow);

                if (hitInfo.transform.gameObject == m_player)
                {
                    m_lastKnownPos = hitInfo.transform.position;
                    return true;
                }
            }
            return false;
        }

        internal bool EnemyIsClose()
        {
            Vector3 originPos = transform.position;
            Vector3 otherPos = m_player.transform.position;
            float distance = Vector3.Distance(originPos, otherPos);

            Vector3 direction = (otherPos - originPos).normalized;

            bool sphereHit = Physics.SphereCast(originPos, .5f, direction, out RaycastHit hitInfo, combatDistance);

            if (distance < combatDistance)
            {
                return true;
            }

            return false;
        }
        internal Vector3 LastPosition()
        {
            return m_lastKnownPos;
        }

    }
}
