using EnemyAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAi
{
    public class EnemyAnimationScriptsAccess : MonoBehaviour
    {
        EnemyController m_enemyController;

        private void Awake()
        {
            m_enemyController = transform.parent.GetComponentInChildren<EnemyController>();
        }

        public void KillEnemy()
        {
            m_enemyController.KillEnemy();
        }


    }
}



