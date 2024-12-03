using UnityEngine;
using Pixelplacement;
using UnityEngine.Audio;
using System.Collections;

namespace PlayerController
{
    public class SFXController_Player : MonoBehaviour
    {
        /************************* VARIABLES ****************************/
        #region Variables
        [HideInInspector] PlayerManager _manager;
        [HideInInspector] SFXTester _tester;
        [Space]
        [SerializeField] AudioMixer _mixerGameScene;
        public string exposedParameter = "Volume"; // Make sure this matches your exposed parameter name
        public float fadeDuration = 2.0f; // Time to fade in (seconds)
        public float targetVolume = 0.0f; // Target volume in decibels (0 is default max in Unity)

        [Space]
        [SerializeField] AmbienceMode ambienceMode;

        [Space]
        [Header("Ambience")]
        [SerializeField] SFX_SO_Ambience _ambienceData;
        [SerializeField] GameObject ambientChirp_Prefab;
        [HideInInspector] AudioClip ambientChirp_lastPlayed;
        [Space]
        [SerializeField] AudioSource ambience_wind;
        [SerializeField] AudioSource ambience_fire;
        [Space]
        [SerializeField] BoxCollider minRange;
        [SerializeField] BoxCollider maxRange;
        [Space]
        [HideInInspector] float chirpTimer;
        [SerializeField] float chirpTimerMin;
        [SerializeField] float chirpTimerMax;

        [Space]
        [Header("Footsteps")]
        [SerializeField] SFX_SO_Footstep _footstepData;
        [Space]
        [SerializeField] GameObject footstep_Prefab;
        [HideInInspector] AudioClip footstep_lastPlayedClip;

        [Space]
        [Header("Weapon")]
        [SerializeField] SFX_SO_Weapon _weaponData;
        [SerializeField] Transform _clashPos;
        [Space]
        [SerializeField] GameObject metalClash_Prefab;
        [SerializeField] GameObject swing_Prefab;
        
        [HideInInspector] AudioClip metalClash_lastPlayedClip;
        [HideInInspector] AudioClip swingLight_lastPlayedClip;
        [HideInInspector] AudioClip swingHeavy_lastPlayedClip;

        [Space]
        [Header("Player")]
        [SerializeField] SFX_SO_Player _playerData;
        [Space]
        [SerializeField] AudioSource _lampAmbience;
        [SerializeField] AudioSource _lowPoiseAmbience;
        [Space]
        [SerializeField] GameObject armourClink_Prefab;
        [SerializeField] GameObject bodyDrop_Prefab;
        [SerializeField] GameObject deathScream_Prefab;
        [Space]
        [HideInInspector] AudioClip armourClink_LastPlayedClip;
        [HideInInspector] AudioClip bodyDrop_LastPlayedClip;
        [HideInInspector] AudioClip deathScream_LastPlayedClip;

        [Space]
        [SerializeField] SFX_SO_Impact _impactData;
        [Space]
        [SerializeField] GameObject impact_Prefab;

        [HideInInspector] AudioClip _impact_QTESuccess_LastPlayedClip;
        [HideInInspector] AudioClip _impact_QTEFailure_LastPlayedClip;



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

            //assign loops to player ambience
            _lampAmbience.clip = _playerData.SFX_Player_LampAmbience_clip;
            _lowPoiseAmbience.clip = _playerData.SFX_Player_LowPoiseAmbience_clip;

            
        }

        private void Start()
        {
            StartCoroutine(FadeInAudio());
        }

        private IEnumerator FadeInAudio()
        {
            float currentTime = 0.0f;
            float startVolume = -80.0f; // Starting volume (usually silence)

            // Set the starting volume
            _mixerGameScene.SetFloat(exposedParameter, startVolume);

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
                _mixerGameScene.SetFloat(exposedParameter, newVolume);
                yield return null;
            }

            // Ensure final value is set
            _mixerGameScene.SetFloat(exposedParameter, targetVolume);
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
            GameObject audioOneshot = Instantiate(footstep_Prefab, transform.position, Quaternion.identity, transform);
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
            GameObject audioOneshot = Instantiate(footstep_Prefab, transform.position, Quaternion.identity, transform);
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
            GameObject audioOneshot = Instantiate(footstep_Prefab, transform.position, Quaternion.identity, transform);
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
            GameObject audioOneshot = Instantiate(footstep_Prefab, transform.position, Quaternion.identity, transform);
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
            ambientChirp_lastPlayed = GetUniqueClip(_ambienceData.sfx_ambience_allChirps, ambientChirp_lastPlayed);
            CreateAmbienceChirp(_ambienceData.GetRandomAmbienceChirp(), GetPointInRange());
        }

        public void CreateAmbienceChirp(AudioClip clip, Vector3 position)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(ambientChirp_Prefab, position, Quaternion.identity);
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

            if(ambienceMode == AmbienceMode.none)
            {
                ambience_fire.Stop();
                ambience_wind.Stop();
            }
            else
            {
                ambience_fire.Play();
                ambience_wind.Play();
            }
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

        #region Metal Clash
        private void CreateMetalClash(AudioClip clip, Vector3 position)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(metalClash_Prefab, position, Quaternion.identity);
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

        #endregion

        #region Swing
        private void CreateSwing(AudioClip clip, Vector3 position)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(swing_Prefab, position, Quaternion.identity);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_weaponData.SFX_Weapon_Swing_volumeMin, _weaponData.SFX_Weapon_Swing_volumeMax);
            audioSource.pitch = Random.Range(_weaponData.SFX_Weapon_Swing_pitchMin, _weaponData.SFX_Weapon_Swing_pitchMax);

            audioSource.maxDistance = Random.Range(_weaponData.SFX_Weapon_Swing_rangeMin, _weaponData.SFX_Weapon_Swing_rangeMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_LightSwing()
        {
            swingLight_lastPlayedClip = GetUniqueClip(_weaponData.SFX_Weapon_LightSwing, swingLight_lastPlayedClip);
            CreateSwing(swingLight_lastPlayedClip, _clashPos.position);
        }

        public void Play_HeavySwing()
        {
            swingHeavy_lastPlayedClip = GetUniqueClip(_weaponData.SFX_Weapon_HeavySwing, swingHeavy_lastPlayedClip);
            CreateSwing(swingHeavy_lastPlayedClip, _clashPos.position);
        }

        #endregion

        #endregion

        /*** Player ***/
        #region Player

        #region Lamp Ambience

        public void Play_LampAmbience()
        {
            _lampAmbience.Play();
            Tween.Value(0, _playerData.SFX_Player_LampAmbience_onVolume, LampAmbience_TweenCallback, 1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
        }

        public void Stop_LampAmbience()
        {
            _lampAmbience.Stop();
        }

        private void LampAmbience_TweenCallback(float t)
        {
            _lampAmbience.volume = t;
        }

        #endregion

        #region Low Poise Ambience
        public void Play_LowPoiseAmbience()
        {
            _lowPoiseAmbience.Play();
            Tween.Value(0, _playerData.SFX_Player_LowPoiseWarning_onVolume, LowPoiseAmbience_TweenCallback, 1f, 0f, Tween.EaseInOut);
        }

        public void Stop_LowPoiseAmbience()
        {
            Tween.Value(_playerData.SFX_Player_LowPoiseWarning_onVolume, 0f, LowPoiseAmbience_TweenCallback, 1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, LowPoiseAmbience_TweenStop);
        }

        private void LowPoiseAmbience_TweenCallback(float t)
        {
            _lowPoiseAmbience.volume = t;
        }

        private void LowPoiseAmbience_TweenStop()
        {
            _lowPoiseAmbience.Stop();
        }


        #endregion

        #region Armour Clink

        private void Create_ArmourClink(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(armourClink_Prefab, transform.position, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_playerData.SFX_Player_ArmourClink_volumeMin, _playerData.SFX_Player_ArmourClink_volumeMax);
            audioSource.pitch = Random.Range(_playerData.SFX_Player_ArmourClink_pitchMin, _playerData.SFX_Player_ArmourClink_pitchMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_ArmourClink()
        {
            float randomFloat = Random.Range(0f, 1f);
            if (randomFloat < _playerData.SFX_Player_ArmourClink_chance)
                return;

            armourClink_LastPlayedClip = GetUniqueClip(_playerData.SFX_Player_ArmourClink_clips, armourClink_LastPlayedClip);
            Create_ArmourClink(armourClink_LastPlayedClip);
        }

        #endregion

        #region Body Drop

        private void Create_BodyDrop(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(bodyDrop_Prefab, transform.position, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_playerData.SFX_Player_BodyDrop_volumeMin, _playerData.SFX_Player_BodyDrop_volumeMax);
            audioSource.pitch = Random.Range(_playerData.SFX_Player_BodyDrop_pitchMin, _playerData.SFX_Player_BodyDrop_pitchMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_BodyDrop()
        {
            bodyDrop_LastPlayedClip = GetUniqueClip(_playerData.SFX_Player_BodyDrop_clips, bodyDrop_LastPlayedClip);
            Create_BodyDrop(bodyDrop_LastPlayedClip);
        }

        #endregion

        #region Death Scream

        private void Create_DeathScream(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(deathScream_Prefab, transform.position, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_playerData.SFX_Player_DeathScream_volumeMin, _playerData.SFX_Player_DeathScream_volumeMax);
            audioSource.pitch = Random.Range(_playerData.SFX_Player_DeathScream_pitchMin, _playerData.SFX_Player_DeathScream_pitchMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_DeathScream()
        {
            deathScream_LastPlayedClip = GetUniqueClip(_playerData.SFX_Player_DeathScream_clips, deathScream_LastPlayedClip);
            Create_DeathScream(deathScream_LastPlayedClip);
        }

        #endregion

        #endregion

        /*** Impacts ***/
        #region Impacts

        #region Successful Player Impact
        private void Create_Impact_QTESuccess(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(impact_Prefab, transform.position, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_impactData.SFX_Impact_QTESuccess_volumeMin, _impactData.SFX_Impact_QTESuccess_volumeMax);
            audioSource.pitch = Random.Range(_impactData.SFX_Impact_QTESuccess_pitchMin, _impactData.SFX_Impact_QTESuccess_pitchMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_Impact_QTESuccess()
        {
            _impact_QTESuccess_LastPlayedClip = GetUniqueClip(_impactData.SFX_Impact_QTESuccess, _impact_QTESuccess_LastPlayedClip);
            Create_Impact_QTESuccess(_impact_QTESuccess_LastPlayedClip);
        }


        #endregion

        #region Failure Player Impact

        private void Create_Impact_QTEFailure(AudioClip clip)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(impact_Prefab, transform.position, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(_impactData.SFX_Impact_QTEFailure_volumeMin, _impactData.SFX_Impact_QTEFailure_volumeMax);
            audioSource.pitch = Random.Range(_impactData.SFX_Impact_QTEFailure_pitchMin, _impactData.SFX_Impact_QTEFailure_pitchMax);

            audioSource.Play();

            Destroy(audioOneshot, length);
        }

        public void Play_Impact_QTEFailure()
        {
            _impact_QTEFailure_LastPlayedClip = GetUniqueClip(_impactData.SFX_Impact_QTEFailure, _impact_QTEFailure_LastPlayedClip);
            Create_Impact_QTEFailure(_impact_QTEFailure_LastPlayedClip);
        }

        #endregion

        #endregion

        #endregion
    }
}

