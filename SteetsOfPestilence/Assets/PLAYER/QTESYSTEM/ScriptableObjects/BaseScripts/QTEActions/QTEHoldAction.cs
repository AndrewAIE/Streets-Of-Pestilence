using QTESystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Hold Action")]
public class QTEHoldAction : QTEAction
{
    [SerializeField]
    private float m_holdDuration;
    private bool m_held = false;    
    List<InputAction> m_inputsToBeHeld;
    private float m_totalTime;
    protected override void onStart()
    {       
        m_totalTime = m_holdDuration + m_timeLimit;
        m_inputsToBeHeld = new List<InputAction>();
        for(int i = 0; i < m_readyInputs.Count; i++)
        {
            m_inputsToBeHeld.Add(m_readyInputs[i]);
            m_qteDisplay.ActivateCue(i, Color.red);
            m_qteDisplay.AnimateCue(m_timeLimit, i, InputList[i]);
        }
    }

    protected override ActionState onUpdate()
    {        
        if(m_timer >= m_totalTime)
        {
            m_timeUp = true;
            if(m_held == true)
            {
                CorrectInputs += InputList.Count;
                m_state = ActionState.success;
                m_qteDisplay.SetIconColor(InputList, Color.red);
            }           
        }
        return m_state;
    }
    public override void CheckInput(InputAction.CallbackContext _context)
    {
        CheckSuccessWindow();
        if (!m_successWindow) 
        {
            m_qteDisplay.MissedInput(InputList);
            m_qteDisplay.IncorrectInput(_context.action.name);
            m_state = ActionState.fail;
            return;
        } 
        bool inputCorrect = false;
        if (m_state == ActionState.running && _context.action.name != "Directional")
        {
            for (int i = 0; i < m_readyInputs.Count; i++)
            {
                if (_context.action == m_readyInputs[i])
                {
                    Debug.Log("YAS");
                    m_readyInputs.RemoveAt(i);
                    inputCorrect = true;
                    break;
                }
            }
            if (m_readyInputs.Count == 0)
            {
                //set icon colour
                for (int i = 0; i < InputList.Count; i++)
                {
                    m_qteDisplay.ShakeCue(i, m_holdDuration);
                }
                m_qteDisplay.SetIconColor(InputList, Color.green);
                m_held = true;
                return;
            }
            if (inputCorrect == false)
            {
                m_qteDisplay.MissedInput(InputList);
                m_qteDisplay.IncorrectInput(_context.action.name);
                m_state = ActionState.fail;
            }            
        }        
    }

    protected override void CheckSuccessWindow()
    {
        if (m_timer >= m_minTime && m_timer <= m_maxTime)
        {
            m_successWindow = true;
            return;
        }
        m_successWindow = false;
    }

    public override void OnRelease(InputAction.CallbackContext _context)
    {        
        //this is peak code dont worry about it
        if (!m_held)
        {
            return;
        }
        for (int i = 0; i < m_inputsToBeHeld.Count; i++)
        {
            if (_context.action == m_inputsToBeHeld[i])
            {
                for (int j = 0; j < m_inputsToBeHeld.Count; j++)
                {
                    m_qteDisplay.DeactivateCue(j);
                    m_qteDisplay.MissedInput(InputList);
                }
                m_qteDisplay.SetIconColor(InputList, Color.white);
                m_state = ActionState.fail;
                m_held = false;
                break;
            }
        }
    }
}
