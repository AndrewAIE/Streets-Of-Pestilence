using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "NewSequenceAction", menuName = "Quick Time Event/New Quick Time Action/SequenceAction")]
    public class QTESequenceAction : QTEAction
    {        
        private float[] m_actionTimeLimits;
        private int m_activeTimeSlot;
        private bool[] m_startedBools;                
        private bool m_allStartBoolsActivated = false;
        private bool m_finalIcon = false;
        [SerializeField]
        private float m_displayLeadInTime, m_timeBetweenInputs;
        
        protected override void onStart()
        {
            //create timers and bools for each input of the sequence
            m_actionTimeLimits = new float[InputList.Count];
            m_startedBools = new bool[InputList.Count];                      
            for (int i = 0; i < m_actionTimeLimits.Length; i++)
            {                
                m_actionTimeLimits[i] = m_timeLimit + (i * m_timeBetweenInputs) + m_displayLeadInTime;
                m_startedBools[i] = false;                
            }
            //m_actionTimeLimits[0] += m_displayLeadInTime;
            m_maxTime = m_actionTimeLimits[m_actionTimeLimits.Length - 1] + (m_successBuffer / 2);
            m_activeTimeSlot = 0;    
        }

        protected override ActionState onUpdate()
        {            
            if(!m_allStartBoolsActivated)
            {
                //Turn on bools as it reaches the appropriate time
                for (int i = 0; i < m_startedBools.Length; i++)
                {                    
                    if (m_startedBools[i] == false && m_timer >= m_actionTimeLimits[i] - m_displayLeadInTime)
                    {
                        //activate cue ring
                        m_qteDisplay.ActivateCue(i);
                        m_qteDisplay.AnimateCue(m_displayLeadInTime, i, InputList[i]);
                        m_startedBools[i] = true;
                        //stop iterating through loops if all bools are activated
                        if (i == m_startedBools.Length - 1)
                        {
                            m_allStartBoolsActivated = true;
                            break;
                        }
                    }
                }
            }
            if (m_state == ActionState.running)
            {
                CheckSuccessWindow();
                if (m_timer >= m_actionTimeLimits[m_activeTimeSlot] + (m_successBuffer / 2) && !m_finalIcon)
                {
                    m_qteDisplay.MissedInput(InputList[m_activeTimeSlot]);
                    m_qteDisplay.DeactivateCue(m_activeTimeSlot);                    
                    m_activeTimeSlot++;
                }
            }
            if (m_timer >= m_maxTime && m_state == ActionState.running)
            {
                m_state = ActionState.success;
                m_timeUp = true;                
            }
            return m_state;
        }        

        public override void CheckInput(InputAction.CallbackContext _context)
        {            
            if (m_state == ActionState.running && _context.action.name != "Directional")
            {
                if (m_successWindow && _context.action == m_readyInputs[m_activeTimeSlot])
                {
                    m_qteDisplay.SuccessfulInput(InputList[m_activeTimeSlot]);
                    m_qteDisplay.DeactivateCue(m_activeTimeSlot);
                    if(m_activeTimeSlot < m_startedBools.Length - 1)
                    {
                        m_activeTimeSlot++;
                    }
                    else
                    {
                        m_finalIcon = true;
                    }
                    CorrectInputs++;                                       
                    return;
                }
                m_qteDisplay.MissedInput(InputList[m_activeTimeSlot]);
                m_qteDisplay.IncorrectInput(_context.action.name);
                m_qteDisplay.DeactivateCue(m_activeTimeSlot);
                if (m_activeTimeSlot < m_startedBools.Length - 1 && m_startedBools[m_activeTimeSlot + 1])
                {
                    m_activeTimeSlot++;
                }
                else
                {
                    m_finalIcon = true;
                }
            }
        }

        protected override void CheckSuccessWindow()
        {
            if (m_timer >= m_actionTimeLimits[m_activeTimeSlot] - (m_successBuffer / 2) && m_timer <= m_actionTimeLimits[m_activeTimeSlot] + (m_successBuffer / 2))
            {
                m_successWindow = true;
                return;
            }
            m_successWindow = false;
        }
    }
}

