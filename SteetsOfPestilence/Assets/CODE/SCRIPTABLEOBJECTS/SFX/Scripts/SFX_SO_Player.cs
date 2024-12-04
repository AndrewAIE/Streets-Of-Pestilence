using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Player", menuName = "SFX SO/Player", order = 0)]
public class SFX_SO_Player : ScriptableObject
{
    [Header("Lamp Ambience")]
    [SerializeField] public AudioClip SFX_Player_LampAmbience_clip;
    [SerializeField] public float SFX_Player_LampAmbience_onVolume;

    [Header("Low Poise Warning")]
    [SerializeField] public AudioClip SFX_Player_LowPoiseAmbience_clip;
    [SerializeField] public float SFX_Player_LowPoiseWarning_onVolume;

    [Header("Armour Clink")]
    [SerializeField] public AudioClip[] SFX_Player_ArmourClink_clips;
    [SerializeField, Range(0,1)] public float SFX_Player_ArmourClink_chance;
    [SerializeField] public float SFX_Player_ArmourClink_volumeMax;
    [SerializeField] public float SFX_Player_ArmourClink_volumeMin;
    [SerializeField] public float SFX_Player_ArmourClink_pitchMax;
    [SerializeField] public float SFX_Player_ArmourClink_pitchMin;

    [Header("Body Drop")]
    [SerializeField] public AudioClip[] SFX_Player_BodyDrop_clips;
    [SerializeField] public float SFX_Player_BodyDrop_volumeMax;
    [SerializeField] public float SFX_Player_BodyDrop_volumeMin;
    [SerializeField] public float SFX_Player_BodyDrop_pitchMax;
    [SerializeField] public float SFX_Player_BodyDrop_pitchMin;

    [Header("Death Scream")]
    [SerializeField] public AudioClip[] SFX_Player_DeathScream_clips;
    [SerializeField] public float SFX_Player_DeathScream_volumeMax;
    [SerializeField] public float SFX_Player_DeathScream_volumeMin;
    [SerializeField] public float SFX_Player_DeathScream_pitchMax;
    [SerializeField] public float SFX_Player_DeathScream_pitchMin;

    [Header("Player Grunts")]
    [SerializeField] public AudioClip[] SFX_Player_Grunts_clips;
    [SerializeField] public float SFX_Player_Grunts_volumeMax;
    [SerializeField] public float SFX_Player_Grunts_volumeMin;
    [SerializeField] public float SFX_Player_Grunts_pitchMax;
    [SerializeField] public float SFX_Player_Grunts_pitchMin;

    [Header("Player Long Grunt")]
    [SerializeField] public AudioClip[] SFX_Player_LongGrunt_clips;
    [SerializeField] public float SFX_Player_LongGrunt_volumeMax;
    [SerializeField] public float SFX_Player_LongGrunt_volumeMin;
    [SerializeField] public float SFX_Player_LongGrunt_pitchMax;
    [SerializeField] public float SFX_Player_LongGrunt_pitchMin;


}
