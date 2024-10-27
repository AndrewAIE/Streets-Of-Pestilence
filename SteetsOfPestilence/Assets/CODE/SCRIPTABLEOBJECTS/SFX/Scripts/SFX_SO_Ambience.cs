using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SFX_SO_Ambience", menuName = "SFX SO/Ambience", order = 0)]
public class SFX_SO_Ambience : ScriptableObject
{
    [Header("Ambient 'Chirps'")]
    [Header("Animal Sounds")]
    [SerializeField] AudioClip[] sfx_ambience_animal_birdFlappingWings;
    [SerializeField] AudioClip[] sfx_ambience_animal_catMeow;
    [SerializeField] AudioClip[] sfx_ambience_animal_coyoteBarks;
    [SerializeField] AudioClip[] sfx_ambience_animal_coyoteHowls;
    [SerializeField] AudioClip[] sfx_ambience_animal_crowCalls;
    [SerializeField] AudioClip[] sfx_ambience_animal_dogAttack;
    [SerializeField] AudioClip[] sfx_ambience_animal_dogBark;
    [SerializeField] AudioClip[] sfx_ambience_animal_dogCries;
    [SerializeField] AudioClip[] sfx_ambience_animal_horseSqueals;
    [SerializeField] AudioClip[] sfx_ambience_animal_mouseSqueaks;
    [SerializeField] AudioClip[] sfx_ambience_animal_owlBarks;
    [SerializeField] AudioClip[] sfx_ambience_animal_owlGrowls;
    [SerializeField] AudioClip[] sfx_ambience_animal_owlShrieks;
    [SerializeField] AudioClip[] sfx_ambience_animal_wolfBarks;
    [SerializeField] AudioClip[] sfx_ambience_animal_wolfHowls;

    [Header("Door Sounds")]
    [SerializeField] AudioClip[] sfx_ambience_doorSounds;

    [Header("Screams")]
    [SerializeField] AudioClip[] sfx_ambience_screams;

    [Space]

    [Header("Ambient 'Loops'")]
    [SerializeField] AudioClip[] sfx_ambience_natureLoops;
    [Space]
    [SerializeField] AudioClip[] sfx_ambience_soundscapes;

    [SerializeField] public AudioClip[] sfx_ambience_allChirps;

    public void Setup()
    {
        // Concatenate all arrays into one
        sfx_ambience_allChirps = sfx_ambience_animal_birdFlappingWings
            .Concat(sfx_ambience_animal_catMeow)
            .Concat(sfx_ambience_animal_coyoteBarks)
            .Concat(sfx_ambience_animal_coyoteHowls)
            .Concat(sfx_ambience_animal_crowCalls)
            .Concat(sfx_ambience_animal_dogAttack)
            .Concat(sfx_ambience_animal_dogBark)
            .Concat(sfx_ambience_animal_dogCries)
            .Concat(sfx_ambience_animal_horseSqueals)
            .Concat(sfx_ambience_animal_mouseSqueaks)
            .Concat(sfx_ambience_animal_owlBarks)
            .Concat(sfx_ambience_animal_owlGrowls)
            .Concat(sfx_ambience_animal_owlShrieks)
            .Concat(sfx_ambience_animal_wolfBarks)
            .Concat(sfx_ambience_animal_wolfHowls)
            .Concat(sfx_ambience_doorSounds)
            .Concat(sfx_ambience_screams)
            .ToArray();
    }

    public AudioClip GetRandomAmbienceChirp()
    {
        return sfx_ambience_allChirps[UnityEngine.Random.Range(0, sfx_ambience_allChirps.Length)];
    }


}
