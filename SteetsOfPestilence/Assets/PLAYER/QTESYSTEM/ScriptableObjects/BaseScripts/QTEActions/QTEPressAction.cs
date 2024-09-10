using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Press Action")]
    public class QTEPressAction : QTEAction
    {
        List<InputAction> m_readyInputs = new List<InputAction>();     

        public override void SetTimeLimit(float _timeLimit, float _successBuffer)
        {
            m_timeLimit = _timeLimit;
            m_successBuffer = _successBuffer;
            m_minTime = m_timeLimit - (m_successBuffer / 2f);
            m_maxTime = m_timeLimit + (m_successBuffer / 2f);
        }

        protected override ActionState onUpdate()
        {              
            return m_state;
        }

        public override void CheckInput(InputAction.CallbackContext _context)
        {
            bool inputCorrect = false;            
            if (_context.performed && _context.action.name != "Directional")
            {
                if (m_timer >= m_minTime && m_timer <= m_maxTime)
                {
                    for (int i = 0; i < m_readyInputs.Count; i++)
                    {
                        if (_context.action == m_readyInputs[i])
                        {
                            m_readyInputs.RemoveAt(i);
                            inputCorrect = true;
                            break;
                        }
                    }
                }
                if (m_state == ActionState.running)
                {
                    if (inputCorrect == false)
                    {
                        IncorrectInput = _context.action.name;
                        m_state = ActionState.fail;
                        return;
                    }
                    if (m_readyInputs.Count == 0)
                    {
                        m_state = ActionState.success;
                    }
                }
            }                       
        }       

        public override void DisplayUpdate()
        {
            
        }        

        public override void SetTargetInputs(QTEInputs _qteInputControl)
        {
            //assign values to check inputs against based on the public InputList 
            m_readyInputs = new List<InputAction>();            
            foreach (QTEInput input in InputList)
            {
                switch (input)
                {
                    case QTEInput.NorthFace:
                        m_readyInputs.Add(_qteInputControl.Inputs.North);                        
                        break;
                    case QTEInput.EastFace:
                        m_readyInputs.Add(_qteInputControl.Inputs.East);
                        break;
                    case QTEInput.SouthFace:
                        m_readyInputs.Add(_qteInputControl.Inputs.South);
                        break;
                    case QTEInput.WestFace:
                        m_readyInputs.Add(_qteInputControl.Inputs.West);
                        break;
                    case QTEInput.LeftShoulder:
                        m_readyInputs.Add(_qteInputControl.Inputs.LShoulder);
                        break;
                    case QTEInput.RightShoulder:
                        m_readyInputs.Add(_qteInputControl.Inputs.RShoulder);
                        break;
                    case QTEInput.LeftTrigger:
                        m_readyInputs.Add(_qteInputControl.Inputs.LTrigger);
                        break;                    
                    case QTEInput.RightTrigger:
                        m_readyInputs.Add(_qteInputControl.Inputs.RTrigger);
                        break;
                    default:
                        break;
                }
            }            
        }        
    }
}
