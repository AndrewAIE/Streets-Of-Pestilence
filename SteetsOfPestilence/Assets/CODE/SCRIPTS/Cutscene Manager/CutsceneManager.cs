using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
namespace Management
{
    public class CutsceneManager : MonoBehaviour
    {
        [HideInInspector] GameManager _gameManager;


        [Header("Playable Director")]
        [SerializeField] private PlayableDirector _director;
        [Space]

        [SerializeField] bool _pressAtoskipActive;
        [SerializeField] GameObject _pressAText;
        [Space]
        [SerializeField] bool _skipCutscene;
        [SerializeField] float _timeToSkipTo;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _gameManager = FindObjectOfType<GameManager>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_gameManager.m_Gamestate == GameState.Cutscene)
            {
                if (Input.GetButtonDown("Fire1") && !_skipCutscene && _pressAtoskipActive)
                {
                    SkipCutscene();
                }

                if (Input.GetButtonDown("Fire1") && !_pressAtoskipActive)
                {
                    _pressAText.SetActive(true);
                    _pressAtoskipActive = true;
                }
            }
        }

        public void SkipCutscene()
        {
            _director.time = _timeToSkipTo;
            _skipCutscene = true;
            TurnOff_PressAtoSkip();
        }

        public void TriggerCutscene()
        {
            _director.Play();
        }

        public void TurnOff_PressAtoSkip()
        {
            _pressAText.SetActive(false);
        }
    }
}