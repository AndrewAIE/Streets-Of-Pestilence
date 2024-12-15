using PlayerController;
using QTESystem;
using UnityEngine;

public class PlayerAnimationScriptsAccess : MonoBehaviour
{
    PlayerManager m_player;
    QTEManager m_qte;
    public SFXController_Player m_SFX;

    void Awake()
    {
        m_player = GetComponentInParent<PlayerManager>();
        m_qte = m_player.GetComponentInChildren<QTEManager>();
    }

    public void ResetAnimationState()
    {
        m_qte.ResetAnimationState();
    }

    public void EnterSloMo()
    {
        m_qte.SlowTime(true);        
    }

    public void ExitSloMo()
    {
        m_qte.SlowTime(false);
    }

    public void PlayerDeath()
    {        
        m_player.KillPlayer();
    }

    public void FadeInUI()
    {
        m_qte.FadeInUI();
    }

    public void FadeOutUI()
    {
        m_qte.FadeOutUI();
    }

    public void PlaySFX_RunStone()
    {
        m_SFX.Play_Footstep_Run_Stone();
    }

    public void PlaySFX_WalkStone()
    {
        m_SFX.Play_Footstep_Walk_Stone();
    }

    public void PlaySFX_LightSwing()
    {
        m_SFX.Play_LightSwing();
    }

    public void PlaySFX_HeavySwing()
    {
        m_SFX.Play_HeavySwing();
    }

    public void PlaySFX_MetalClash()
    {
        m_SFX.Play_MetalClash();
    }

    public void PlaySFX_LightHit()
    {
        m_SFX.Play_LightHit();
    }

    public void PlaySFX_HeavyHit()
    {
        m_SFX.Play_HeavyHit();
    }

    public void ExitCombat()
    {
        m_qte.ReactivatePlayer();
        m_player.EndCombat();
    }

    public void PlaySFX_Grunt()
    {
       m_SFX.Play_Player_Grunt();
    }

    public void PlaySFX_LongGrunt()
    {
        m_SFX.Play_Player_LongGrunt();
    }

   
}
