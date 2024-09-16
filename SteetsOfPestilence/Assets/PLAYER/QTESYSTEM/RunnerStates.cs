using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTESystem
{
    public abstract class RunnerState : MonoBehaviour
    {
        protected QTERunner m_runner;
        protected bool m_started = false;
        public abstract void EnterState(QTERunner _runner);
        public abstract void StateUpdate();
        public abstract void ExitState();
    }

    public class EncounterStart : RunnerState
    {
        public override void EnterState(QTERunner _runner)
        {
            m_runner = _runner;
            //set stance to netural
            m_runner.EnterStance(QTERunner.PlayerStance.NeutralStance);

            //turn on poise bar
            m_runner.QteDisplay.ActivatePoiseBar();

            //reset poise
            m_runner.PoiseBar.ResetPoise();
            
            //m_qteDisplay.UpdatePoiseBar(_poiseBar._poise);
            m_runner.WaitingStreams = new List<QTEStreamData>();
            for (int i = 0; i < m_runner.EncounterData.NeutralStreamData.Count; i++)
            {
                m_runner.WaitingStreams.Add(Instantiate(m_runner.ActiveStreamData[i]));
            }
            m_started = true;
        }

        public override void StateUpdate()
        {
            ExitState();
        }

        public override void ExitState()
        {
            m_runner.CurrentState = new ActionActive();
            m_runner.CurrentState.EnterState(m_runner);
        }
    }

    public class ActionActive : RunnerState
    {
        public override void EnterState(QTERunner _runner)
        {
            m_runner = _runner;
        }

        public override void StateUpdate()
        {

        }

        public override void ExitState()
        {
            
        }
    }

    public class StreamStart : RunnerState
    {
        public override void EnterState(QTERunner _runner)
        {
            m_runner = _runner;
        }

        public override void StateUpdate()
        {

        }

        public override void ExitState()
        {

        }
    }

    public class BetweenActions : RunnerState
    {
        public override void EnterState(QTERunner _runner)
        {
            m_runner = _runner;
        }

        public override void StateUpdate()
        {

        }

        public override void ExitState()
        {

        }
    }

    public class BetweenStreams : RunnerState
    {
        public override void EnterState(QTERunner _runner)
        {
            m_runner = _runner;
        }

        public override void StateUpdate()
        {

        }

        public override void ExitState()
        {

        }
    }

    public class EncounterEnd : RunnerState
    {
        public override void EnterState(QTERunner _runner)
        {
            m_runner = _runner;
        }

        public override void StateUpdate()
        {

        }

        public override void ExitState()
        {

        }
    }
}
