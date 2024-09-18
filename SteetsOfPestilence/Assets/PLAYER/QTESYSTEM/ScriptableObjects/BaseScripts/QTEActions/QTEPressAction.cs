using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Press Action")]
    public class QTEPressAction : QTEAction
    {
        List<InputAction> m_readyInputs = new List<InputAction>();            

        protected override ActionState onUpdate()
        {
            if (m_timer > m_maxTime && m_state == ActionState.running)
            {
                RemoveTimingRings(InputList.Count);
                m_qteDisplay.MissedInput(InputList);
                m_state = ActionState.fail;                
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
                            CorrectInputs++;
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
                        RemoveTimingRings(InputList.Count);
                        m_state = ActionState.fail;
                        return;
                    }
                    if (m_readyInputs.Count == 0)
                    {                       
                        //set icon colour
                        m_qteDisplay.SetIconColor(InputList, Color.green);
                        RemoveTimingRings(InputList.Count);
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

        public void RemoveTimingRings(int _count)
        {
            if(_count > 0)
            {
                for (int i = 0; i < _count; i++)
                {
                    GameObject Holder = m_qteDisplay.VisualCues[i];
                    m_qteDisplay.VisualCues.RemoveAt(i);
                    Destroy(Holder);
                }
            }           
        }

        public override void DisplayUpdate()
        {
            if(m_state == ActionState.running)
            {
                for (int i = 0; i < InputList.Count; i++)
                {
                    float cueSize = 1 - (m_timer / m_timeLimit);
                    if (m_state == ActionState.running)
                    {
                        m_qteDisplay.SetCueSize(cueSize, i);
                    }
                }
            }           
        }       
    }
}
