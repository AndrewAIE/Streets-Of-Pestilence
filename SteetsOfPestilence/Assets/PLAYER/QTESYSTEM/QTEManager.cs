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

    public class QTEManager : MonoBehaviour
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
        public int AvailableSuccessPoints;
        public int CurrentSuccessPoints;

        #endregion

        //*** Timers ***//
        #region Timers
        public float Timer;        
        public float BeginningOfStreamTimeLimit;
        public float BetweenActionTimeLimit;
        public float ActionTimeLimit;
        #endregion

        //*** Enum Variables ***//
        #region Enum Variables
        public ActionState ActionState;        
        public PlayerStance CurrentPlayerStance;
        private EncounterState m_encounterState;

        #endregion

        //*** Player & Enemy ***//
        #region Player & Enemy 
        public GameObject Enemy;
        private GameObject Player;

        #endregion

        #endregion
        //******************** QTE Runner States **********************//        
        public QTEStates CurrentState;

        public EncounterStart EncounterStart = new EncounterStart();
        public StreamStart StreamStart = new StreamStart();
        public ActionActive ActionActive = new ActionActive();
        public BetweenActions BetweenActions = new BetweenActions();
        public BetweenStreams BetweenStreams = new BetweenStreams();

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
            Player = gameObject;            
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
            CurrentState.StateUpdate(Timer);                    
        }

        #endregion

        //*** Loading Encoutner Data ***//
        #region LoadingEncounterData
        public void LoadEncounter(QTEEncounterData _encounterData, GameObject _enemy)
        {
            //load data from enemy encounter data and start encounter            
            EncounterData = _encounterData;            
            Enemy = _enemy;
            EnterStance(QTEManager.PlayerStance.NeutralStance);
            QteDisplay.ActivatePoiseBar();
            PoiseBar.ResetPoise();

            //Load Stream Data
            ActiveStreamData = EncounterData.NeutralStreamData;            
            WaitingStreams = new List<QTEStreamData>();
            for (int i = 0; i < EncounterData.NeutralStreamData.Count; i++)
            {
                WaitingStreams.Add(Instantiate(ActiveStreamData[i]));
            }
            SelectStream();                      

            //Set Encounter State and begin Encouner
            CurrentState = EncounterStart;
            CurrentState.EnterState(this);
        }

        public void SelectStream()
        {
            if(WaitingStreams.Count == 0)
            {
                ActiveStream = Instantiate(ActiveStream);
            }
            else
            {
                ActiveStream = Instantiate(SelectRandomStream());
            }            
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

        public void EndOfEncounter()
        {
            QteDisplay.DeactivatePoiseBar();
            QteDisplay.DeactivatePanels();
            WaitingStreams.Clear();
            ActiveStream = null;
            this.enabled = false;
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
        }

        //Comment
        public QTEStreamData SelectRandomStream()
        {            
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
        public void ActivateStreamPanels(List<QTEInput> _streamInputs)
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
            QteDisplay.DeactivatePanels();
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

        public QTEAction CreateAction()
        {
            return ActiveAction = Instantiate(ActiveStream.Actions[StreamPosition]);
        }

        public void DestroyAction()
        {
            Destroy(ActiveAction);
        }

        #endregion

        //*** Poise Bar and Combat Outcome ***//
        #region Poise Bar and Combat Outcome

        //Comment
        public void PoiseValueCheck()
        {
            float successRate = (float)CurrentSuccessPoints/(float)AvailableSuccessPoints;            
            int poiseChange;
            switch(successRate)
            {
                case 0:
                    poiseChange = -2;                    
                    break;
                case < 0.5f:
                    poiseChange = -1;                    
                    break;
                case 0.5f:
                    poiseChange = 0;                    
                    break;
                case 1:
                    poiseChange = 2;                    
                    break;
                case > 0.5f:
                    poiseChange = 1;                   
                    break;
                default:
                    poiseChange = 0;                    
                    break;
            }

            //adjust poise value based of successes and falures in stream
            PoiseBar.SetPoise(poiseChange);
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
            EndOfEncounter();
            GetComponent<PlayerInput>().enabled = true;
        }

        //Player Loss
        private void playerLoss()
        {
            EndOfEncounter();

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
            if(_context.performed)
            {
                QteDisplay.Input(_context.action.name);
                if (ActionState == ActionState.running)
                {
                    ActiveAction?.CheckInput(_context);
                }
            }
            if(_context.canceled)
            {
                 QteDisplay.InputReleased(_context.action.name);
                ActiveAction?.OnRelease(_context);
            }
                    
        }

        internal void ResetStreamData()
        {
            StreamPosition = 0;
            Timer = 0;            
            ActiveDisplayList.Clear();            
        }
        #endregion

        #endregion
    }
}

