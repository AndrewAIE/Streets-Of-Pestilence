using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    public class SFXController_Player : MonoBehaviour
    {
        /************************* VARIABLES ****************************/
        #region Variables
        [HideInInspector] PlayerManager _manager;
        [HideInInspector] SFXTester _tester;
        [Space]
        [SerializeField] AmbienceMode ambienceMode;

        [Space]
        [Header("Ambience")]
        [SerializeField] SFX_SO_Ambience _ambienceData;
        [SerializeField] GameObject ambientPrefab;
        [SerializeField] AudioClip ambient_lastPlayed;
        [Space]
        [SerializeField] BoxCollider minRange;
        [SerializeField] BoxCollider maxRange;
        [Space]
        [SerializeField] float chirpTimer;
        [SerializeField] float chirpTimerMin;
        [SerializeField] float chirpTimerMax;

        [Space]
        [Header("Footsteps")]
        [SerializeField] SFX_SO_Footstep _footstepData;
        [SerializeField] GameObject footstepPrefab;
        [SerializeField] AudioClip footstep_lastPlayedClip;

        [Header("Weapon")]
        [SerializeField] SFX_SO_Weapon _weaponData;
        [SerializeField] Transform _clashPos;
        [Space]
        [SerializeField] GameObject metalClashPrefab;
        [SerializeField] AudioClip metalClash_lastPlayedClip;
        [Space]
        [SerializeField] GameObject swingPrefab;
        [SerializeField] AudioClip swing_lastPlayedClip;


        #endregion

        /************************ ENUM *********************************/

        public enum AmbienceMode
        {
            roaming,
            combat,
            none
        }

        /************************ METHODS *******************************/
        #region Methods

        /*** Awake ***/
        #region Awake
        private void Awake()
        {
            _tester = GetComponent<SFXTester>();

            //_manager = GetComponentInParent<PlayerManager>();

            // Concatenate all arrays into one
            _ambienceData.Setup();
        }

        #endregion

        /*** Update ***/
        #region Update 
        private void Update()
        {
            switch (ambienceMode)
            {
                case AmbienceMode.roaming:
                    Ambience_Roaming();
                    break;

                case AmbienceMode.combat:
                    break;
            }
        }

        #endregion

        /*** GetUniqueClip ***/
        #region Get Unique Clip
        // Method to find a new, unique clip from an array of clips
        public AudioClip GetUniqueClip(AudioClip[] clips, AudioClip lastPlayedClip)
        {
            // If there's only one clip, return it (no other options available)
            if (clips.Length <= 1) return clips[0];

            AudioClip newClip;

            // Loop until a unique clip is found
            do
            {
                newClip = clips[Random.Range(0, clips.Length)];
            } while (newClip == lastPlayedClip);

            return newClip;
        }

        #endregion

        /*** Footsteps ***/
        #region Footsteps
        public void Play_Footstep_Walk_Stone()
        {
            footstep_lastPlayedClip = GetUniqueClip(_footstepData.SFX_footstep_stone_walk, footstep_lastPlayedClip);
            CreateFootstep_StoneWalk(footstep_lastPlayedClip);
        }

        public void Play_Footstep_Run_Stone()
        {
            footstep_lastPlayedClip = GetUniqueClip(_footstepData.SFX_footstep_stone_run, footstep_lastPlayedClip);
            CreateFootstep_StoneRun(footstep_lastPlayedClip);
        }

        public void Play_Footstep_Walk_Mud()
        {
            footstep_lastPlayedClip = GetUniqueClip(_footstepData.SFX_footstep_mud_walk, footstep_lastPlayedClip);
            CreateFootstep_MudWalk(footstep_lastPlayedClip);
        }

        public void Play_Footstep_Run_Mud()
        {
            footstep_lastPlayedClip = GetUniqueClip(_footstepData.SFX_footstep_mud_run, footstep_lastPlayedClip);
            CreateFootstep_MudRun(footstep_lastPlayedClip);
        }

        private void CreateFootstep_StoneWalk(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(footstepPrefab, Vector3.zero, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_footstepData.SFX_footstep_stone_walk_volumeMin, _footstepData.SFX_footstep_stone_walk_volumeMax);
            audioSource.pitch = Random.Range(_footstepData.SFX_footstep_stone_walk_pitchMin, _footstepData.SFX_footstep_stone_walk_pitchMax);
            audioSource.maxDistance = Random.Range(_footstepData.SFX_footstep_stone_walk_rangeMin, _footstepData.SFX_footstep_stone_walk_rangeMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        private void CreateFootstep_StoneRun(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(footstepPrefab, Vector3.zero, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_footstepData.SFX_footstep_stone_run_volumeMin, _footstepData.SFX_footstep_stone_run_volumeMax);
            audioSource.pitch = Random.Range(_footstepData.SFX_footstep_stone_run_pitchMin, _footstepData.SFX_footstep_stone_run_pitchMax);
            audioSource.maxDistance = Random.Range(_footstepData.SFX_footstep_stone_run_rangeMin, _footstepData.SFX_footstep_stone_run_rangeMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        private void CreateFootstep_MudWalk(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(footstepPrefab, Vector3.zero, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_footstepData.SFX_footstep_mud_walk_volumeMin, _footstepData.SFX_footstep_mud_walk_volumeMax);
            audioSource.pitch = Random.Range(_footstepData.SFX_footstep_mud_walk_pitchMin, _footstepData.SFX_footstep_mud_walk_pitchMax);
            audioSource.maxDistance = Random.Range(_footstepData.SFX_footstep_mud_walk_rangeMin, _footstepData.SFX_footstep_mud_walk_rangeMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        private void CreateFootstep_MudRun(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(footstepPrefab, Vector3.zero, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_footstepData.SFX_footstep_mud_run_volumeMin, _footstepData.SFX_footstep_mud_run_volumeMax);
            audioSource.pitch = Random.Range(_footstepData.SFX_footstep_mud_run_pitchMin, _footstepData.SFX_footstep_mud_run_pitchMax);
            audioSource.maxDistance = Random.Range(_footstepData.SFX_footstep_mud_run_rangeMin, _footstepData.SFX_footstep_mud_run_rangeMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }




        #endregion

        /*** Ambience ***/
        #region Ambience

        /*** Roaming ***/
        #region Roaming

        private void Ambience_Roaming()
        {
            if(chirpTimer <= 0)
            {
                Play_Ambience_Chirp();
                chirpTimer = Random.Range(chirpTimerMin, chirpTimerMax);
            }
            else
            {
                chirpTimer -= Time.deltaTime;
            }
        }

        public void Play_Ambience_Chirp()
        {
            ambient_lastPlayed = GetUniqueClip(_ambienceData.sfx_ambience_allChirps, ambient_lastPlayed);
            CreateAmbienceChirp(_ambienceData.GetRandomAmbienceChirp(), GetPointInRange());
        }

        public void CreateAmbienceChirp(AudioClip clip, Vector3 position)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(ambientPrefab, position, Quaternion.identity);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();
            
            audioSource.clip = clip;
            audioSource.volume = Random.Range(0.5f, 1f);
            audioSource.pitch = Random.Range(0.9f, 1.1f);

            audioSource.maxDistance = Random.Range(20f, 30f);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void SetAmbienceMode(int inputMode)
        {
            ambienceMode = (AmbienceMode)inputMode;
        }

        /*** Find a point ***/
        #region Find a point

        public Vector3 GetPointInRange()
        {
            Vector3 randomPoint;

            do
            {
                // Generate a random point within maxRange
                randomPoint = GetRandomPointInBox(maxRange);
            }
            while (IsPointInBox(randomPoint, minRange));  // Keep generating if the point is inside minRange

            return randomPoint;
        }

        private Vector3 GetRandomPointInBox(BoxCollider box)
        {
            // Get the bounds of the box
            Bounds bounds = box.bounds;

            // Generate a random point within the bounds
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(randomX, randomY, randomZ);
        }

        private bool IsPointInBox(Vector3 point, BoxCollider box)
        {
            // Check if the point is within the bounds of the box
            return box.bounds.Contains(point);
        }

        #endregion

        #endregion

        #endregion

        /*** Weapon ***/
        #region Weapon

        private void CreateMetalClash(AudioClip clip, Vector3 position)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(metalClashPrefab, position, Quaternion.identity);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_weaponData.SFX_Weapon_MetalClash_volumeMin, _weaponData.SFX_Weapon_MetalClash_volumeMax);
            audioSource.pitch = Random.Range(_weaponData.SFX_Weapon_MetalClash_pitchMin, _weaponData.SFX_Weapon_MetalClash_pitchMax);

            audioSource.maxDistance = Random.Range(_weaponData.SFX_Weapon_MetalClash_rangeMin, _weaponData.SFX_Weapon_MetalClash_rangeMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_MetalClash()
        {
            metalClash_lastPlayedClip = GetUniqueClip(_weaponData.SFX_Weapon_MetalClash_clips, metalClash_lastPlayedClip);
            CreateMetalClash(metalClash_lastPlayedClip, _clashPos.position);
        }

        private void CreateSwing(AudioClip clip, Vector3 position)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(swingPrefab, position, Quaternion.identity);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_weaponData.SFX_Weapon_Swing_volumeMin, _weaponData.SFX_Weapon_Swing_volumeMax);
            audioSource.pitch = Random.Range(_weaponData.SFX_Weapon_Swing_pitchMin, _weaponData.SFX_Weapon_Swing_pitchMax);

            audioSource.maxDistance = Random.Range(_weaponData.SFX_Weapon_Swing_rangeMin, _weaponData.SFX_Weapon_Swing_rangeMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_Swing()
        {
            swing_lastPlayedClip = GetUniqueClip(_weaponData.SFX_Weapon_LightSwing, swing_lastPlayedClip);
            CreateSwing(swing_lastPlayedClip, _clashPos.position);
        }



        #endregion

        #endregion


    }
}


/*
#region SFX
private void OnFootstep(AnimationEvent animationEvent)
{
    if (animationEvent.animatorClipInfo.weight > 0.5f)
    {
        if (FootstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_manager.centerPoint), FootstepAudioVolume);
        }
    }
}

private void OnLand(AnimationEvent animationEvent)
{
    if (animationEvent.animatorClipInfo.weight > 0.5f)
    {
        AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_manager.centerPoint), _manager._data.FootstepAudioVolume);
    }
}

#endregion*/
        