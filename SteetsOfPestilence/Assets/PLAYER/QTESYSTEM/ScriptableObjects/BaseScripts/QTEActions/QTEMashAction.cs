using UnityEngine;
using QTESystem;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Mash Action")]
public class QTEMashAction : QTEAction
{
    [SerializeField]
    private int m_mashTarget;
    private int m_mashCount = 0;
    [SerializeField]
    private float m_timePerPress;
    protected override ActionState onUpdate()
    {
        //if the timer has reached the time limit without enough mash inputs, return state fail
        if(m_timer >= m_timeLimit + (m_timePerPress * m_mashTarget) && m_state == ActionState.running)
        {            
            if(m_mashCount < m_mashTarget)
            {
                m_qteDisplay.StopFlash();
                
                for (int i = 0; i < InputList.Count; i++)
                {
                    m_qteDisplay.MissedInput(InputList[i], i);
                    m_qteDisplay.DeactivateCue(i);
                }
                return m_state = ActionState.fail;
            }            
        }        
        if(m_timer >= m_maxTime)
        {
            m_timeUp = true;
        }

        return m_state;
    }
    public override void CheckInput(InputAction.CallbackContext _context)
    {
        //check if action is currently running and set input correct to false
        if (m_state != ActionState.running)
            return;
        bool inputCorrect = false;
        //if the correct button is pressed, increase mash count, and set input correct to true
        if (_context.action.name != "Directional")
        {
            for (int i = 0; i < m_readyInputs.Count; i++)
            {
                if (_context.action == m_readyInputs[i])
                {
                    m_mashCount++;
                    inputCorrect = true;
                    m_qteDisplay.SetIconColor(InputList[i], Color.green);
                    //if the mash target has been reached, set state to success
                    if(m_mashCount >= m_mashTarget)
                    {
                        CorrectInputs += InputList.Count;
                        m_qteDisplay.StopFlash();                        
                        m_state = ActionState.success;
                        for (int j = 0; j < InputList.Count; j++)
                        {
                            m_qteDisplay.SuccessfulInput(InputList[j], j);
                        }
                    }
                    break;
                }
            } 
            //if the incorrect button is pressed, fail the action
            if (inputCorrect == false)
            {
                m_qteDisplay.StopFlash();
                for (int j = 0; j < InputList.Count; j++)
                {
                    m_qteDisplay.DeactivateCue(j);
                }                
                m_qteDisplay.MissedInput(InputList);
                m_qteDisplay.IncorrectInput(_context.action.name);                
                m_state = ActionState.fail;
            }
        }
    }

    public override void OnRelease(InputAction.CallbackContext _context)
    {

    }
    protected override bool checkSuccessWindow()
    {
        return false;
    }
    protected override void onStart()
    {
        m_maxTime = (m_timeLimit * 2) + (m_timePerPress * m_mashTarget);
        for (int i = 0; i < InputList.Count; i++)
        {
            m_qteDisplay.AnimateMashCue(m_timeLimit, i, InputList[i]);
        }
    }

    public override void CreateInputRings()
    {
        
    }
}

    
    
