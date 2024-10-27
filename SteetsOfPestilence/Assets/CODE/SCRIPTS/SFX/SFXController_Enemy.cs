using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController_Enemy : MonoBehaviour
{
    [Space]
    [Header("Enemy")]
    [SerializeField] SFX_SO_Enemy _enemyData;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] EnemySFXMode enemySFXMode;

    [Header("Idle")]
    [SerializeField] float idleTimer;
    [SerializeField] float idleTimerMax;
    [SerializeField] float idleTimerMin;

    public enum EnemySFXMode
    {
        none,
        idle,
        combat
    }

    public void SetEnemySFXMode(int inputEnemySFXMode)
    {
        enemySFXMode = (EnemySFXMode)inputEnemySFXMode;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemySFXMode)
        {
            case EnemySFXMode.idle:
                EnemyIdleSFX();
                break;

            case EnemySFXMode.combat:
                break;
        }
    }

    private void CreateEnemySFX(AudioClip clip)
    {
        float length = clip.length;
        GameObject audioOneshot = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
        AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = Random.Range(0.8f, 1f);
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        audioSource.Play();

        Destroy(audioOneshot, length);
    }

    /*** Idle ***/
    #region Idle
    private void EnemyIdleSFX()
    {
        if(idleTimer <= 0)
        {
            Play_Enemy_Idle();
            idleTimer = Random.Range(idleTimerMin, idleTimerMax);
        }
        else
            idleTimer -= Time.deltaTime;
    }

    public void Play_Enemy_Idle()
    {
        CreateEnemySFX(_enemyData.sfx_enemy_idle[Random.Range(0, _enemyData.sfx_enemy_idle.Length)]);
    }

    #endregion

    public void Play_Enemy_Burn()
    {
        CreateEnemySFX(_enemyData.sfx_enemy_burn[Random.Range(0, _enemyData.sfx_enemy_burn.Length)]);
    }

    // Method to play a random "mid-combat" sound effect for the enemy
    public void Play_Enemy_MidCombat()
    {
        // Select a random mid-combat sound from the sfx_enemy_midCombat array and play it
        CreateEnemySFX(_enemyData.sfx_enemy_midCombat[Random.Range(0, _enemyData.sfx_enemy_midCombat.Length)]);
    }

    // Method to play a random "on attack" sound effect for the enemy
    public void Play_Enemy_OnAttack()
    {
        // Select a random attack sound from the sfx_enemy_OnAttack array and play it
        CreateEnemySFX(_enemyData.sfx_enemy_OnAttack[Random.Range(0, _enemyData.sfx_enemy_OnAttack.Length)]);
    }

    // Method to play a random "on death" sound effect for the enemy
    public void Play_Enemy_OnDeath()
    {
        // Select a random death sound from the sfx_enemy_OnDeath array and play it
        CreateEnemySFX(_enemyData.sfx_enemy_OnDeath[Random.Range(0, _enemyData.sfx_enemy_OnDeath.Length)]);
    }

    // Method to play a random "on defeat player" sound effect for the enemy
    public void Play_Enemy_OnDefeatPlayer()
    {
        // Select a random sound for defeating the player from the sfx_enemy_OnDefeatPlayer array and play it
        CreateEnemySFX(_enemyData.sfx_enemy_OnDefeatPlayer[Random.Range(0, _enemyData.sfx_enemy_OnDefeatPlayer.Length)]);
    }

    // Method to play a random "on lose track of player" sound effect for the enemy
    public void Play_Enemy_OnLoseTrackOfPlayer()
    {
        // Select a random sound for losing track of the player from the sfx_enemy_OnLoseTrackOfPlayer array and play it
        CreateEnemySFX(_enemyData.sfx_enemy_OnLoseTrackOfPlayer[Random.Range(0, _enemyData.sfx_enemy_OnLoseTrackOfPlayer.Length)]);
    }

    // Method to play a random "on spot player" sound effect for the enemy
    public void Play_Enemy_OnSpotPlayer()
    {
        // Select a random sound for spotting the player from the sfx_enemy_OnSpotPlayer array and play it
        CreateEnemySFX(_enemyData.sfx_enemy_OnSpotPlayer[Random.Range(0, _enemyData.sfx_enemy_OnSpotPlayer.Length)]);
    }

    // Method to play a random "on surprise player" sound effect for the enemy
    public void Play_Enemy_OnSurprisePlayer()
    {
        // Select a random sound for surprising the player from the sfx_enemy_OnSurprisePlayer array and play it
        CreateEnemySFX(_enemyData.sfx_enemy_OnSurprisePlayer[Random.Range(0, _enemyData.sfx_enemy_OnSurprisePlayer.Length)]);
    }
}
