using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

namespace QTESystem
{
    #region Public Enums
    //comment
    public enum ActionState
    {
        running,
        success,
        fail,
        complete
    }

    //comment
    public enum QTEInput
    {
        NorthFace,
        EastFace,
        SouthFace,
        WestFace,
        LeftShoulder,
        LeftTrigger,
        RightShoulder,
        RightTrigger,
        NorthDirectional,
        EastDirectional,
        SouthDirectional,
        WestDirectional
    }

    #endregion

    public class QTERunner : MonoBehaviour
    {
        //******************** Variables *******************//
        #region Variables

        //*** QTE DATA ***//
        #region QTE Data
        
        private InputActionMap m_actionMap;
        
        public QTEInputs InputActions;        
        
        public QTEEncounterData EncounterData;
        
        public List<QTEStreamData> ActiveStreamData;
        
        public List<QTEStreamData> WaitingStreams;
       
        public QTEStreamData ActiveStream;
        
        public QTEAction ActiveAction;        
        
        [Tooltip("QTE Display")]
        public QTEDisplay QteDisplay;

        public List<QTEInput> ActiveDisplayList;

        #endregion

        //*** Poise Bar ***//
        #region Poise Bar
        public PoiseBarController PoiseBar;

        //comment
        public int StreamPosition;

        //comment
        public int ChangeInPoiseValue;

        #endregion

        //*** Timers ***//
        #region Timers
        private float Timer;        
        private float m_beginningOfStreamTimeLimit;
        private float m_betweenActionTimeLimit;

        float m_actionTimer;
        #endregion

        //*** Enum Variables ***//
        #region Enum Variables
        private ActionState m_actionState;        
        public PlayerStance CurrentPlayerStance;
        private EncounterState m_encounterState;

        #endregion

        //*** Player & Enemy ***//
        #region Player & Enemy 
        public GameObject Enemy;
        public GameObject Player;

        #endregion

        #endregion
        //******************** QTE Runner States **********************//
        public RunnerState CurrentState;       


        //******************** Enums **********************//
        #region Enums

        //comment
        public enum PlayerStance
        {
            NeutralStance,
            OffensiveStance,
            DefensiveStance
        }

        //comment
        private enum EncounterState
        {
            beginningOfEncounter,
            beginningOfStream,
            actionActive,
            betweenActions,            
            betweenStreams,            
            endOfEncounter            
        }

        #endregion

        //******************** Methods ********************//
        #region Methods

        //*** Awake, Enable, Disable ***//
        #region Awake, Enable, Disable

        private void Awake()
        {
            InputActions = new QTEInputs();
            ActiveDisplayList = new List<QTEInput>();
            Player = GetComponent<GameObject>();         
        }
        
        private void OnEnable()
        {            
            InputActions.Enable();
            m_actionMap = InputActions.Inputs;
            m_actionMap.actionTriggered += onActionInput;
        }

        private void OnDisable()
        {
            m_actionMap.actionTriggered -= onActionInput;
            InputActions.Disable();            
        }

        #endregion

        //*** Update ***//
        #region Update

        void Update()
        {
            Timer += Time.deltaTime;
            CurrentState.StateUpdate();
                    
        }

        #endregion

        //*** Loading Encoutner Data ***//
        #region LoadingEncounterData
        public void LoadEncounter(QTEEncounterData _encounterData, GameObject _enemy)
        {
            //load data from encounter and start the neutral stream            
            EncounterData = _encounterData;            
            Enemy = _enemy;
            EnterEncounterState(EncounterState.beginningOfEncounter);            
        }

        public List<QTEInput> GetStreamActionInputs()
        {
            List<QTEInput> actions = new List<QTEInput>();
            for (int i = 0; i < ActiveStream.Actions.Count; i++)
            {
                for (int j = 0; j < ActiveStream.Actions[i].InputList.Count; j++)
                {
                    actions.Add(ActiveStream.Actions[i].InputList[j]);
                }
            }
            return actions;
        }

        #endregion

        //*** Encoutner States ***//
        #region EncounterStates
        private void EnterEncounterState(EncounterState _encounterState)
        {
            m_encounterState = _encounterState;
            switch(m_encounterState)
            {                
                case EncounterState.beginningOfEncounter:
                    
                    break;

                case EncounterState.beginningOfStream:

                    //reset change in poise bar value and stream iterator                    
                    ChangeInPoiseValue = 0;
                    StreamPosition = 0;
                    //select approp
                    //activate appropriate panels in QTE Display
                    
                    ActiveStream = selectRandomStream();
                    activateStreamPanels(GetStreamActionInputs());                    
                    
                    //set new timer data and set timer to 0                    
                    m_beginningOfStreamTimeLimit = ActiveStream.BeginningOfStreamPause;                    
                    Timer = 0;
                    break;

                case EncounterState.actionActive:

                    //set active action, set time limit and controls and activate appropriate display, enable controls
                    Timer = 0;
                    ActiveAction = ActiveStream.Actions[StreamPosition];
                    ActiveAction.SetTimeLimit(ActiveStream.ActionTimer, ActiveStream.SuccessBuffer);
                    ActiveAction.SetTargetInputs(InputActions);                   
                    QteDisplay.ActivateCue();
                    m_actionTimer = ActiveStream.ActionTimer + (ActiveStream.SuccessBuffer / 2f);
                    break;

                case EncounterState.betweenActions:

                    //increase stream iterator, reset display of icons 
                    StreamPosition++;
                    //set new time limit and reset timer
                    ActiveAction.Started = false;
                    ActiveAction.IncorrectInput = null;
                    QteDisplay.SetIconColor(ActiveDisplayList, Color.white);
                    m_betweenActionTimeLimit = ActiveStream.BetweenActionTimer;
                    Timer = 0;                    
                    break;

                case EncounterState.betweenStreams:

                    //deactivate icon display, reset timer and iterator, adjust poise bar
                    QteDisplay.DeactivatePanels();
                    StreamPosition = 0;
                    Timer = 0;
                    PoiseValueCheck();
                    ActiveDisplayList.Clear();                    
                    break;
                    
                default:
                    break;

            }
        }

        private void beginningOfEncounter()
        {
            //enable controls for QTE Event            
            EnterEncounterState(EncounterState.beginningOfStream);
        }
        
        private void beginningOfStream()
        {
            //When timer ends, begin Quick Time Stream            
            if(Timer >= m_beginningOfStreamTimeLimit)
            {
                EnterEncounterState(EncounterState.actionActive);
            }
        }

        private void activeAction()
        {
            // update and get state from active action
            m_actionState = ActiveAction.ActionUpdate(Timer);
            float cueSize = 1 - (Timer / ActiveStream.ActionTimer);
            if(m_actionState == ActionState.running)
            {
                QteDisplay.SetCueSize(cueSize);
            }            
            // determine whether action has succeeded or failed
            switch (m_actionState)
            {
                case ActionState.success:
                    actionSuccess();
                    break;
                case ActionState.fail:
                    actionIncorrectInput();
                    break;
                default:
                    break;
            }
            
            if(Timer >= m_actionTimer)
            {
                if(m_actionState == ActionState.running)
                {
                    actionTimerFail();
                }
                EnterEncounterState(EncounterState.betweenActions);
            }
        }

        private void betweenActions()
        {
            //When timer is complete, either activate next action or end stream if there are no actions left
            
            if (Timer > m_betweenActionTimeLimit)
            {
                if (StreamPosition >= ActiveStream.Actions.Count)
                {                    
                    EnterEncounterState(EncounterState.betweenStreams);
                }
                else
                {                    
                    EnterEncounterState(EncounterState.actionActive);
                }
            }
        }

        private void betweenStreams()
        {
            //when timer is complete, start next stream            
            if (Timer > ActiveStream.EndOfStreamPause)
            {
                EnterEncounterState(EncounterState.beginningOfStream);
            }
        }

        private void endOfEncounter()
        {            
            QteDisplay.DeactivatePoiseBar();
            WaitingStreams.Clear();
            ActiveStream = null;
            this.enabled = false;
        }

        #endregion

        //*** Success & Fail Methods ***//
        #region Success & Fail Methods 

        //comment
        private void actionSuccess()
        {
            //increase poise value and enter between actions state
            ActiveAction.CompleteAction();
            ChangeInPoiseValue++;

            //set icon colour
            QteDisplay.SetIconColor(ActiveAction.InputList, Color.green);
            GameObject Holder = QteDisplay.VisualCues[0];
            QteDisplay.VisualCues.RemoveAt(0);
            Destroy(Holder);
        }

        //comment
        private void actionIncorrectInput()
        {
            //decrease poise value and enter between actions state
            ActiveAction.CompleteAction();
            ChangeInPoiseValue--;            
            QteDisplay.MissedInput(ActiveAction.InputList);
            QteDisplay.IncorrectInput(ActiveAction.IncorrectInput);
            GameObject Holder = QteDisplay.VisualCues[0];
            QteDisplay.VisualCues.RemoveAt(0);
            Destroy(Holder);
        }

        //comment
        private void actionTimerFail()
        {
            ChangeInPoiseValue--;            
            QteDisplay.MissedInput(ActiveAction.InputList);
            GameObject Holder = QteDisplay.VisualCues[0];
            QteDisplay.VisualCues.RemoveAt(0);
            Destroy(Holder);
        }

        #endregion

        //*** Stream Data ***//
        #region Stream Data

        //Comment
        public void EnterStance(PlayerStance _stance)
        {
            CurrentPlayerStance = _stance;
            switch (CurrentPlayerStance)
            {
                case PlayerStance.NeutralStance:
                    ActiveStreamData = EncounterData.NeutralStreamData;
                    break;
                case PlayerStance.OffensiveStance:
                    ActiveStreamData = EncounterData.OffensiveStreamData;
                    break;
                case PlayerStance.DefensiveStance:
                    ActiveStreamData = EncounterData.DefensiveStreamData;
                    break;
                default:
                    break;
            }
            //for (int i = 0; i < m_encounterData.NeutralStreamData.Count; i++)
            //{
            //    m_waitingStreams.Add(Instantiate(m_activeStreamData[i]));
            //}
        }

        //Comment
        private QTEStreamData selectRandomStream()
        {             
            Debug.Log(WaitingStreams.Count);
            int selector = Random.Range(0, WaitingStreams.Count);

            QTEStreamData selectedStream = Instantiate(WaitingStreams[selector]);
            WaitingStreams.RemoveAt(selector);
            if(ActiveStream)
            {
                WaitingStreams.Add(ActiveStream);
            }
            return selectedStream;
        }

        //Comment
        private void activateStreamPanels(List<QTEInput> _streamInputs)
        {
            bool[] panelActivator = { false, false, false, false };

            foreach (QTEInput input in _streamInputs)
            {
                QteDisplay.CreateInputPrompt(input);
                switch (input)
                {
                    case QTEInput.NorthDirectional:
                        panelActivator[3] = true;
                        break;
                    case QTEInput.EastDirectional:
                        panelActivator[3] = true;
                        break;
                    case QTEInput.SouthDirectional:
                        panelActivator[3] = true;
                        break;
                    case QTEInput.WestDirectional:
                        panelActivator[3] = true;
                        break;
                    case QTEInput.NorthFace:
                        panelActivator[2] = true;                        
                        break;
                    case QTEInput.EastFace:                        
                        panelActivator[2] = true;
                        break;
                    case QTEInput.SouthFace:                        
                        panelActivator[2] = true;
                        break;
                    case QTEInput.WestFace:                        
                        panelActivator[2] = true;
                        break;
                    case QTEInput.LeftShoulder:                        
                        panelActivator[1] = true;
                        break;
                    case QTEInput.RightShoulder:                        
                        panelActivator[1] = true;
                        break;
                    case QTEInput.LeftTrigger:                        
                        panelActivator[0] = true;
                        break;                    
                    case QTEInput.RightTrigger:                        
                        panelActivator[0] = true;
                        break;
                }
            }
            for (int i = 0; i < panelActivator.Length; i++)
            {
                if (panelActivator[i])
                {                    
                    QteDisplay.ActivatePanel(i);
                    switch(i)
                    {
                        case 0:
                            ActiveDisplayList.Add(QTEInput.LeftTrigger);
                            ActiveDisplayList.Add(QTEInput.RightTrigger);
                            break;
                        case 1:
                            ActiveDisplayList.Add(QTEInput.LeftShoulder);
                            ActiveDisplayList.Add(QTEInput.RightShoulder);
                            break;
                        case 2:
                            ActiveDisplayList.Add(QTEInput.NorthFace);
                            ActiveDisplayList.Add(QTEInput.EastFace);
                            ActiveDisplayList.Add(QTEInput.SouthFace);
                            ActiveDisplayList.Add(QTEInput.WestFace);
                            break;
                        case 3:
                            ActiveDisplayList.Add(QTEInput.NorthDirectional);
                            ActiveDisplayList.Add(QTEInput.EastDirectional);
                            ActiveDisplayList.Add(QTEInput.SouthDirectional);
                            ActiveDisplayList.Add(QTEInput.WestDirectional);
                            break;
                    }
                }
            }
        }

        #endregion

        //*** Poise Bar and Combat Outcome ***//
        #region Poise Bar and Combat Outcome

        //Comment
        public void PoiseValueCheck()
        {
            //adjust poise value based of successes and falures in stream
            PoiseBar.SetPoise(ChangeInPoiseValue);

            //change to appropriate stance based off of poise value
            //switch (m_playerStance)
            //{
            //    case PlayerStance.NeutralStance:
            //        if (m_poiseValue >= 4)
            //        {
            //            EnterStance(PlayerStance.OffensiveStance);
            //            break;
            //        }
            //        if (m_poiseValue <= -4)
            //        {
            //            EnterStance(PlayerStance.DefensiveStance);
            //            break;
            //        }
            //        break;
            //    case PlayerStance.DefensiveStance:
            //        if (m_poiseValue > -4)
            //        {
            //            EnterStance(PlayerStance.NeutralStance);
            //            break;
            //        }
            //        if (m_poiseValue <= -10)
            //        {
            //            playerLoss();
            //            break;
            //        }
            //        break;
            //    case PlayerStance.OffensiveStance:
            //        if (m_poiseValue < 4)
            //        {
            //            EnterStance(PlayerStance.NeutralStance);
            //            break;
            //        }
            //        if (m_poiseValue >= 10)
            //        {
            //            playerWin();
            //            break;
            //        }
            //        break;
            //    default:
            //        break;
            //
            //}

            if (PoiseBar._poise >= PoiseBar._maxPoise)
            {
                playerWin();
            }
            if (PoiseBar._poise <= PoiseBar._minPoise)
            {
                playerLoss();
            }

            //m_qteDisplay.UpdatePoiseBar(_poiseBar._poise);
        }
        
        //Player Win
        private void playerWin()
        {
            Destroy(Enemy);
            EnterEncounterState(EncounterState.endOfEncounter);
            GetComponent<PlayerInput>().enabled = true;

        }

        //Player Loss
        private void playerLoss()
        {
            EnterEncounterState(EncounterState.endOfEncounter);

            //ANDREW TO DO
            //Replace this with the GameManager Method ReloadScene();
            //I dont know how to access game manager outside of this namespace
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion

        //*** Input ***//
        #region Inputs
        private void onActionInput(InputAction.CallbackContext _context)
        {
            Debug.Log("WAHOO");
            if(m_actionState == ActionState.running)
            {
                ActiveAction?.CheckInput(_context);
            }           
        }
        #endregion
        
        #endregion
    }
}

