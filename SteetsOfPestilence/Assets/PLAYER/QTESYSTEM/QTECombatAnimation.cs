using PlayerController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTECombatAnimation : MonoBehaviour
{
    Animator m_playerAnim;
    Animator m_enemyAnim;   

    public void SetAnimators(Animator _playerAnim, Animator _enemyAnim)
    {
        m_playerAnim = _playerAnim;
        m_enemyAnim = _enemyAnim;

        m_playerAnim.SetLayerWeight(0, 0);
        m_playerAnim.SetLayerWeight(1, 1);

        m_enemyAnim.SetLayerWeight(0, 0);
        m_enemyAnim.SetLayerWeight(1, 1);
    }

    public void PlayAnimation(string _animation)
    {
        m_playerAnim.SetTrigger(_animation);
        m_enemyAnim.SetTrigger(_animation);
    }

    public void RemoveAnimators()
    {
        m_playerAnim.SetLayerWeight(0, 1);
        m_playerAnim.SetLayerWeight(1, 0);

        m_enemyAnim.SetLayerWeight(0, 1);
        m_enemyAnim.SetLayerWeight(1, 0);

        m_playerAnim = null;
        m_enemyAnim = null;
    }


}
