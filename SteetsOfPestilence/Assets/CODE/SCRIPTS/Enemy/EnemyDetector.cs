using UnityEngine;
using PlayerController;

namespace EnemyAI
{
    public class EnemyDetector : MonoBehaviour
    {
        [SerializeField] private float m_viewRadius = 10;
        [SerializeField] private float m_viewAngle = 40;
        [SerializeField] private float combatDistance = 2f;
        [SerializeField] float angle;
        public LayerMask m_obstructMask;
        public bool m_canSeePlayer { get; private set; }

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
            originPos.y += 1;
            Vector3 otherPos = m_player.transform.position;
            otherPos.y += 1;
            Vector3 direction = otherPos - originPos;
            Debug.DrawRay(originPos, direction, Color.magenta, .1f);
            if (direction.magnitude > m_viewRadius) return false;

            angle = Vector3.Angle(direction, transform.forward);
            
            if (angle < m_viewAngle)
            {
                Physics.Raycast(originPos, direction, out RaycastHit hitInfo, m_viewRadius, m_obstructMask);
                
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
            dis = distance;
            if (distance < combatDistance)
            {
                return true;
            }

            return false;
        }
        public float dis;
        internal Vector3 LastPosition()
        {
            return m_lastKnownPos;
        }

    }
}
