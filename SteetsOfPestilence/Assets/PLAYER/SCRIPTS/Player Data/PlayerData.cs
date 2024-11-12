using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = "New Player Data", menuName = "Player/Player Data", order = 0)]
    public class PlayerData : ScriptableObject
    {
        /************************************ MOVEMENT *******************************/
        #region Movement
        [Header("Input")]
        [Tooltip("Move speed of the character in m/s")]
        public float WalkingSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float RunningSpeed = 5.335f;

        [Space]

        [Tooltip("The threshold at which the player starts sprinting")]
        public float RunThres = 0.9f;

        [Space]

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("Max player falling speed")]
        public float TerminalVelocity = 53.0f;

        [Space]

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        #endregion

        /************************************ CAMERA *******************************/
        #region Camera
        [Header("Camera")]
        [SerializeField] public float FreeLook_RS_LookInputThres = 0.1f;
        [SerializeField] public float FreeLook_RS_TimerThres = 1f;

        #endregion

        /********************************* SFX ****************************/
        #region SFX
        [Space]
        [Header("SFX")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        #endregion

        /********************************* ANIMATION ********************************/



        #region Debug
        [Header("Debug")]
        public float DebugSpeed = 20f;

        #endregion
    }
}
