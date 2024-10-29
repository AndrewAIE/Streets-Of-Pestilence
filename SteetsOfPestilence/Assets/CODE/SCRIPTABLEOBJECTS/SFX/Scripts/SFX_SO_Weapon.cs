using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Weapon", menuName = "SFX SO/Weapon", order = 0)]
public class SFX_SO_Weapon : ScriptableObject
{
    [Header("Metal Clash")]
    [SerializeField] public AudioClip[] SFX_Weapon_MetalClash_clips;
    [Space]
    [SerializeField, Range(0, 1)] public float SFX_Weapon_MetalClash_volumeMax;
    [SerializeField, Range(0, 1)] public float SFX_Weapon_MetalClash_volumeMin;
    [SerializeField, Range(0, 4)] public float SFX_Weapon_MetalClash_pitchMax;
    [SerializeField, Range(0, 4)] public float SFX_Weapon_MetalClash_pitchMin;
    [SerializeField] public float SFX_Weapon_MetalClash_rangeMax;
    [SerializeField] public float SFX_Weapon_MetalClash_rangeMin;

    [Header("Light Swing")]
    [SerializeField] public AudioClip[] SFX_Weapon_LightSwing;
    [SerializeField] public AudioClip[] SFX_Weapon_HeavySwing;
    [Space]
    [SerializeField, Range(0, 1)] public float SFX_Weapon_Swing_volumeMax;
    [SerializeField, Range(0, 1)] public float SFX_Weapon_Swing_volumeMin;
    [SerializeField, Range(0, 4)] public float SFX_Weapon_Swing_pitchMax;
    [SerializeField, Range(0, 4)] public float SFX_Weapon_Swing_pitchMin;
    [SerializeField] public float SFX_Weapon_Swing_rangeMax;
    [SerializeField] public float SFX_Weapon_Swing_rangeMin;
}
