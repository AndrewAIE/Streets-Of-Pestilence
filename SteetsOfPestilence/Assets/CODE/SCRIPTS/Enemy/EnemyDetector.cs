using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private float m_viewRadius = 10;
    [SerializeField] private float m_viewAngle = 50;
    [SerializeField] float angle;
    public LayerMask m_obstructMask;
    public bool m_canSeePlayer { get; private set; }

    private Vector3 m_targetLastKnownPos;
    private GameObject m_player;

    private void Awake()
    {
        m_player = FindObjectOfType<PlayerManager>().gameObject;
    }
    private void Update()
    {
        m_canSeePlayer = FieldOfViewCheck();
    }
    [SerializeField] Vector3 direction;
    private bool FieldOfViewCheck()
    {
        Vector3 otherPos = m_player.transform.position;
        direction = otherPos - transform.position;
        if (direction.magnitude > m_viewRadius) return false;
        Debug.Log("in range");

        angle = Vector3.Angle(direction, transform.forward);

        if (angle < m_viewAngle)
        {
            Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, m_viewRadius, m_obstructMask);
            if (hitInfo.transform.gameObject.GetComponent<PlayerManager>())
            {
                return true;
            }
        }

        return false;
    }
}
