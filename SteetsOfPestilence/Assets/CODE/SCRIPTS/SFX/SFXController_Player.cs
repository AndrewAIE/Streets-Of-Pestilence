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

        /*** Footsteps ***/
        #region Footsteps
        public void Play_Footstep_Walk_Stone()
        {
            CreateFootstep(_footstepData.SFX_footstep_stone_walk[Random.Range(0, _footstepData.SFX_footstep_stone_walk.Length)], true);
        }

        public void Play_Footstep_Run_Stone()
        {
            CreateFootstep(_footstepData.SFX_footstep_stone_run[Random.Range(0, _footstepData.SFX_footstep_stone_run.Length)], true);
        }

        public void Play_Footstep_Walk_Mud()
        {
            CreateFootstep(_footstepData.SFX_footstep_mud_walk[Random.Range(0, _footstepData.SFX_footstep_mud_walk.Length)], false);
        }

        public void Play_Footstep_Run_Mud()
        {
            CreateFootstep(_footstepData.SFX_footstep_mud_run[Random.Range(0, _footstepData.SFX_footstep_mud_run.Length)], false);
        }

        private void CreateFootstep(AudioClip clip, bool isStone)
        {
            float length = clip.length;
            GameObject audioOneshot = Instantiate(footstepPrefab, Vector3.zero, Quaternion.identity, transform);
            AudioSource audioSource = audioOneshot.GetComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = Random.Range(0.8f, 1f);
            audioSource.pitch = Random.Range(0.9f, 1.1f);

            if(!isStone)
            {
                audioSource.volume *= 0.8f;
            }

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
        