using UnityEngine;

public class QTEAudio : MonoBehaviour
{
    private AudioSource m_audioSource;
    [SerializeField]
    private AudioClip m_successNoise, m_failNoise, m_incorrectInput;

    // Start is called before the first frame update
    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void SuccessfulInput()
    {
        Debug.Log("SCHWING");
        //m_audioSource.PlayOneShot(m_successNoise);
    }

    public void FailAction()
    {
        Debug.Log("WOMP WOMP");
        //m_audioSource.PlayOneShot(m_failNoise);
    }

    public void IncorrectInput()
    {
        Debug.Log("BRZZZZT");
        //m_audioSource.PlayOneShot(m_incorrectInput);
    }



}
