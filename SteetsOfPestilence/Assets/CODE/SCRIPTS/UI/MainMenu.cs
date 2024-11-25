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


        [Space]
        [SerializeField] RectTransform _pointerTransform;
        [SerializeField] Vector3 _pointerOffset;
        [SerializeField] float _pointerSpeed;

        private void Awake()
        {
            _buttons = _buttonParent.GetComponentsInChildren<Button>();

            _pointerTransform.position = _buttons[_buttonIndex].gameObject.transform.position + _pointerOffset;
        }

        private void Update()
        {
            if (_currentButton == null)
            {
                _currentButton = GetComponentInChildren<Button>();
                EventSystem.current.SetSelectedGameObject(_currentButton.gameObject);
            }

            //set pointer position
            _pointerTransform.position = Vector3.Lerp(_pointerTransform.position, _buttons[_buttonIndex].gameObject.transform.position + _pointerOffset, _pointerSpeed * Time.deltaTime);
        }

        public void StartGame()
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