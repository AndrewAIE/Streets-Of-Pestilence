using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Press Action")]
    public class QTEPressAction : QTEAction
    {
        List<InputAction> m_readyInputs = new List<InputAction>();     

        public override void SetData(float _timeLimit, float _successBuffer, QTEDisplay _display)
        {
            //Set Time Data
            m_timeLimit = _timeLimit;
            m_successBuffer = _successBuffer;
            m_minTime = m_timeLimit - (m_successBuffer / 2f);
            m_maxTime = m_timeLimit + (m_successBuffer / 2f);
            //Set QTE Display
            m_qteDisplay = _display;            
        }

        protected override ActionState onUpdate()
        {
            if (m_timer > m_maxTime && m_state == ActionState.running)
            {
                if (m_state == ActionState.running)
                {
                    RemoveTimingRing();
                    m_qteDisplay.MissedInput(InputList);
                    m_state = ActionState.fail;
                }
            }
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
                        m_qteDisplay.MissedInput(InputList);
                        m_qteDisplay.IncorrectInput(_context.action.name);                        
                        RemoveTimingRing();
                        m_state = ActionState.fail;
                        return;
                    }
                    if (m_readyInputs.Count == 0)
                    {                       
                        //set icon colour
                        m_qteDisplay.SetIconColor(InputList, Color.green);
                        RemoveTimingRing();
                        m_state = ActionState.success;
                        return;
                    }
                }                
            }                       
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
                    case QTEInput.NorthDirectional:
                        m_readyInputs.Add(_qteInputControl.Inputs.Up);
                        break;
                    case QTEInput.EastDirectional:
                        m_readyInputs.Add(_qteInputControl.Inputs.Right);
                        break;
                    case QTEInput.SouthDirectional:
                        m_readyInputs.Add(_qteInputControl.Inputs.South);
                        break;
                    case QTEInput.WestDirectional:
                        m_readyInputs.Add(_qteInputControl.Inputs.Left);
                        break;
                    default:
                        break;
                }
            }
        }

        public void RemoveTimingRing()
        {
            GameObject Holder = m_qteDisplay.VisualCues[0];
            m_qteDisplay.VisualCues.RemoveAt(0);
            Destroy(Holder);
        }

        public override void DisplayUpdate()
        {
            float cueSize = 1 - (m_timer / m_timeLimit);
            if (m_state == ActionState.running)
            {
                m_qteDisplay.SetCueSize(cueSize);
            }
        }       
    }
}
