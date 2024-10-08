using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;

namespace EnemyAI
{
    public class EnemyDetector : MonoBehaviour
    {
        [SerializeField] private float m_viewRadius = 10;
        [SerializeField] private float m_viewAngle = 50;
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
            Vector3 direction = otherPos - originPos;
            if (direction.magnitude > m_viewRadius) return false;
            Debug.Log("in range");

            angle = Mathf.Abs(Vector3.Angle(direction, transform.forward));

            if (angle < m_viewAngle)
            {
                Physics.Raycast(originPos, direction, out RaycastHit hitInfo, m_viewRadius, m_obstructMask);
                Debug.DrawRay(originPos, direction, Color.magenta, m_viewRadius);
                if (hitInfo.transform.gameObject == m_player)
                {
                    m_lastKnownPos = hitInfo.transform.position;
                    return true;
                }
            }

            return false;
        }

        internal Vector3 LastPosition()
        {
            return m_lastKnownPos;
        }

    }
}
