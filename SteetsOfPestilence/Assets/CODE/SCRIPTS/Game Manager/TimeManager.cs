using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float m_slowMoSpeed, m_slowMoChangeRate;
    private float m_timeScale;
    private bool timeSlowDown;
    private bool timeSpeedUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSlowDown)
        {
            m_timeScale = Mathf.MoveTowards(m_timeScale, m_slowMoSpeed, m_slowMoChangeRate * Time.unscaledDeltaTime);
            if (m_timeScale <= m_slowMoSpeed)
            {
                m_timeScale = m_slowMoSpeed;
                timeSlowDown = false;
            }
            Time.timeScale = m_timeScale;

        }
        if (timeSpeedUp)
        {
            m_timeScale = Mathf.MoveTowards(m_timeScale, 1, m_slowMoChangeRate * Time.unscaledDeltaTime);
            if (m_timeScale >= 1)
            {
                m_timeScale = 1;
                timeSpeedUp = false;
            }
            Time.timeScale = m_timeScale;
        }
    }

    public void TimeSlowDown()
    {
        timeSpeedUp = false;
        timeSlowDown = true;
    }

    public void TimeSpeedUp()
    {
        timeSlowDown = false;
        timeSpeedUp = true;
    }
}
