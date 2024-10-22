using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using EnemyAI;
using PlayerController;


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
        [SerializeField]
        public int m_poiseChangeValue = 2;
        

        #endregion

        //*** Timers ***//
        #region Timers
        public float Timer;        
        public float BeginningOfStreamTimeLimit;
        public float BetweenActionTimeLimit;
        public float ActionTimeLimit;

        public TimeManager TimerManager;
        #endregion

        //*** Enum Variables ***//
        #region Enum Variables
        public ActionState ActionState;        
        public PlayerStance CurrentPlayerStance;
        private EncounterState m_encounterState;

        #endregion

        //*** Player & Enemy ***//
        #region Player & Enemy 
        public EnemyController Enemy;
        private PlayerManager Player;

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
            Player = GetComponent<PlayerManager>();            
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
            Timer += Time.unscaledDeltaTime;
            CurrentState.StateUpdate(Timer);            
        }
        #endregion

        //*** Loading Encounter Data ***//
        #region LoadingEncounterData
        public void LoadEncounter(QTEEncounterData _encounterData, EnemyController _enemy)
        {
            //load data from enemy encounter data and start encounter            
            EncounterData = _encounterData;            
            Enemy = _enemy;
            EnterStance(QTEManager.PlayerStance.NeutralStance);
            QteDisplay.ActivatePoiseBar();
            PoiseBar.ResetPoise();
            LoadUI(_enemy.m_EType);
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

        public void LoadUI(EnemyAI.EnemyType _enemyType)
        {
            QteDisplay.LoadUI(_enemyType);
        }

        public void EndOfEncounter()
        {
            QteDisplay.DeactivatePoiseBar();
            QteDisplay.DeactivatePanels();
            Player.SetPlayerActive(true);
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

        public void ActivateInputCues(List<QTEInput> _streamInputs)
        {
            foreach (QTEInput input in _streamInputs)
            {
                QteDisplay.CreateInputPrompt(input);
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
                    poiseChange = -m_poiseChangeValue * 2;                    
                    break;
                case < 0.5f:
                    poiseChange = -m_poiseChangeValue;                    
                    break;
                case 0.5f:
                    poiseChange = 0;                    
                    break;
                case 1:
                    poiseChange = m_poiseChangeValue * 2;                    
                    break;
                case > 0.5f:
                    poiseChange = m_poiseChangeValue;                   
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
            Player.KillPlayer();            
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

        public void ResetStreamData()
        {
            StreamPosition = 0;
            Timer = 0;            
            ActiveDisplayList.Clear();
            Invoke("DeleteCues", 0.35f);
        }

        private void DeleteCues()
        {
            for (int i = 0; i < QteDisplay.FinishingCues.Count; i++)
            {
                GameObject holder = QteDisplay.FinishingCues[0];
                QteDisplay.FinishingCues.Remove(holder);
                Destroy(holder);
            }
        }
        #endregion

        #endregion
    }
}

