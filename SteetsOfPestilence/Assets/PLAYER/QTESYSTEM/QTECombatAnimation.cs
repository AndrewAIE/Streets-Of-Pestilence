using PlayerController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class QTECombatAnimation : MonoBehaviour
{
    Animator m_playerAnim;
    Animator m_enemyAnim;
    private int m_index;
    public bool EndState;

    public void SetQTEAnimations(Animator _playerAnim, Animator _enemyAnim)
    {
        m_playerAnim = _playerAnim;
        m_enemyAnim = _enemyAnim;
        PlayAnimation("QTECombat");
    }

    public void PlayAnimation(string _animation)
    {
        m_playerAnim.SetTrigger(_animation);
        m_enemyAnim.SetTrigger(_animation);
    }

    public void ResetTriggers()
    {
        foreach(var parameter in m_playerAnim.parameters)
        {
            if(parameter.type == AnimatorControllerParameterType.Trigger)
            {
                m_playerAnim.ResetTrigger(parameter.name);
                m_enemyAnim.ResetTrigger(parameter.name);
            }
        }        
    }

    public void ResetAnimators()
    {
        m_playerAnim = null;
        m_enemyAnim = null;
    }

    public void SelectAnimation(int _poiseValue)
    {
        EndState = false;
        m_index++;
        if (m_index >= 2)
            m_index = 0;
        switch(_poiseValue)
        {
            case < -4:
                if(m_index == 0)
                {
                    PlayAnimation("EnemyAdvantageOne");
                    break;
                }
                PlayAnimation("EnemyAdvantageTwo");
                    break;
            case > 4:
                if (m_index == 0)
                {
                    PlayAnimation("PlayerAdvantageOne");
                    break;
                }
                PlayAnimation("PlayerAdvantageTwo");
                break;
            default:
                if (m_index == 0)
                {
                    PlayAnimation("NeutralOne");
                    break;
                }
                PlayAnimation("NeutralTwo");
                break;
        }
    }
    
    public void ExitState()
    {
        ResetTriggers();
        EndState = true;
    }
}
