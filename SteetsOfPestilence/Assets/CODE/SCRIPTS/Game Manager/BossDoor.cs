using UnityEngine;

public class BossDoor : MonoBehaviour
{
    private int m_unlocked = 0;

    [SerializeField] private GameObject m_openDoor, m_closedDoor;

    [SerializeField] private GameObject[] m_lockObjects;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) UnlockLock();
    }

    public void UnlockLock()
    {
        m_unlocked++;

        for(int i = 0; i < m_lockObjects.Length; i++)
        {
            if (i < m_unlocked) m_lockObjects[i].SetActive(true);
            else m_lockObjects[i].SetActive(false);
        }


        if(m_unlocked >= m_lockObjects.Length)
        {
            OpenDoor();
        }
    }
    private void OpenDoor()
    {
        m_closedDoor.SetActive(false);
        m_openDoor.SetActive(true);
    }

}
