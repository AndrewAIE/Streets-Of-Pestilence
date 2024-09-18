using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;


namespace QTESystem
{
    public abstract class QTEAction : ScriptableObject
    {
        public bool Started = false;
        protected ActionState m_state;
        protected float m_timeLimit;
        protected float m_timer = 0;
        protected float m_minTime, m_maxTime;
        protected float m_successBuffer;
        public string IncorrectInput;
        private List<Image> m_timingRings;
        protected QTEDisplay m_qteDisplay;
       
        protected abstract ActionState onUpdate();

        public abstract void DisplayUpdate();
        
        public abstract void SetTargetInputs(QTEInputs _qteInputControl);
        public abstract void CheckInput(InputAction.CallbackContext _context);
        public List<QTEInput> InputList;

        public ActionState ActionUpdate(float _timer)
        {
            if (!Started)
            {                            
                //set timer and action state to defaults                
                m_state = ActionState.running;
                Started = true;
                IncorrectInput = null;                                
            }
            m_timer = _timer;
            return onUpdate();
        }

        public void SetData(float _timeLimit, float _successBuffer, QTEDisplay _display)
        {
            //Set Time Data
            m_timeLimit = _timeLimit;
            m_successBuffer = _successBuffer;
            m_minTime = m_timeLimit - (m_successBuffer / 2f);
            m_maxTime = m_timeLimit + (m_successBuffer / 2f);
            //Set QTE Display
            m_qteDisplay = _display;
        }
    }
}

