using System.Collections;
using System.Collections.Generic;
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
        private bool[] m_finishedBools;        
        private bool m_allStartBoolsActivated = false;
        [SerializeField]
        private float m_displayLeadInTime, m_timeBetweenInputs;

        protected override void onStart()
        {
            //create timers and bools for each input of the sequence
            m_actionTimeLimits = new float[InputList.Count];
            m_startedBools = new bool[InputList.Count];
            m_finishedBools = new bool[InputList.Count];            
            for (int i = 0; i < m_actionTimeLimits.Length; i++)
            {
                m_actionTimeLimits[i] = m_timeLimit + (i * m_timeBetweenInputs);
                m_startedBools[i] = false;
                m_finishedBools[i] = false;
            }
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
            if(m_state == ActionState.running && m_timer >= m_actionTimeLimits[m_activeTimeSlot] + (m_successBuffer / 2))
            {
                m_qteDisplay.DeactivateCue(m_activeTimeSlot);
                m_activeTimeSlot++;
                if(m_activeTimeSlot == m_actionTimeLimits.Length)
                {
                    m_state = ActionState.success;
                    m_timeUp = true;                    
                    for (int i = 0; i < InputList.Count; i++)
                    {                        
                        m_qteDisplay.VisualCues.RemoveAt(0);                        
                    }
                    
                }
            }
            return m_state;
        }

        public override void DisplayUpdate()
        {            
            //Animate the Rings
            for(int i = 0; i < m_actionTimeLimits.Length; i++)
            {
                if (m_state == ActionState.running && m_startedBools[i] == true)
                {
                    float cueSize = 1 - (m_timer / m_actionTimeLimits[i]);
                    m_qteDisplay.SetCueSize(cueSize, i);
                }
            }
            //Destroy each ring at the appropriate time
        }

        public override void CheckInput(InputAction.CallbackContext _context)
        {
            bool inputCorrect = false;
            if (m_state == ActionState.running && _context.action.name != "Directional")
            {
                if (m_timer >= m_actionTimeLimits[m_activeTimeSlot] - (m_successBuffer / 2) && m_timer <= m_actionTimeLimits[m_activeTimeSlot] + (m_successBuffer / 2))
                {
                    if (_context.action == m_readyInputs[m_activeTimeSlot])
                    {
                        CorrectInputs++;
                        inputCorrect = true;                        
                    }                    
                }
                if (inputCorrect == false)
                {
                    m_qteDisplay.MissedInput(InputList);
                    m_qteDisplay.IncorrectInput(_context.action.name);
                    m_qteDisplay.DeactivateCue(m_activeTimeSlot);                    
                    return;
                }
                else
                {
                    //set icon colour
                    m_qteDisplay.SuccessfulInput(InputList[m_activeTimeSlot]);
                    m_qteDisplay.DeactivateCue(m_activeTimeSlot);                    
                    return;
                }                
            }
        }
    }
}

