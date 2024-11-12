using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;


namespace QTESystem
{
    public abstract class QTEAction : ScriptableObject
    {
        [HideInInspector]
        public bool Started = false;
        protected ActionState m_state;
        protected float m_timeLimit;
        protected float m_timer = 0;
        protected float m_minTime, m_maxTime;
        protected float m_successBuffer;
        [HideInInspector]
        public string IncorrectInput;
        private List<Image> m_timingRings;
        protected QTEDisplay m_qteDisplay;
        [HideInInspector]
        public int CorrectInputs = 0;
        protected List<InputAction> m_readyInputs = new List<InputAction>();       

        protected abstract ActionState onUpdate();
        protected abstract void onStart();
        protected abstract bool CheckSuccessWindow();
        
        public void RemoveTimingRings()
        {
            for (int i = 0; i < InputList.Count; i++)
            {                
                m_qteDisplay.ActiveVisualCues.RemoveAt(0);                
            }            
        }
        public void SetTargetInputs(QTEInputs _qteInputControl)
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
        public abstract void CheckInput(InputAction.CallbackContext _context);
        public abstract void OnRelease(InputAction.CallbackContext _context);
        public List<QTEInput> InputList;

        protected bool m_timeUp;
        public bool TimeUp()
        {
            return m_timeUp;
        }

        public ActionState ActionUpdate(float _timer)
        {
            if (!Started)
            {                            
                //set timer and action state to defaults                
                m_state = ActionState.running;
                Started = true;
                IncorrectInput = null;
                m_timeUp = false;
                onStart();
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

        public void CompleteAction()
        {
            m_state = ActionState.complete;
        }       
    }
}

