using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Impact", menuName = "SFX SO/Impact", order = 0)]
public class SFX_SO_Impact : ScriptableObject
{
    [Header("Impacts")]
    [SerializeField] public AudioClip[] SFX_Impact_QTESuccess;
    [SerializeField] public float SFX_Impact_QTESuccess_volumeMin;
    [SerializeField] public float SFX_Impact_QTESuccess_volumeMax;
    [SerializeField] public float SFX_Impact_QTESuccess_pitchMin;
    [SerializeField] public float SFX_Impact_QTESuccess_pitchMax;


    [Space]
    [SerializeField] public AudioClip[] SFX_Impact_QTEFailure;
    [SerializeField] public float SFX_Impact_QTEFailure_volumeMin;
    [SerializeField] public float SFX_Impact_QTEFailure_volumeMax;
    [SerializeField] public float SFX_Impact_QTEFailure_pitchMin;
    [SerializeField] public float SFX_Impact_QTEFailure_pitchMax;
    /*[SerializeField] AudioClip SFX_Impact_EncoutnerSuccess_MinorEnemy;
    [SerializeField] AudioClip SFX_Impact_EncoutnerSuccess_MajorEnemy;
    [SerializeField] AudioClip SFX_Impact_EncounterFailure;*/
}
