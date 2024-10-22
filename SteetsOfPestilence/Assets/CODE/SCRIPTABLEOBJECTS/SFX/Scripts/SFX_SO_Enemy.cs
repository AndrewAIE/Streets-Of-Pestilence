using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Enemy", menuName = "SFX SO/Enemy", order = 0)]
public class SFX_SO_Enemy : ScriptableObject
{
    [SerializeField] AudioClip[] sfx_enemy_idle;
    [SerializeField] AudioClip[] sfx_enemy_burn;
    [SerializeField] AudioClip[] sfx_enemy_midCombat;
    [SerializeField] AudioClip[] sfx_enemy_OnAttack;
    [SerializeField] AudioClip[] sfx_enemy_OnDeath;
    [SerializeField] AudioClip[] sfx_enemy_OnDefeatPlayer;
    [SerializeField] AudioClip[] sfx_enemy_OnLoseTrackOfPlayer;
    [SerializeField] AudioClip[] sfx_enemy_OnSpotPlayer;
    [SerializeField] AudioClip[] sfx_enemy_OnSurprisePlayer;
}
