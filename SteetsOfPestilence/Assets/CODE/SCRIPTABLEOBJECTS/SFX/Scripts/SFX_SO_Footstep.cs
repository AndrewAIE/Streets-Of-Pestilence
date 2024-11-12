using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Footstep", menuName = "SFX SO/Footstep", order = 0)]
public class SFX_SO_Footstep : ScriptableObject
{
    [Header("Stone Walk")]
    [SerializeField] public AudioClip[] SFX_footstep_stone_walk;
    [Range(0, 1)] public float SFX_footstep_stone_walk_volumeMin;
    [Range(0, 1)] public float SFX_footstep_stone_walk_volumeMax;
    [Range(0, 1)] public float SFX_footstep_stone_walk_pitchMin;
    [Range(0, 1)] public float SFX_footstep_stone_walk_pitchMax;
    public float SFX_footstep_stone_walk_rangeMin;
    public float SFX_footstep_stone_walk_rangeMax;

    [Header("Stone Run")]
    [SerializeField] public AudioClip[] SFX_footstep_stone_run;
    [Range(0, 1)] public float SFX_footstep_stone_run_volumeMin;
    [Range(0, 1)] public float SFX_footstep_stone_run_volumeMax;
    [Range(0, 1)] public float SFX_footstep_stone_run_pitchMin;
    [Range(0, 1)] public float SFX_footstep_stone_run_pitchMax;
    public float SFX_footstep_stone_run_rangeMin;
    public float SFX_footstep_stone_run_rangeMax;

    [Header("Mud Walk")]
    [SerializeField] public AudioClip[] SFX_footstep_mud_walk;
    [Range(0, 1)] public float SFX_footstep_mud_walk_volumeMin;
    [Range(0, 1)] public float SFX_footstep_mud_walk_volumeMax;
    [Range(0, 1)] public float SFX_footstep_mud_walk_pitchMin;
    [Range(0, 1)] public float SFX_footstep_mud_walk_pitchMax;
    public float SFX_footstep_mud_walk_rangeMin;
    public float SFX_footstep_mud_walk_rangeMax;

    [Header("Mud Run")]
    [SerializeField] public AudioClip[] SFX_footstep_mud_run;
    [Range(0, 1)] public float SFX_footstep_mud_run_volumeMin;
    [Range(0, 1)] public float SFX_footstep_mud_run_volumeMax;
    [Range(0, 1)] public float SFX_footstep_mud_run_pitchMin;
    [Range(0, 1)] public float SFX_footstep_mud_run_pitchMax;
    public float SFX_footstep_mud_run_rangeMin;
    public float SFX_footstep_mud_run_rangeMax;
}
