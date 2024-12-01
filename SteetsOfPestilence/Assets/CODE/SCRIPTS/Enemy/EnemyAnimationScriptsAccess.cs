using EnemyAI;
using UnityEngine;

namespace EnemyAi
{
    public class EnemyAnimationScriptsAccess : MonoBehaviour
    {
        EnemyController m_enemyController;

        private void Awake()
        {
            m_enemyController = transform.parent.parent.GetComponentInChildren<EnemyController>();
        }

        public void KillEnemy()
        {
            m_enemyController.KillEnemy();
        }
    }
}



