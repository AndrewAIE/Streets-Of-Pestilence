using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTESystem;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Mash Action")]
public class QTEMashAction : QTEAction
{
    [SerializeField]
    private int m_mashTarget;
    private int m_mashCount = 0;
    [SerializeField]
    private float m_timePerPress;
    protected override ActionState onUpdate()
    {
        if(m_timer >= m_timeLimit + (m_timePerPress * m_mashTarget) && m_state == ActionState.running)
        {
            m_timeUp = true;
            if(m_mashCount < m_mashTarget)
            {
                return m_state = ActionState.fail;
            }
        }
        return m_state;
    }
    public override void CheckInput(InputAction.CallbackContext _context)
    {
        bool inputCorrect = false;
        if (m_state == ActionState.running && _context.action.name != "Directional")
        {
            for (int i = 0; i < m_readyInputs.Count; i++)
            {
                if (_context.action == m_readyInputs[i])
                {
                    m_mashCount++;
                    inputCorrect = true;
                    break;
                }
            }  
            if (inputCorrect == false)
            {
                m_qteDisplay.MissedInput(InputList);
                m_qteDisplay.IncorrectInput(_context.action.name);
                m_state = ActionState.fail;
            }
        }
    }

    public override void OnRelease(InputAction.CallbackContext _context)
    {

    }

    protected override void CheckSuccessWindow()
    {
        
    }

    protected override void onStart()
    {
        for (int i = 0; i < InputList.Count; i++)
        {
            m_qteDisplay.ActivateCue(i, Color.white);
            m_qteDisplay.AnimateMashCue(m_timeLimit, i, InputList[i]);
        }
    }

    
}

    
    
