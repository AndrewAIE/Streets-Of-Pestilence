using EnemyAI;
using PlayerController;
using UnityEngine;

namespace EnemyAi
{
    public class EnemyAnimationScriptsAccess : MonoBehaviour
    {
        EnemyController m_enemyController;
        SFXController_Enemy m_enemySFXManager;
        SFXController_Player m_playerSFXManager;

        /*[SerializeField] private AudioClip _lastPlayedClip_Idle;
        [SerializeField] private AudioClip _lastPlayedClip_Burn;
        [SerializeField] private AudioClip _lastPlayedClip_MidCombat;
        [SerializeField] private AudioClip _lastPlayedClip_OnAttack;
        [SerializeField] private AudioClip _lastPlayedClip_OnDeath;
        [SerializeField] private AudioClip _lastPlayedClip_OnDefeatPlayer;
        [SerializeField] private AudioClip _lastPlayedClip_OnLoseTrackOfPlayer;
        [SerializeField] private AudioClip _lastPlayedClip_OnSpotPlayer;
        [SerializeField] private AudioClip _lastPlayedClip_OnSurprisePlayer;*/

        private void Awake()
        {
            m_enemyController = transform.parent.parent.GetComponentInChildren<EnemyController>();
            m_enemySFXManager = transform.parent.transform.parent.GetComponentInChildren<SFXController_Enemy>();
            m_playerSFXManager = FindObjectOfType<SFXController_Player>();
        }            

        public void KillEnemy()
        {
            m_enemyController.KillEnemy();
        }

        public void PlaySFX_FootstepWalk()
        {
            m_playerSFXManager.Play_Footstep_Walk_Stone();
        }

        public void PlaySFX_FootstepRun()
        {
            m_playerSFXManager.Play_Footstep_Run_Stone();
        }

        public void PlaySFX_LightSwing()
        {
            m_playerSFXManager.Play_LightSwing();
        }

        public void PlaySFX_HeavySwing()
        {
            m_playerSFXManager.Play_HeavySwing();
        }

        public void PlaySFX_Burn()
        {
            m_enemySFXManager.Play_Enemy_Burn();
        }

        public void PlaySFX_MidCombat()
        {
            m_enemySFXManager.Play_Enemy_MidCombat();
        }

        public void PlaySFX_OnAttack()
        {
            m_enemySFXManager.Play_Enemy_OnAttack();
        }

        public void PlaySFX_OnDeath()
        {
            m_enemySFXManager.Play_Enemy_OnDeath();
        }

        public void PlaySFX_OnDefeatPlayer()
        {
            m_enemySFXManager.Play_Enemy_OnDefeatPlayer();
        }

        public void PlaySFX_LightHit()
        {
            m_playerSFXManager.Play_LightHit();
        }

        public void PlaySFX_HeavyHit()
        {
            m_playerSFXManager.Play_HeavyHit();
        }

    }
}



