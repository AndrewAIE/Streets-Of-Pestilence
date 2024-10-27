using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Enemy", menuName = "SFX SO/Enemy", order = 0)]
public class SFX_SO_Enemy : ScriptableObject
{
    [SerializeField] public AudioClip[] sfx_enemy_idle;
    [SerializeField] public AudioClip[] sfx_enemy_burn;
    [SerializeField] public AudioClip[] sfx_enemy_midCombat;
    [SerializeField] public AudioClip[] sfx_enemy_OnAttack;
    [SerializeField] public AudioClip[] sfx_enemy_OnDeath;
    [SerializeField] public AudioClip[] sfx_enemy_OnDefeatPlayer;
    [SerializeField] public AudioClip[] sfx_enemy_OnLoseTrackOfPlayer;
    [SerializeField] public AudioClip[] sfx_enemy_OnSpotPlayer;
    [SerializeField] public AudioClip[] sfx_enemy_OnSurprisePlayer;
}
