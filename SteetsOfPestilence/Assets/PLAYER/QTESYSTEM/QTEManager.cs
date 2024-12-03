using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EnemyAI;
using PlayerController;
using Management;


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
        RightTrigger
    }

    #endregion

    public class QTEManager : MonoBehaviour
    {
        //******************** Variables *******************//
        #region Variables
        //*** Manager ***//
        GameManager m_manager;
        bool m_paused = false;

        //*** QTE DATA ***//
        #region QTE Data
        
        private InputActionMap m_actionMap;
        
        public QTEInputs InputActions;        
        
        public QTEEncounterData EncounterData;
        
        public List<QTEStreamData> ActiveStreamData;
        
        public List<QTEStreamData> WaitingStreams;
       
        public QTEStreamData ActiveStream;
        
        public QTEAction ActiveAction;

        public QTECombatAnimation CombatAnimation;
        
        [Tooltip("QTE Display")]
        public QTEDisplay QteDisplay;

        public List<QTEInput> ActiveDisplayList;        
        [SerializeField] private float m_canvasFadeDuration;

        
        #endregion

        //*** Poise Bar ***//
        #region Poise Bar
        public PoiseBarController PoiseBar;        

        

        //Poise variables
        public int ChangeInPoiseValue;
        public int AvailableSuccessPoints;
        public int CurrentSuccessPoints;
        [SerializeField]
        private int m_defaultPoiseChangeValue = 2;
        private float m_poiseChangeValue;

        #endregion

        //Combat Variables
        public int StreamPosition;
        private bool m_isBossFight = false;
        private bool m_bossPhaseOneComplete = false;

        //*** Timers ***//
        #region Timers
        public float Timer;        
        public float BeginningOfStreamTimeLimit;
        public float BetweenActionTimeLimit;
        public float ActionTimeLimit;
        [SerializeField]
        private SlowMotionManager m_slowMoManager;
        #endregion

        //*** Enum Variables ***//
        #region Enum Variables
        public ActionState ActionState;        
        public PlayerStance CurrentPlayerStance;        

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
        public CombatAnimation CombatAnim = new CombatAnimation();

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

        
        #region Awake, Enable, Disable

        private void Awake()
        {
            InputActions = new QTEInputs();
            ActiveDisplayList = new List<QTEInput>();
            CombatAnimation = GetComponentInChildren<QTECombatAnimation>();
            m_slowMoManager = GetComponentInChildren<SlowMotionManager>();
            Player = GetComponentInParent<PlayerManager>();
            m_manager = FindFirstObjectByType<GameManager>();
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

        #region Update
        void Update()
        {
            //check for pause functionality
            if (m_manager.m_Gamestate == GameState.Paused && !m_paused)
            {
                m_paused = true;
                QteDisplay.Pause();
                InputActions.Disable();
            }
            if(m_manager.m_Gamestate !=GameState.Paused && m_paused)
            {
                m_paused = false;
                QteDisplay.Resume();
                InputActions.Enable();
            }
            if (!m_paused)
            {
                Timer += Time.unscaledDeltaTime;
            }            
            CurrentState.StateUpdate(Timer);
        }
        #endregion
        
        #region LoadingEncounterData
        public void LoadEncounter(QTEEncounterData _encounterData, EnemyController _enemy)
        {
            //load data from enemy encounter data and start encounter            
            EncounterData = _encounterData;            
            Enemy = _enemy;
            EnterStance(PlayerStance.NeutralStance);
            QteDisplay.ActivatePoiseBar();            
            LoadUI(_enemy.m_EType);
            //check if boss fight
            if(_enemy.m_EType == EnemyType.Boss)
            {
                m_isBossFight = true;
            }
            //Load Stream Data
            ActiveStreamData = EncounterData.NeutralStreamData;            
            WaitingStreams = new List<QTEStreamData>();
            for (int i = 0; i < EncounterData.NeutralStreamData.Count; i++)
            {
                WaitingStreams.Add(Instantiate(ActiveStreamData[i]));
            }
            SelectStream();
            SetQTEAnimators();
            //reset all poise data
            PoiseBar.gameObject.SetActive(true);
            PoiseBar.ResetPoise();
            m_poiseChangeValue = m_defaultPoiseChangeValue;
            Timer = 0;
            //Set Encounter State and begin Encounter
            CurrentState = EncounterStart;
            CurrentState.EnterState(this);
            
        }

        public void LoadBossEncounterTwo()
        {
            m_bossPhaseOneComplete = true;
            
            EncounterData = Enemy.transform.parent.GetComponentInChildren<BossSecondPhaseData>().SecondPhaseData;
            ActiveStreamData = EncounterData.NeutralStreamData;
            ActiveStream = null;
            WaitingStreams = new List<QTEStreamData>();
            for (int i = 0; i < EncounterData.NeutralStreamData.Count; i++)
            {
                WaitingStreams.Add(Instantiate(ActiveStreamData[i]));
            }
            SelectStream();
            //reset poise bar           
            PoiseBar.ResetPoise();
            m_poiseChangeValue = m_defaultPoiseChangeValue;
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

        public void SetQTEAnimators()
        {
            Animator player = Player.GetComponentInChildren<Animator>();
            Animator enemy = Enemy.transform.parent.GetComponentInChildren<Animator>();            
            CombatAnimation.SetQTEAnimations(player, enemy);
        }

        public void SelectQTECombatAnimations()
        {
            CombatAnimation.SelectAnimation(PoiseBar._poise);
        }

        public void LoadUI(EnemyType _enemyType)
        {
            QteDisplay.LoadUI(_enemyType);            
        }

        public void EndOfEncounter()
        {
            if(QteDisplay.FinishingCues.Count > 0)
            {
                StartCoroutine(DeleteCues());
            }
            if (m_isBossFight)
            {
                m_isBossFight = false;
                m_bossPhaseOneComplete = false;
            }
            
            Enemy.EndCombat();            
            WaitingStreams.Clear();            
            this.enabled = false;           
        }

        public void ReactivatePlayer()
        {
            QteDisplay.DeactivatePoiseBar();
            QteDisplay.DeactivatePanels();
            Player.SetPlayerActive(true);
            ActiveStream = null;
        }

        #endregion
        
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
            //Clear tweens from previous stream
            QteDisplay.ClearTweens();
            //Select random stream from unselected streams
            int selector = Random.Range(0, WaitingStreams.Count);
            QTEStreamData selectedStream = Instantiate(WaitingStreams[selector]);
            WaitingStreams.RemoveAt(selector);
            if(ActiveStream)
            {
                WaitingStreams.Add(ActiveStream);
            }
            return selectedStream;
        }

        public void ActivateInputCues()
        {
            foreach (QTEStreamData streamData in ActiveStreamData)
            {
                for(int i = 0; i < streamData.Actions.Count; i++)
                {
                    Debug.Log($"Creating Action {i} for {streamData.name}");
                    streamData.Actions[i].CreateInputRings();
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
        
        #region Combat and Poise Bar

        //Comment
        public void PoiseValueCheck()
        {
            float successRate = (float)CurrentSuccessPoints/(float)AvailableSuccessPoints;            
            int poiseChange;
            m_poiseChangeValue += 0.3f;
            Debug.Log($"Poise = {m_poiseChangeValue}");
            switch(successRate)
            {
                case 0:
                    poiseChange = (int)-m_poiseChangeValue * 2;                    
                    break;
                case < 0.5f:
                    poiseChange = (int)-m_poiseChangeValue;                    
                    break;
                case 0.5f:
                    poiseChange = 0;                    
                    break;
                case 1:
                    poiseChange = (int)m_poiseChangeValue * 2;                    
                    break;
                case > 0.5f:
                    poiseChange = (int)m_poiseChangeValue;                   
                    break;
                default:
                    poiseChange = 0;                    
                    break;
            }
            //load second phase of boss fight if necessary           
            
            //adjust poise value based of successes and falures in stream 
            PoiseBar.SetPoise(poiseChange);
            if (PoiseBar._poise >= PoiseBar._maxPoise)
            {
                if (m_isBossFight && !m_bossPhaseOneComplete)
                {
                    LoadBossEncounterTwo();                    
                    return;
                }
                playerWin();
            }
            if (PoiseBar._poise <= PoiseBar._minPoise)
            {
                playerLoss();
            }            
        }
        
        //Player Win
        private void playerWin()
        {
            
            CombatAnimation.PlayAnimation("PlayerWin");            
            EndOfEncounter();
            Invoke("ReactivatePlayer", 3.8f);
        }

        //Player Loss
        private void playerLoss()
        {            
            Enemy.DisableEnemy();
            EndOfEncounter();
            CombatAnimation.PlayAnimation("EnemyWin");
            
        }       

        public void SlowTime(bool _activate)
        {
            if(_activate)
            {                
                m_slowMoManager.TimeSlowDown();
                return;
            }
            m_slowMoManager.TimeSpeedUp();
        }

        public void ResetAnimationState()
        {
            CombatAnimation.EndState = true;
            CombatAnimation.ResetTriggers();            
        }   

        public void FadeInUI()
        {
            QteDisplay.FadeInUI(m_canvasFadeDuration);
        }

        public void FadeOutUI()
        {
            QteDisplay.FadeOutUI(m_canvasFadeDuration);
        }
        #endregion

        
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
                QteDisplay.ResetAllActiveIconColours();
                ActiveAction?.OnRelease(_context);
            }                    
        }

        public void ResetStreamData()
        {
            StreamPosition = 0;
            Timer = 0;            
            ActiveDisplayList.Clear();
            StartCoroutine(DeleteCues());
        }

        private IEnumerator DeleteCues()
        {
            yield return new WaitForSecondsRealtime(0.35f);            
            int count = QteDisplay.FinishingCues.Count;
            //Debug.Log($"Number of rings to be removed {count}");
            for (int i = 0; i < count; i++)
            {                
                GameObject holder = QteDisplay.FinishingCues[0];
                QteDisplay.FinishingCues.Remove(holder);
                Destroy(holder);
                //Debug.Log($"Removing ring {i + 1}");
            }
        }
        #endregion

        #endregion
    }
}

