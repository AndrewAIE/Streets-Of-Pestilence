using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneObjectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_levelLoader;
    [SerializeField]
    private GameObject[] m_enemyObjects;

    public void DeactivateLevels()
    {
        m_levelLoader.SetActive(true);
    }
    public void ActivateEnemies()
    {
        foreach(GameObject enemy in m_enemyObjects)
        {
            enemy.SetActive(true);
        }
    }        
}
