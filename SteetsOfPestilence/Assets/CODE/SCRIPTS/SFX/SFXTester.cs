using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXTester : MonoBehaviour
{
    [SerializeField] SFXController_Player controller;

    [Header("Footstep")]
    [SerializeField] bool is_running = false;
    [Space]
    [SerializeField] bool is_walking = false;
    [Space]
    [SerializeField] bool is_stone = true;
    [Space]
    [SerializeField] float footstepTimer;
    [Space]
    [SerializeField] float walk_footstepTimerMax;
    [SerializeField] float walk_footstepTimerMin;
    [Space]
    [SerializeField] float run_footstepTimerMax;
    [SerializeField] float run_footstepTimerMin;

    [Header("Enemy")]
    [SerializeField] GameObject placeHolderEnemy;

    private void Awake()
    {
        controller = GetComponent<SFXController_Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*** Footsteps ***/
        #region Footsteps
        if (is_walking)
        {
            if (footstepTimer <= 0)
            {
                if (is_stone)
                {
                    controller.Play_Footstep_Walk_Stone();
                }
                else
                {
                    controller.Play_Footstep_Walk_Mud();
                }

                footstepTimer = Random.Range(walk_footstepTimerMin, walk_footstepTimerMax);
            }
            else
            {
                footstepTimer -= Time.deltaTime;
            }
        }

        if (is_running)
        {
            if (footstepTimer <= 0)
            {
                if (is_stone)
                {
                    controller.Play_Footstep_Run_Stone();
                }
                else
                {
                    controller.Play_Footstep_Run_Mud();
                }

                footstepTimer = Random.Range(run_footstepTimerMin, run_footstepTimerMax);
            }
            else
            {
                footstepTimer -= Time.deltaTime;
            }
        }

        #endregion
    }

    public void Set_isWalking(bool inputBool)
    {
        is_walking = inputBool;
    }

    public void Set_isRunning(bool inputBool)
    {
        is_running = inputBool;
    }

    public void Set_isStone(bool inputBool)
    {
        is_stone = inputBool;
    }
}
