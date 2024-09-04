using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace QTESystem
{
    public abstract class QTEAction : ScriptableObject
    {
        public bool Started = false;
        protected ActionState m_state;
        protected float m_timeLimit;
        protected float m_timer = 0;        
        public string IncorrectInput;        
       
        protected abstract ActionState onUpdate();

        public abstract void DisplayUpdate();
        public abstract void SetTimeLimit(float _timer);
        public abstract void SetTargetInputs(QTEInputs _qteInputControl);

        public abstract void CheckInput(InputAction.CallbackContext _context);
        public List<QTEInput> InputList;

        public ActionState ActionUpdate()
        {
            if (!Started)
            {                            
                //set timer and action state to defaults
                m_timer = 0;
                m_state = ActionState.running;
                Started = true;
                IncorrectInput = null;                                
            }            
            return onUpdate();
        }
        
        public void CompleteAction()
        {
            m_state = ActionState.complete;
        }
    }
}

