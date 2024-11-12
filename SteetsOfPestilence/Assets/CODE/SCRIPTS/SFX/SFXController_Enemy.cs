using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController_Enemy : MonoBehaviour
{
    [HideInInspector] SFXController_Player _playerSFXController;

    [Space]
    [Header("Enemy")]
    [SerializeField] SFX_SO_Enemy _enemyData;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] EnemySFXMode enemySFXMode;

    [Header("Idle")]
    [SerializeField] float idleTimer;
    [SerializeField] float idleTimerMax;
    [SerializeField] float idleTimerMin;

    [Header("Last Clip PLayed")]
    [SerializeField] private AudioClip _lastPlayedClip_Idle;
    [SerializeField] private AudioClip _lastPlayedClip_Burn;
    [SerializeField] private AudioClip _lastPlayedClip_MidCombat;
    [SerializeField] private AudioClip _lastPlayedClip_OnAttack;
    [SerializeField] private AudioClip _lastPlayedClip_OnDeath;
    [SerializeField] private AudioClip _lastPlayedClip_OnDefeatPlayer;
    [SerializeField] private AudioClip _lastPlayedClip_OnLoseTrackOfPlayer;
    [SerializeField] private AudioClip _lastPlayedClip_OnSpotPlayer;
    [SerializeField] private AudioClip _lastPlayedClip_OnSurprisePlayer;

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
        _playerSFXController = FindObjectOfType<SFXController_Player>();
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
        audioSource.volume = Random.Range(_enemyData.SFX_Enemy_Voice_volumeMin, _enemyData.SFX_Enemy_Voice_volumeMax);
        audioSource.pitch = Random.Range(_enemyData.SFX_Enemy_Voice_pitchMin, _enemyData.SFX_Enemy_Voice_pitchMax);

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
        _lastPlayedClip_Idle = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_idle, _lastPlayedClip_Idle);
        CreateEnemySFX(_lastPlayedClip_Idle);
    }

    #endregion

    public void Play_Enemy_Burn()
    {
        _lastPlayedClip_Burn = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_burn, _lastPlayedClip_Burn);
        CreateEnemySFX(_lastPlayedClip_Burn);
    }

    public void Play_Enemy_MidCombat()
    {
        _lastPlayedClip_MidCombat = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_midCombat, _lastPlayedClip_MidCombat);
        CreateEnemySFX(_lastPlayedClip_MidCombat);
    }

    public void Play_Enemy_OnAttack()
    {
        _lastPlayedClip_OnAttack = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_OnAttack, _lastPlayedClip_OnAttack);
        CreateEnemySFX(_lastPlayedClip_OnAttack);
    }

    public void Play_Enemy_OnDeath()
    {
        _lastPlayedClip_OnDeath = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_OnDeath, _lastPlayedClip_OnDeath);
        CreateEnemySFX(_lastPlayedClip_OnDeath);
    }

    public void Play_Enemy_OnDefeatPlayer()
    {
        _lastPlayedClip_OnDefeatPlayer = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_OnDefeatPlayer, _lastPlayedClip_OnDefeatPlayer);
        CreateEnemySFX(_lastPlayedClip_OnDefeatPlayer);
    }

    public void Play_Enemy_OnLoseTrackOfPlayer()
    {
        _lastPlayedClip_OnLoseTrackOfPlayer = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_OnLoseTrackOfPlayer, _lastPlayedClip_OnLoseTrackOfPlayer);
        CreateEnemySFX(_lastPlayedClip_OnLoseTrackOfPlayer);
    }

    public void Play_Enemy_OnSpotPlayer()
    {
        _lastPlayedClip_OnSpotPlayer = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_OnSpotPlayer, _lastPlayedClip_OnSpotPlayer);
        CreateEnemySFX(_lastPlayedClip_OnSpotPlayer);
    }

    public void Play_Enemy_OnSurprisePlayer()
    {
        _lastPlayedClip_OnSurprisePlayer = _playerSFXController.GetUniqueClip(_enemyData.SFX_enemy_OnSurprisePlayer, _lastPlayedClip_OnSurprisePlayer);
        CreateEnemySFX(_lastPlayedClip_OnSurprisePlayer);
    }

}
