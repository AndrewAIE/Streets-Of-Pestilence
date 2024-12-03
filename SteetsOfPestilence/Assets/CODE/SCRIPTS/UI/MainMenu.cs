using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Management
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] bool _interactable;
        [Space]
        [SerializeField] GameObject _buttonParent;
        [SerializeField] Button _currentButton;
        [SerializeField] Button[] _buttons;
        [SerializeField] int _buttonIndex;
        [Space]
        [SerializeField] Color _defaultUnderlay;
        [SerializeField] Color _redUnderlay;


        public AudioSource m_audioSource;
        public AudioClip m_selectionChangeAudioClip;

        [Space]
        [SerializeField] Vector2 _pointerStartPosition;
        [SerializeField] RectTransform _pointerTransform;
        [SerializeField] Vector3 _pointerOffset;
        [SerializeField] float _pointerSpeed;
        private Vector3 m_prevPos;

        [SerializeField] AudioMixer m_mainMenuMixer;
        public string exposedParameter = "Main Menu Volume"; // Make sure this matches your exposed parameter name
        public float fadeDuration = 2.0f; // Time to fade in (seconds)
        public float targetVolume_ON = 0.0f; // Target volume in decibels (0 is default max in Unity)
        public float targetVolume_OFF = -80.0f; // Target volume in decibels (0 is default max in Unity)
        public float currentVolume;

        private void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
            if (_currentButton == null)
            {
                _currentButton = GetComponentInChildren<Button>();
                EventSystem.current.SetSelectedGameObject(_currentButton.gameObject);
            }
            
            _pointerOffset.x = _pointerTransform.position.x;

            _pointerTransform.localPosition = _pointerStartPosition;
            m_prevPos = _currentButton.gameObject.transform.position;
        }

        private void Update()
        {
            if(!_interactable) { return; }
            if (_currentButton == null)
            {
                _currentButton = GetComponentInChildren<Button>();
                EventSystem.current.SetSelectedGameObject(_currentButton.gameObject);
            }
            else
            {
                _currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            }

            //set pointer position
            if (_currentButton != null)
            {
                Vector3 newPos = _currentButton.gameObject.transform.position;
               
                newPos.x = _pointerOffset.x;
                _pointerTransform.position = Vector3.Lerp(_pointerTransform.position, newPos, _pointerSpeed * Time.deltaTime);

                if (m_prevPos != newPos)
                {
                    m_audioSource.PlayOneShot(m_selectionChangeAudioClip);
                    m_prevPos = newPos;
                }
            }
        }

        private IEnumerator FadeOutAudio()
        {
            float currentTime = 0f;

            // Set the starting volume
            m_mainMenuMixer.SetFloat(exposedParameter, targetVolume_ON);

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(targetVolume_ON, targetVolume_OFF, currentTime / fadeDuration);
                m_mainMenuMixer.SetFloat(exposedParameter, newVolume);
                currentVolume = newVolume;
                yield return null;
            }

            // Ensure final value is set
            currentVolume = targetVolume_OFF;
            m_mainMenuMixer.SetFloat(exposedParameter, targetVolume_OFF);
        }

        public void StartGame()
        {
            TriggerTransitionAnimation();
        }

        public void LoadGame()
        {
            //Debug.Log("Starting Game...");
            SceneChanger.ChangeScene(1);
        }

        public void QuitGame()
        {
            SceneChanger.QuitGame();
        }

        private void SetButtonTextColor(Color _newColor, Button _button)
        {
            _button.GetComponentInChildren<TextMeshProUGUI>().color = _newColor;
        }

        public void EnableInteraction()
        {
            _interactable = true;
        }

        public void TriggerTransitionAnimation()
        {
            GetComponent<Animator>().SetTrigger("Main Menu Transition");
        }
        
    }
}