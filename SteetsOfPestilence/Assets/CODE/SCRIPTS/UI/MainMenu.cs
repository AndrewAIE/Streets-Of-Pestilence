using TMPro;
using UnityEngine;
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


        private AudioSource m_audioSource;
        public AudioClip m_selectChangeAudio;

        [Space]
        [SerializeField] RectTransform _pointerTransform;
        [SerializeField] Vector3 _pointerOffset;
        [SerializeField] float _pointerSpeed;
        private Vector3 m_prevPos;

        private void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
            if (_currentButton == null)
            {
                _currentButton = GetComponentInChildren<Button>();
                EventSystem.current.SetSelectedGameObject(_currentButton.gameObject);
            }
            
            _pointerOffset.x = _pointerTransform.position.x;

            _pointerTransform.position = _currentButton.gameObject.transform.position + _pointerOffset;
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
                    m_audioSource.Play();
                    m_prevPos = newPos;
                }
            }
        }

        public void StartGame()
        {
            Invoke("LoadGame", 2.25f);
            
        }
        public void LoadGame()
        {
            Debug.Log("Starting Game...");
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