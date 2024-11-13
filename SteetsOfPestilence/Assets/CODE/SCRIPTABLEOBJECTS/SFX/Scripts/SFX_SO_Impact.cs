using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Impact", menuName = "SFX SO/Impact", order = 0)]
public class SFX_SO_Impact : ScriptableObject
{
    [Header("Impacts")]
    [SerializeField] AudioClip SFX_Impact_QTESuccess;
    [SerializeField] AudioClip SFX_Impact_QTEFailure;
    [SerializeField] AudioClip SFX_Impact_EncoutnerSuccess;
    [SerializeField] AudioClip SFX_Impact_EncounterFailure;
}
