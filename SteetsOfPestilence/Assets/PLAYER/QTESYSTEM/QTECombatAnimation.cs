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
        ResetTriggers();
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
        switch(_poiseValue)
        {
            case >= 10:
                break;
            case <= -10:
                break;            
            default:
                PlayAnimation("CombatSequence");
                break;
        }
    }
}
