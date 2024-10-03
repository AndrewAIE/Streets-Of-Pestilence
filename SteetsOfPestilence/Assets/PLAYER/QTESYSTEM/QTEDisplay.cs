using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace QTESystem
{
    public class QTEDisplay : MonoBehaviour
    {
        //******************** Variables *******************//
        #region Variables
        [Header("Button Icons")]
        [SerializeField]
        private GameObject m_iconParent;
        [Space]
        [SerializeField]
        private Image m_northButtonIcon;
        [SerializeField]
        private Image m_eastButtonIcon;
        [SerializeField]
        private Image m_southButtonIcon;
        [SerializeField]
        private Image m_westButtonIcon;
        [SerializeField]
        private Image m_lShoulderButtonIcon;
        [SerializeField]
        private Image m_lTriggerButtonIcon;
        [SerializeField]
        private Image m_rShoulderButtonIcon;
        [SerializeField]
        private Image m_rTriggerButtonIcon;
        [SerializeField]
        private Image m_northDirectionalButtonIcon;
        [SerializeField]
        private Image m_eastDirectionalButtonIcon;
        [SerializeField]
        private Image m_southDirectionalButtonIcon;
        [SerializeField]
        private Image m_westDirectionalButtonIcon;

        [Space]
        [Header("QTE System")]
        private List<QTEInput> m_iconsToActivate = new List<QTEInput>();

        [Space]
        [Header("Poise Bar")]
        [SerializeField]
        private GameObject m_barObject;
        [SerializeField]
        private PoiseBarController m_poiseBar;

        [SerializeField]
        private List<GameObject> m_iconPanels;        
        
        [Header("Visual Cue Prefabs")]
        [SerializeField]
        GameObject m_faceButtonCue;
        [SerializeField]
        GameObject m_lShoulderButtonCue, m_rShoulderButtonCue, m_lTriggerCue, m_rTriggerCue,
            m_northDirectionalCue, m_eastDirectionalCue, m_southDirectionalCue, m_westDirectionalCue;

        [SerializeField]
        int m_cueStartSize;
        [HideInInspector]
        public List<GameObject> ActiveVisualCues, FinishingCues;
        

        [HideInInspector]
        public PoiseBarController poiseBarController;

        private QTEUIAnimation m_iconAnimation;
        private QTEAudio m_audio;        
        #endregion       

        private void Awake()
        {
            m_iconAnimation = GetComponentInChildren<QTEUIAnimation>();
            m_audio = GetComponentInChildren<QTEAudio>();            
        }
        //*** Panel ***//
        #region Panel

        
        public void ActivatePanel(int _indicator)
        {
            m_iconPanels[_indicator].SetActive(true);
        }

        
        public void DeactivatePanels()
        {
            foreach (GameObject panel in m_iconPanels)
            {
                panel.SetActive(false);
            }
        }

        #endregion

        //*** Icon Feedback ***//
        #region Icon Feedback

        //Comment
        public void SetIconColor(List<QTEInput> _iconsToSet, Color _color)
        {
            for (int i = 0; i < _iconsToSet.Count; i++)
            {
                m_iconsToActivate.Add(_iconsToSet[i]);
                switch (m_iconsToActivate[i])
                {
                    case QTEInput.NorthFace:
                        m_northButtonIcon.color = _color;
                        break;
                    case QTEInput.EastFace:
                        m_eastButtonIcon.color = _color;
                        break;
                    case QTEInput.SouthFace:
                        m_southButtonIcon.color = _color;
                        break;
                    case QTEInput.WestFace:
                        m_westButtonIcon.color = _color;
                        break;
                    case QTEInput.LeftShoulder:
                        m_lShoulderButtonIcon.color = _color;
                        break;
                    case QTEInput.LeftTrigger:
                        m_lTriggerButtonIcon.color = _color;
                        break;
                    case QTEInput.RightShoulder:
                        m_rShoulderButtonIcon.color = _color;
                        break;
                    case QTEInput.RightTrigger:
                        m_rTriggerButtonIcon.color = _color;
                        break;
                    case QTEInput.NorthDirectional:
                        m_northDirectionalButtonIcon.color = _color;
                        break;
                    case QTEInput.EastDirectional:
                        m_eastDirectionalButtonIcon.color = _color;
                        break;
                    case QTEInput.SouthDirectional:
                        m_southDirectionalButtonIcon.color = _color;
                        break;
                    case QTEInput.WestDirectional:
                        m_westDirectionalButtonIcon.color = _color;
                        break;
                }
            }
            m_iconsToActivate.Clear();
        }

        public void SetIconColor(QTEInput _iconToSet, Color _color)
        {
            switch (_iconToSet)
            {
                case QTEInput.NorthFace:
                    m_northButtonIcon.color = _color;
                    break;
                case QTEInput.EastFace:
                    m_eastButtonIcon.color = _color;
                    break;
                case QTEInput.SouthFace:
                    m_southButtonIcon.color = _color;
                    break;
                case QTEInput.WestFace:
                    m_westButtonIcon.color = _color;
                    break;
                case QTEInput.LeftShoulder:
                    m_lShoulderButtonIcon.color = _color;
                    break;
                case QTEInput.LeftTrigger:
                    m_lTriggerButtonIcon.color = _color;
                    break;
                case QTEInput.RightShoulder:
                    m_rShoulderButtonIcon.color = _color;
                    break;
                case QTEInput.RightTrigger:
                    m_rTriggerButtonIcon.color = _color;
                    break;
                case QTEInput.NorthDirectional:
                    m_northDirectionalButtonIcon.color = _color;
                    break;
                case QTEInput.EastDirectional:
                    m_eastDirectionalButtonIcon.color = _color;
                    break;
                case QTEInput.SouthDirectional:
                    m_southDirectionalButtonIcon.color = _color;
                    break;
                case QTEInput.WestDirectional:
                    m_westDirectionalButtonIcon.color = _color;
                    break;
            }
        }
        //Comment
        public void IncorrectInput(string _incorrectInput)
        {
            //select icon based on input            
            Image image = GetIcon(_incorrectInput);            
            //animate corresponding icon and play audio
            m_iconAnimation.IncorrectInput(image);
            m_audio.IncorrectInput();
        }

        //Comment
        public void MissedInput(List<QTEInput> _iconsToShake)
        {
            foreach(QTEInput input in _iconsToShake)
            {
                Image image = GetIcon(input);
                //animate corresponding icon and play audio
                m_iconAnimation.IncorrectInput(image);
            }
        }

        public void MissedInput(QTEInput _iconsToShake)
        {
            Image image = GetIcon(_iconsToShake);
            //animate corresponding icon and play audio
            m_iconAnimation.IncorrectInput(image);
        }

        public void SuccessfulInput(List<QTEInput> _icons, Image _ring)
        {
            
            SetIconColor(_icons, Color.green);
            StartCoroutine("ResetIconColor", _icons);
        }

        public void SuccessfulInput(QTEInput _icon, int _selector)
        {
            RectTransform rect = ActiveVisualCues[_selector].GetComponent<Image>().rectTransform;
            Vector2 size = new Vector2(rect.sizeDelta.x * 3, rect.sizeDelta.y * 3); 
            m_iconAnimation.SuccessfulInput(rect, size, 0.15f);
            SetIconColor(_icon, Color.green);
            StartCoroutine("ResetIconColor", _icon);
            FinishingCues.Add(ActiveVisualCues[_selector]);
        }


        private IEnumerator ResetIconColor(List<QTEInput> _icons)
        {
            
            yield return new WaitForSeconds(0.2f);
            Debug.Log("STOP BEING GREEN PLEASE");
            SetIconColor(_icons, Color.white);
        }

        private IEnumerator ResetIconColor(QTEInput _icon)
        {
            yield return new WaitForSeconds(0.2f);
            SetIconColor(_icon, Color.white);
        }

        public void Input(string _input)
        {
            //get corresponding icon and send through to animation script
            m_iconAnimation.InputButton(GetIcon(_input));            
        }

        public void InputReleased(string _incorrectInput)
        {   
            //get corresponding icon and send through to animation script
            m_iconAnimation.ReleaseButton(GetIcon(_incorrectInput));
        }

        /// <summary>
        /// Get Correct Image based on QTE InputSystem Action Names
        /// </summary>
        /// <param name="_input"></param>
        /// <returns></returns>  


        public Image GetIcon(string _input)
        {            
            Image image;
            switch (_input)
            {
                case "North":
                    image = m_northButtonIcon;
                    break;
                case "East":
                    image = m_eastButtonIcon;
                    break;
                case "South":
                    image = m_southButtonIcon;
                    break;
                case "West":
                    image = m_westButtonIcon;
                    break;
                case "LShoulder":
                    image = m_lShoulderButtonIcon;
                    break;
                case "LTrigger":
                    image = m_lTriggerButtonIcon;
                    break;
                case "RShoulder":
                    image = m_rShoulderButtonIcon;
                    break;
                case "RTrigger":
                    image = m_rTriggerButtonIcon;
                    break;
                case "Up":
                    image = m_northDirectionalButtonIcon;
                    break;
                case "Right":
                    image = m_eastDirectionalButtonIcon;
                    break;
                case "Down":
                    image = m_southDirectionalButtonIcon;
                    break;
                case "Left":
                    image = m_westDirectionalButtonIcon;
                    break;
                default:
                    image = null;
                    Debug.LogWarning("QTEDisplay - GetIcon(): No Corresponding Icon to String Parameter");
                    break;
            }
            return image;
        }

        public Image GetIcon(QTEInput _input)
        {
            switch (_input)
            {
                case QTEInput.NorthFace:
                    return m_northButtonIcon;                    
                case QTEInput.EastFace:
                    return m_eastButtonIcon;                    
                case QTEInput.SouthFace:
                    return m_southButtonIcon;                    
                case QTEInput.WestFace:
                    return m_westButtonIcon;                    
                case QTEInput.LeftShoulder:
                    return m_lShoulderButtonIcon;                    
                case QTEInput.LeftTrigger:
                    return m_lTriggerButtonIcon;                    
                case QTEInput.RightShoulder:
                    return m_rShoulderButtonIcon;                    
                case QTEInput.RightTrigger:
                    return m_rTriggerButtonIcon;                    
                case QTEInput.NorthDirectional:
                    return m_northDirectionalButtonIcon;                    
                case QTEInput.EastDirectional:
                    return m_eastDirectionalButtonIcon;                    
                case QTEInput.SouthDirectional:
                    return m_southDirectionalButtonIcon;                    
                case QTEInput.WestDirectional:
                    return m_westDirectionalButtonIcon;
                default:
                    return m_northButtonIcon;
            }
        }
        #endregion

        //*** Poise Bar ***//
        #region Poise Bar      

        public void ActivatePoiseBar()
        {
            m_barObject.SetActive(true);
            //m_poiseBar = m_barObject.GetComponent<PoiseBarController>();
        }

        public void DeactivatePoiseBar()
        {            
            m_barObject.SetActive(false);
        }

        #endregion

        //*** Cue ***//
        #region Cue

        //Comment
        public void CreateInputPrompt(QTEInput _input)
        {
            switch(_input)
            {
                case QTEInput.NorthFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_northButtonIcon.transform));
                    break;
                case QTEInput.EastFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_eastButtonIcon.transform));
                    break;
                case QTEInput.SouthFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_southButtonIcon.transform));
                    break;
                case QTEInput.WestFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_westButtonIcon.transform));
                    break;
                case QTEInput.LeftShoulder:
                    ActiveVisualCues.Add(Instantiate(m_lShoulderButtonCue, m_lShoulderButtonIcon.transform));
                    break;
                case QTEInput.RightShoulder:
                    ActiveVisualCues.Add(Instantiate(m_rShoulderButtonCue, m_rShoulderButtonIcon.transform));
                    break;
                case QTEInput.LeftTrigger:
                    ActiveVisualCues.Add(Instantiate(m_lTriggerCue, m_lTriggerButtonIcon.transform));
                    break;                
                case QTEInput.RightTrigger:
                    ActiveVisualCues.Add(Instantiate(m_rTriggerCue, m_rTriggerButtonIcon.transform));
                    break;
                case QTEInput.NorthDirectional:
                    ActiveVisualCues.Add(Instantiate(m_northDirectionalCue, m_northDirectionalButtonIcon.transform));
                    break;
                case QTEInput.EastDirectional:
                    ActiveVisualCues.Add(Instantiate(m_eastDirectionalCue, m_eastDirectionalButtonIcon.transform));
                    break;
                case QTEInput.SouthDirectional:
                    ActiveVisualCues.Add(Instantiate(m_southDirectionalCue, m_southDirectionalButtonIcon.transform));
                    break;
                case QTEInput.WestDirectional:
                    ActiveVisualCues.Add(Instantiate(m_westDirectionalCue, m_westDirectionalButtonIcon.transform));
                    break;
            }            
        }

        public void AnimateCue(float _targetTime, int _selector, QTEInput _input)
        {
            SetCueSize(m_cueStartSize, _selector);
            Vector2 targetSize = GetIcon(_input).rectTransform.sizeDelta * 0.8f;
            Image image = ActiveVisualCues[_selector].GetComponent<Image>();
            m_iconAnimation.StartRingAnimation(image.rectTransform, targetSize, _targetTime);
        }

        public void ShakeCue(int _selector, float _timer)
        {
            Image image = ActiveVisualCues[_selector].GetComponent<Image>();
            RectTransform ring = image.rectTransform;
            m_iconAnimation.HoldShake(ring, _timer);
        }

        public void StopShake()
        {
            m_iconAnimation.CancelShake();
        }

        //Comment
        public void SetCueSize(float _sizePercentage, int _selector)
        {            
            Vector2 targetSize = m_southButtonIcon.rectTransform.sizeDelta;
            float sizeRange = m_cueStartSize - targetSize.x;
            Image image = ActiveVisualCues[_selector].GetComponent<Image>();

            image.rectTransform.sizeDelta = new Vector2(targetSize.x + (sizeRange * _sizePercentage), targetSize.y + (sizeRange * _sizePercentage));
        }

        //Comment
        public void ActivateCue(int _count, Color _color)
        {            
            Image image = ActiveVisualCues[_count].GetComponent<Image>();
            image.color = _color;
            
        }

        public void DeactivateCue(int _count)
        {
            Image image = ActiveVisualCues[_count].GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }

        public void AnimateMashCue(float _timeLimit, int _selector, QTEInput _input)
        {
            Vector2 ringSize = GetIcon(_input).rectTransform.sizeDelta;
            Image image = ActiveVisualCues[_selector].GetComponent<Image>();
            image.rectTransform.sizeDelta = ringSize;
            m_iconAnimation.FlashRing(image);
        }

        public void StopFlash()
        {
            m_iconAnimation.CancelFlash();
        }
        
        #endregion

    }
}
