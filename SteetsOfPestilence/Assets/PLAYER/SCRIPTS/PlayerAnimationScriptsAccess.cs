using PlayerController;
using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationScriptsAccess : MonoBehaviour
{
    PlayerManager m_player;
    QTEManager m_qte;    

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

    public void ReactivatePlayerControl()
    {
        m_player.SetPlayerActive(true);
    }

    public void FadeInUI()
    {
        m_qte.FadeInUI();
    }

    public void FadeOutUI()
    {
        m_qte.FadeOutUI();
    }
}
