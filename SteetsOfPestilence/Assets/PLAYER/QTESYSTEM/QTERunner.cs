using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;


namespace QTESystem
{

    public class QTERunner : MonoBehaviour
    {
        public QTEInputs InputActions;
        private InputActionMap m_actionMap;
        
        private QTEEncounterData m_encounterData;
        private List<QTEStreamData> m_activeStreamData;
        private List<QTEStreamData> m_waitingStreams;
        private QTEStreamData m_activeStream;        
        private QTEAction m_activeAction;
        


        [SerializeField]
        private QTEDisplay m_qteDisplay;
        private List<QTEInput> m_activeDisplayList;

        private int m_streamPosition;
        private int m_poiseValue = 0;
        private int m_changeInPoiseValue;
        
        private float m_timer;        
        private float m_beginningOfStreamTimeLimit;
        private float m_betweenActionTimeLimit;
        
        private ActionState m_actionState;        
        private GameObject m_enemy;
        private GameObject m_player;

        float m_actionTimer;

        private enum PlayerStance
        {
            NeutralStance,
            OffensiveStance,
            DefensiveStance
        }
        private PlayerStance m_playerStance;

        private enum EncounterState
        {
            beginningOfEncounter,
            beginningOfStream,
            actionActive,
            betweenActions,            
            betweenStreams,            
            endOfEncounter            
        }
        private EncounterState m_encounterState;

        private void Awake()
        {
            InputActions = new QTEInputs();
            m_activeDisplayList = new List<QTEInput>();
            m_player = GetComponent<GameObject>();
            
        }

        private void Start()
        {            
                     
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

        // Update is called once per frame
        void Update()
        {
            m_timer += Time.deltaTime;
            switch (m_encounterState)
            {
                case EncounterState.beginningOfEncounter:
                    beginningOfEncounter();
                    break;
                case EncounterState.beginningOfStream:
                    beginningOfStream();
                    break;
                case EncounterState.actionActive:
                    activeAction();
                    break;
                case EncounterState.betweenActions:
                    betweenActions();                    
                    break;                
                case EncounterState.betweenStreams:
                    betweenStreams();                    
                    break;
                case EncounterState.endOfEncounter:
                    endOfEncounter();
                    break;
                default:
                    break;            
            }            
        }
        

        #region LoadingEncounterData
        public void LoadEncounter(QTEEncounterData _encounterData, GameObject _enemy)
        {
            //load data from encounter and start the neutral stream            
            m_encounterData = _encounterData;            
            m_enemy = _enemy;
            EnterEncounterState(EncounterState.beginningOfEncounter);            
        }

        public List<QTEInput> GetStreamActionInputs()
        {
            List<QTEInput> actions = new List<QTEInput>();
            for (int i = 0; i < m_activeStream.Actions.Count; i++)
            {
                for (int j = 0; j < m_activeStream.Actions[i].InputList.Count; j++)
                {
                    actions.Add(m_activeStream.Actions[i].InputList[j]);
                }
            }
            return actions;
        }

        #endregion
        #region EncounterStates
        private void EnterEncounterState(EncounterState _encounterState)
        {
            m_encounterState = _encounterState;
            switch(m_encounterState)
            {                
                case EncounterState.beginningOfEncounter:

                    EnterStance(PlayerStance.NeutralStance);                                       
                    m_qteDisplay.ActivatePoiseBar();
                    m_poiseValue = 0;
                    m_qteDisplay.UpdatePoiseBar(m_poiseValue);                    
                    m_waitingStreams = new List<QTEStreamData>();
                    for (int i = 0; i < m_encounterData.NeutralStreamData.Count; i++)
                    {
                        m_waitingStreams.Add(Instantiate(m_activeStreamData[i]));
                    }
                    break;

                case EncounterState.beginningOfStream:

                    //reset change in poise bar value and stream iterator                    
                    m_changeInPoiseValue = 0;
                    m_streamPosition = 0;
                    //select approp
                    //activate appropriate panels in QTE Display
                    
                    m_activeStream = selectRandomStream();
                    activateStreamPanels(GetStreamActionInputs());                    
                    
                    //set new timer data and set timer to 0                    
                    m_beginningOfStreamTimeLimit = m_activeStream.BeginningOfStreamPause;                    
                    m_timer = 0;
                    break;

                case EncounterState.actionActive:

                    //set active action, set time limit and controls and activate appropriate display, enable controls
                    m_timer = 0;
                    m_activeAction = m_activeStream.Actions[m_streamPosition];
                    m_activeAction.SetTimeLimit(m_activeStream.ActionTimer, m_activeStream.SuccessBuffer);
                    m_activeAction.SetTargetInputs(InputActions);                   
                    m_qteDisplay.ActivateCue();
                    m_actionTimer = m_activeStream.ActionTimer + (m_activeStream.SuccessBuffer / 2f);
                    break;

                case EncounterState.betweenActions:

                    //increase stream iterator, reset display of icons 
                    m_streamPosition++;
                    //set new time limit and reset timer
                    m_activeAction.Started = false;
                    m_activeAction.IncorrectInput = null;
                    m_qteDisplay.SetIconColor(m_activeDisplayList, Color.white);
                    m_betweenActionTimeLimit = m_activeStream.BetweenActionTimer;
                    m_timer = 0;                    
                    break;

                case EncounterState.betweenStreams:

                    //deactivate icon display, reset timer and iterator, adjust poise bar
                    m_qteDisplay.DeactivatePanels();
                    m_streamPosition = 0;
                    m_timer = 0;
                    PoiseValueCheck();
                    m_activeDisplayList.Clear();                    
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
            if(m_timer >= m_beginningOfStreamTimeLimit)
            {
                EnterEncounterState(EncounterState.actionActive);
            }
        }

        private void activeAction()
        {
            // update and get state from active action
            m_actionState = m_activeAction.ActionUpdate(m_timer);
            float cueSize = 1 - (m_timer / m_activeStream.ActionTimer);
            if(m_actionState == ActionState.running)
            {
                m_qteDisplay.SetCueSize(cueSize);
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
            
            if(m_timer >= m_actionTimer)
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
            
            if (m_timer > m_betweenActionTimeLimit)
            {
                if (m_streamPosition >= m_activeStream.Actions.Count)
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
            if (m_timer > m_activeStream.EndOfStreamPause)
            {
                EnterEncounterState(EncounterState.beginningOfStream);
            }
        }

        private void endOfEncounter()
        {            
            m_qteDisplay.DeactivatePoiseBar();
            m_waitingStreams.Clear();
            m_activeStream = null;
            this.enabled = false;
        }

        #endregion
        #region OtherFunctions
        private void actionSuccess()
        {
            //increase poise value and enter between actions state
            m_activeAction.CompleteAction();
            m_changeInPoiseValue++;
            m_qteDisplay.SetIconColor(m_activeAction.InputList, Color.green);
            GameObject Holder = m_qteDisplay.VisualCues[0];
            m_qteDisplay.VisualCues.RemoveAt(0);
            Destroy(Holder);
        }

        private void actionIncorrectInput()
        {
            //decrease poise value and enter between actions state
            m_activeAction.CompleteAction();
            m_changeInPoiseValue--;            
            m_qteDisplay.MissedInput(m_activeAction.InputList);
            m_qteDisplay.IncorrectInput(m_activeAction.IncorrectInput);
            GameObject Holder = m_qteDisplay.VisualCues[0];
            m_qteDisplay.VisualCues.RemoveAt(0);
            Destroy(Holder);
        }

        private void actionTimerFail()
        {
            m_changeInPoiseValue--;            
            m_qteDisplay.MissedInput(m_activeAction.InputList);
            GameObject Holder = m_qteDisplay.VisualCues[0];
            m_qteDisplay.VisualCues.RemoveAt(0);
            Destroy(Holder);
        }
        private void EnterStance(PlayerStance _stance)
        {
            m_playerStance = _stance;
            switch (m_playerStance)
            {
                case PlayerStance.NeutralStance:
                    m_activeStreamData = m_encounterData.NeutralStreamData;
                    break;
                case PlayerStance.OffensiveStance:
                    m_activeStreamData = m_encounterData.OffensiveStreamData;
                    break;
                case PlayerStance.DefensiveStance:
                    m_activeStreamData = m_encounterData.DefensiveStreamData;
                    break;
                default:
                    break;
            }
            //for (int i = 0; i < m_encounterData.NeutralStreamData.Count; i++)
            //{
            //    m_waitingStreams.Add(Instantiate(m_activeStreamData[i]));
            //}
        }

        private QTEStreamData selectRandomStream()
        {             
            Debug.Log(m_waitingStreams.Count);
            int selector = Random.Range(0, m_waitingStreams.Count);

            QTEStreamData selectedStream = Instantiate(m_waitingStreams[selector]);
            m_waitingStreams.RemoveAt(selector);
            if(m_activeStream)
            {
                m_waitingStreams.Add(m_activeStream);
            }
            return selectedStream;
        }

        private void activateStreamPanels(List<QTEInput> _streamInputs)
        {
            bool[] panelActivator = { false, false, false };

            foreach (QTEInput input in _streamInputs)
            {
                m_qteDisplay.CreateInputPrompt(input);
                switch (input)
                {

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
                    m_qteDisplay.ActivatePanel(i);
                    switch(i)
                    {
                        case 0:
                            m_activeDisplayList.Add(QTEInput.LeftTrigger);
                            m_activeDisplayList.Add(QTEInput.RightTrigger);
                            break;
                        case 1:
                            m_activeDisplayList.Add(QTEInput.LeftShoulder);
                            m_activeDisplayList.Add(QTEInput.RightShoulder);
                            break;
                        case 2:
                            m_activeDisplayList.Add(QTEInput.NorthFace);
                            m_activeDisplayList.Add(QTEInput.EastFace);
                            m_activeDisplayList.Add(QTEInput.SouthFace);
                            m_activeDisplayList.Add(QTEInput.WestFace);
                            break;
                    }
                }
            }
        }

        public void PoiseValueCheck()
        {
            //adjust poise value based of successes and falures in stream
            m_poiseValue += m_changeInPoiseValue;

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

            if (m_poiseValue >= 10)
            {
                playerWin();                
            }
            if (m_poiseValue <= -10)
            {
                playerLoss();
            }
            m_qteDisplay.UpdatePoiseBar(m_poiseValue);           
        }
        private void playerWin()
        {            
            Destroy(m_enemy);
            EnterEncounterState(EncounterState.endOfEncounter);
            GetComponent<PlayerInput>().enabled = true;

        }
        private void playerLoss()
        {           
            EnterEncounterState(EncounterState.endOfEncounter);

            //ANDREW TO DO
            //Replace this with the GameManager Method ReloadScene();
            //I dont know how to access game manager outside of this namespace
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        #endregion
        #region Inputs
        private void onActionInput(InputAction.CallbackContext _context)
        {
            Debug.Log("WAHOO");
            if(m_actionState == ActionState.running)
            {
                m_activeAction?.CheckInput(_context);
            }           
        }
        #endregion
    }

    #region PublicEnums
    public enum ActionState
    {
        running,
        success,
        fail,
        complete
    }

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






}

