using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    public class SFXController : MonoBehaviour
    {
        [SerializeField] PlayerManager _manager;

        /********************************* SFX ****************************/
        #region SFX
        [Space]
        [Header("SFX")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        #endregion

        private void Awake()
        {
            _manager = GetComponentInParent<PlayerManager>();
        }

        /************ SFX **************/
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

        #endregion
    }
}