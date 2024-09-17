using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

namespace QTESystem
{
    public abstract class QTEStates
    {
        protected QTEManager m_qteManager;
        protected float m_timer;
        
        protected bool m_started = false;
        public abstract void EnterState(QTEManager _manager);
        public abstract void StateUpdate(float _timer);
        public abstract void ExitState();        
    }

    public class EncounterStart : QTEStates
    {
        public override void EnterState(QTEManager _manager)
        {
            m_qteManager = _manager;
            //m_qteDisplay.UpdatePoiseBar(_poiseBar._poise);            
            m_started = true;
        }

        public override void StateUpdate(float _timer)
        {
            m_qteManager.CurrentState = m_qteManager.StreamStart;
            ExitState();
        }

        public override void ExitState()
        {            
            m_qteManager.CurrentState.EnterState(m_qteManager);
        }
    }

    public class StreamStart : QTEStates
    {
        private float m_timeLimit;
        public override void EnterState(QTEManager _manager)
        {
            m_qteManager = _manager;
            //reset change in poise bar value and stream iterator                    
            m_qteManager.ChangeInPoiseValue = 0;
            m_qteManager.StreamPosition = 0;            

            //set new timer data and set timer to 0                    
            m_timeLimit = m_qteManager.ActiveStream.BeginningOfStreamPause;
            m_qteManager.Timer = 0;
            m_qteManager.ActivateStreamPanels(m_qteManager.GetStreamActionInputs());
        }

        public override void StateUpdate(float _timer)
        {
            if (_timer >= m_timeLimit)
            {
                m_qteManager.CurrentState = m_qteManager.ActionActive;
                ExitState();
            }
        }

        public override void ExitState()
        {
            m_qteManager.CurrentState.EnterState(m_qteManager);        
        }
    }

    public class ActionActive : QTEStates
    {
        QTEAction m_activeAction;
        QTEDisplay m_display;
        float m_timeLimit;
        float m_targetTime;
        ActionState m_activeActionState;

        public override void EnterState(QTEManager _manager)
        {
            //set components from QTE Runner
            m_qteManager = _manager;            
            m_activeAction = _manager.ActiveAction;
            m_display = _manager.QteDisplay;

            m_qteManager.Timer = 0;
            
            //Set new active action
            m_activeAction = m_qteManager.ActiveStream.Actions[m_qteManager.StreamPosition];
            m_qteManager.ActiveAction = m_activeAction;
            m_activeAction.SetData(m_qteManager.ActiveStream.ActionTimer, m_qteManager.ActiveStream.SuccessBuffer, _manager.QteDisplay);
            m_activeAction.SetTargetInputs(m_qteManager.InputActions);
           
            m_display.ActivateCue(0);
            m_timeLimit = m_qteManager.ActiveStream.ActionTimer + (m_qteManager.ActiveStream.SuccessBuffer / 2f);            
        }

        public override void StateUpdate(float _timer)
        {            
            //update and get state from active action
            m_activeActionState = m_activeAction.ActionUpdate(_timer);
            // determine whether action has succeeded or failed
            if (m_activeActionState == ActionState.running)
            {
                m_activeAction.DisplayUpdate();
            }                

            if (_timer >= m_timeLimit)
            {               
                m_qteManager.CurrentState = m_qteManager.BetweenActions;
                ExitState();
            }            
        }

        public override void ExitState()
        {
            m_qteManager.CurrentState.EnterState(m_qteManager); 
        }
    }
    

    public class BetweenActions : QTEStates
    {
        float m_timeLimit;
        public override void EnterState(QTEManager _manager)
        {
            m_qteManager = _manager;
            //increase stream iterator, reset display of icons 
            m_qteManager.StreamPosition++;
            //set new time limit and reset timer
            m_qteManager.ActiveAction.Started = false;
            m_qteManager.ActiveAction.IncorrectInput = null;
            m_qteManager.QteDisplay.SetIconColor(m_qteManager.ActiveDisplayList, Color.white);            
            m_timeLimit = m_qteManager.ActiveStream.BetweenActionTimer;
            m_qteManager.Timer = 0;
        }

        public override void StateUpdate(float _timer)
        {
            if (_timer > m_timeLimit)
            {
                if (m_qteManager.StreamPosition < m_qteManager.ActiveStream.Actions.Count)
                {
                    m_qteManager.CurrentState = m_qteManager.ActionActive;
                    ExitState();
                }
                else
                {
                    m_qteManager.CurrentState = m_qteManager.BetweenStreams;
                    ExitState();
                }                
            }
        }

        public override void ExitState()
        {
            m_qteManager.CurrentState.EnterState(m_qteManager);
        }
    }

    public class BetweenStreams : QTEStates
    {
        private float m_timeLimit;
        
        public override void EnterState(QTEManager _manager)
        {
            m_qteManager = _manager;

            //reset stream data
            m_qteManager.ResetStreamData();
            m_timeLimit = m_qteManager.ActiveStream.EndOfStreamPause;
        }

        public override void StateUpdate(float _timer)
        {
            //when timer is complete, start next stream            
            if (_timer > m_timeLimit)
            {
                m_qteManager.CurrentState = m_qteManager.StreamStart;
                ExitState();
            }
        }

        public override void ExitState()
        {            
            m_qteManager.CurrentState.EnterState(m_qteManager);
        }
    }
}
