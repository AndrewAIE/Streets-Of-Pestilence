using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Enemy", menuName = "SFX SO/Enemy", order = 0)]
public class SFX_SO_Enemy : ScriptableObject
{
    [SerializeField] public AudioClip[] SFX_enemy_idle;
    [SerializeField] public AudioClip[] SFX_enemy_burn;
    [SerializeField] public AudioClip[] SFX_enemy_midCombat;
    [SerializeField] public AudioClip[] SFX_enemy_OnAttack;
    [SerializeField] public AudioClip[] SFX_enemy_OnDeath;
    [SerializeField] public AudioClip[] SFX_enemy_OnDefeatPlayer;
    [SerializeField] public AudioClip[] SFX_enemy_OnLoseTrackOfPlayer;
    [SerializeField] public AudioClip[] SFX_enemy_OnSpotPlayer;
    [SerializeField] public AudioClip[] SFX_enemy_OnSurprisePlayer;

    [Header("Variables")]
    [SerializeField] public float SFX_Enemy_Voice_volumeMin;
    [SerializeField] public float SFX_Enemy_Voice_volumeMax;
    [SerializeField] public float SFX_Enemy_Voice_pitchMin;
    [SerializeField] public float SFX_Enemy_Voice_pitchMax;
}
