using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Footstep", menuName = "SFX SO/Footstep", order = 0)]
public class SFX_SO_Footstep : ScriptableObject
{
    [Header("Stone")]
    [SerializeField] public AudioClip[] SFX_footstep_stone_walk;
    [SerializeField] public AudioClip[] SFX_footstep_stone_run;
    [Header("Run")]
    [SerializeField] public AudioClip[] SFX_footstep_mud_walk;
    [SerializeField] public AudioClip[] SFX_footstep_mud_run;
}
