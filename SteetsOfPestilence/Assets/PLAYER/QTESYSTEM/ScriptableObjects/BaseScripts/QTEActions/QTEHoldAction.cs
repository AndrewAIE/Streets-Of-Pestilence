using QTESystem;
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
        Color ringColor = new Color(0.5f, 0, 0, 1);
        for(int i = 0; i < m_readyInputs.Count; i++)
        {
            m_inputsToBeHeld.Add(m_readyInputs[i]);
            m_qteDisplay.ActivateCue(i, ringColor);            
            m_qteDisplay.AnimateCue(m_timeLimit, i, InputList[i]);
        }
    }
    protected override ActionState onUpdate()
    {      
        if(m_timer > m_maxTime + 0.1f && !m_held && m_state == ActionState.running)
        {
            for (int i = 0; i < InputList.Count; i++)
            {
                m_qteDisplay.DeactivateCue(i);
            }
            m_state = ActionState.fail;
            m_qteDisplay.MissedInput(InputList);
        }
        if(m_timer >= m_totalTime)
        {
            m_timeUp = true;
            if (m_held == true)
            {                
                m_held = false;
                m_qteDisplay.StopShake();
                CorrectInputs += InputList.Count;
                for (int i = 0; i < InputList.Count; i++)
                {
                    m_qteDisplay.SuccessfulInput(InputList[i], i);
                }
                return m_state = ActionState.success;
            }            
            if(m_state == ActionState.running)
            {
                m_state = ActionState.fail;
                m_qteDisplay.MissedInput(InputList);
            }
            
        }
        return m_state;
    }
    public override void CheckInput(InputAction.CallbackContext _context)
    {        
        if(m_state != ActionState.running)
        {
            return;
        }       
        
        bool inputCorrect = false;
        if (_context.action.name != "Directional")
        {
            if (!CheckSuccessWindow())
            {
                m_qteDisplay.MissedInput(InputList);
                m_qteDisplay.IncorrectInput(_context.action.name);
                m_state = ActionState.fail;
                for (int i = 0; i < m_inputsToBeHeld.Count; i++)
                {
                    m_qteDisplay.DeactivateCue(i);
                }
                return;
            }
            for (int i = 0; i < m_readyInputs.Count; i++)
            {
                if (_context.action == m_readyInputs[i])
                {                    
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
                for (int i = 0; i < m_inputsToBeHeld.Count; i++)
                {
                    m_qteDisplay.DeactivateCue(i);
                }
                m_qteDisplay.MissedInput(InputList);
                m_qteDisplay.IncorrectInput(_context.action.name);
                m_state = ActionState.fail;
            }            
        }        
    }

    protected override bool CheckSuccessWindow()
    {
        if (m_timer >= m_minTime && m_timer <= m_maxTime)
        {
            return true;
        }
        return false;
    }

    public override void OnRelease(InputAction.CallbackContext _context)
    {       
        if (!m_held)
        {
            return;
        }        
        m_qteDisplay.StopShake();
        for (int i = 0; i < m_inputsToBeHeld.Count; i++)
        {
            if (_context.action == m_inputsToBeHeld[i])
            {
                for (int j = 0; j < m_inputsToBeHeld.Count; j++)
                {
                    m_qteDisplay.DeactivateCue(j);
                }
                m_state = ActionState.fail;
                m_held = false;                
                m_qteDisplay.SetIconColor(InputList, Color.white);
                m_qteDisplay.MissedInput(InputList);
                break;
            }
        }        
    }

    public override void CreateInputRings()
    {
        foreach (QTEInput input in InputList)
        {
            m_qteDisplay.CreateHoldInputPrompt(input);
        }

    }
}
