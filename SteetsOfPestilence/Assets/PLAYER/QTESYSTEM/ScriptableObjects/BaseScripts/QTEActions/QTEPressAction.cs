using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Press Action")]
    public class QTEPressAction : QTEAction
    {         

        protected override ActionState onUpdate()
        {
            if(m_timer > m_maxTime)
            {
                m_timeUp = true;                
                if (m_state == ActionState.running)
                {                                        
                    m_qteDisplay.MissedInput(InputList);
                    m_state = ActionState.fail;
                }
            }
            CheckSuccessWindow();
            return m_state;            
        }

        protected override void onStart()
        {            
            for (int i = 0; i < InputList.Count; i++)
            {
                m_qteDisplay.ActivateCue(i, Color.white);
                m_qteDisplay.AnimateCue(m_timeLimit, i, InputList[i]);
            }
        }

        public override void CheckInput(InputAction.CallbackContext _context)
        {            
            bool inputCorrect = false;            
            if (m_state == ActionState.running && _context.action.name != "Directional")
            {
                if (m_successWindow)
                {
                    for (int i = 0; i < m_readyInputs.Count; i++)
                    {
                        if (_context.action == m_readyInputs[i])
                        {
                            m_readyInputs.RemoveAt(i);
                            CorrectInputs++;
                            m_qteDisplay.DeactivateCue(i);
                            inputCorrect = true;
                            break;
                        }
                    }
                    if (m_readyInputs.Count == 0)
                    {
                        //set icon colour
                        m_qteDisplay.SetIconColor(InputList, Color.green);                        
                        m_state = ActionState.success;
                        return;
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
            
        }
    }
}
