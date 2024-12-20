using Management;
using PlayerController;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    [SerializeField]
    private float m_slowMoSpeed, m_slowMoChangeRate;
    private float m_timeScale = 1;
    private bool timeSlowDown = false;
    private bool timeSpeedUp = false;

    private CameraController _camera;

    private GameManager m_gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _camera = FindObjectOfType<CameraController>();
        m_gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_gameManager.GetGameState() == GameState.Paused)
        {
            return;
        }
        if (timeSlowDown)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, m_slowMoSpeed, m_slowMoChangeRate * Time.unscaledDeltaTime);            
            if (Time.timeScale <= m_slowMoSpeed)
            {
                Time.timeScale = m_slowMoSpeed;
                timeSlowDown = false;
            }            
        }
        if (timeSpeedUp)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1, m_slowMoChangeRate * Time.unscaledDeltaTime);
            if (Time.timeScale >= 1)
            {                
                Time.timeScale = 1;
                timeSpeedUp = false;
            }            
        }
    }

    public void TimeSlowDown()
    {
        timeSpeedUp = false;
        timeSlowDown = true;
        _camera.PP_Slowmo_On();
    }

    public void TimeSpeedUp()
    {
        timeSlowDown = false;
        timeSpeedUp = true;
        _camera.PP_Slowmo_Off();
    }

    public void CancelTimeWarp()
    {
        timeSlowDown = false;
        timeSpeedUp = false;
        _camera.PP_Slowmo_Off();
    }
}
