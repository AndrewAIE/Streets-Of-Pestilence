using UnityEngine;

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
        private float m_timeLimit;
        public override void EnterState(QTEManager _manager)
        {
            m_qteManager = _manager;            
            m_timeLimit = 1f;            
        }

        public override void StateUpdate(float _timer)
        {            
            if(_timer > m_timeLimit)
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

    public class StreamStart : QTEStates
    {
        private float m_timeLimit;
        public override void EnterState(QTEManager _manager)
        {            
            m_qteManager = _manager;
            m_qteManager.SlowTime(true);
            //reset change in poise bar value and stream iterator                    
            m_qteManager.ChangeInPoiseValue = 0;
            m_qteManager.StreamPosition = 0;
            m_qteManager.AvailableSuccessPoints = 0;
            m_qteManager.CurrentSuccessPoints = 0;
            m_qteManager.ActivateInputCues(m_qteManager.GetStreamActionInputs());
            //set new timer data and set timer to 0                    
            m_timeLimit = m_qteManager.ActiveStream.BeginningOfStreamPause;
            m_qteManager.Timer = 0;            
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

        ActionState m_activeActionState;

        public override void EnterState(QTEManager _manager)
        {
            //set components from QTE Runner
            m_qteManager = _manager;            
            m_activeAction = _manager.ActiveAction;
            m_qteManager.Timer = 0;            
            //Set new active action
            m_activeAction = m_qteManager.CreateAction();            
            m_activeAction.SetData(m_qteManager.ActiveStream.ActionTimer, m_qteManager.ActiveStream.SuccessBuffer, _manager.QteDisplay);
            m_activeAction.SetTargetInputs(m_qteManager.InputActions);
            m_qteManager.AvailableSuccessPoints += m_activeAction.InputList.Count;        
                        
        }

        public override void StateUpdate(float _timer)
        {            
            //update and get state from active action
            m_activeActionState = m_activeAction.ActionUpdate(_timer);
            // determine whether action has succeeded or failed
            switch(m_activeActionState)
            {
                case ActionState.success:
                    m_qteManager.CurrentSuccessPoints += m_activeAction.CorrectInputs;                    
                    m_activeAction.CompleteAction();
                    break;
                case ActionState.fail:
                    m_activeAction.CompleteAction();
                    break;                
                default:
                    break;
            }       
            if (m_activeAction.TimeUp())
            {                
                m_activeAction.RemoveTimingRings();
                m_qteManager.CurrentState = m_qteManager.BetweenActions;
                ExitState();
            }            
        }
        
        public override void ExitState()
        {
            m_qteManager.DestroyAction();
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

    public class CombatAnimation : QTEStates
    {
        public override void EnterState(QTEManager _manager)
        {            
            m_qteManager = _manager;
            m_qteManager.SlowTime(false);
            //reset stream data
            m_qteManager.ResetStreamData();            
            //set poise bar                     
            m_qteManager.PoiseValueCheck();
            m_qteManager.SelectQTECombatAnimations();
        }

        public override void StateUpdate(float _timer)
        {
            //when timer is complete, start next stream            
            if (m_qteManager.CombatAnimation.EndState)
            {                
                m_qteManager.CurrentState = m_qteManager.StreamStart;
                ExitState();
            }
        }

        public override void ExitState()
        {
            
            m_qteManager.SelectStream();
            m_qteManager.CurrentState.EnterState(m_qteManager);
        }       
    }
}
