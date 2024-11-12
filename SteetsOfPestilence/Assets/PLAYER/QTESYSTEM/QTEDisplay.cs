using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private GameObject m_northButtonParent;
        private Image m_northButtonIcon;
        private Image m_northButtonText;
        [SerializeField]
        private GameObject m_eastButtonParent;
        private Image m_eastButtonIcon;
        private Image m_eastButtonText;
        [SerializeField]
        private GameObject m_southButtonParent;
        private Image m_southButtonIcon;
        private Image m_southButtonText;
        [SerializeField]
        private GameObject m_westButtonParent;
        private Image m_westButtonIcon;
        private Image m_westButtonText;
        [SerializeField]
        private GameObject m_lShoulderButtonParent;
        private Image m_lShoulderButtonIcon;
        private Image m_lShoulderButtonText;
        [SerializeField]
        private GameObject m_lTriggerButtonParent;
        private Image m_lTriggerButtonIcon;
        private Image m_lTriggerButtonText;
        [SerializeField]
        private GameObject m_rShoulderButtonParent;
        private Image m_rShoulderButtonIcon;
        private Image m_rShoulderButtonText;
        [SerializeField]
        private GameObject m_rTriggerButtonParent;
        private Image m_rTriggerButtonIcon;
        private Image m_rTriggerButtonText;

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
        GameObject m_lShoulderButtonCue, m_rShoulderButtonCue, m_lTriggerCue, m_rTriggerCue;

        [SerializeField]
        float m_cueSizeRatio;
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

            // North button assignments
            m_northButtonIcon = m_northButtonParent.GetComponentInChildren<Image>();
            m_northButtonText = m_northButtonParent.GetComponentsInChildren<Image>()[1];

            // East button assignments
            m_eastButtonIcon = m_eastButtonParent.GetComponentInChildren<Image>();
            m_eastButtonText = m_eastButtonParent.GetComponentsInChildren<Image>()[1];

            // South button assignments
            m_southButtonIcon = m_southButtonParent.GetComponentInChildren<Image>();
            m_southButtonText = m_southButtonParent.GetComponentsInChildren<Image>()[1];

            // West button assignments
            m_westButtonIcon = m_westButtonParent.GetComponentInChildren<Image>();
            m_westButtonText = m_westButtonParent.GetComponentsInChildren<Image>()[1];

            // Left Shoulder button assignments
            m_lShoulderButtonIcon = m_lShoulderButtonParent.GetComponentInChildren<Image>();
            m_lShoulderButtonText = m_lShoulderButtonParent.GetComponentsInChildren<Image>()[1];

            // Left Trigger button assignments
            m_lTriggerButtonIcon = m_lTriggerButtonParent.GetComponentInChildren<Image>();
            m_lTriggerButtonText = m_lTriggerButtonParent.GetComponentsInChildren<Image>()[1];

            // Right Shoulder button assignments
            m_rShoulderButtonIcon = m_rShoulderButtonParent.GetComponentInChildren<Image>();
            m_rShoulderButtonText = m_rShoulderButtonParent.GetComponentsInChildren<Image>()[1];

            // Right Trigger button assignments
            m_rTriggerButtonIcon = m_rTriggerButtonParent.GetComponentInChildren<Image>();
            m_rTriggerButtonText = m_rTriggerButtonParent.GetComponentsInChildren<Image>()[1];

        }
        //*** Panel ***//
        [SerializeField]
        private GameObject[] m_maskPanels;
        #region Panel       
        
        public void DeactivatePanels()
        {
            foreach (GameObject panel in m_iconPanels)
            {
                panel.SetActive(false);
            }
            foreach (GameObject panel in m_maskPanels)
            {
                panel.SetActive(false);            
            }
        }

        internal void LoadUI(EnemyAI.EnemyType _enemyType)
        {
            switch (_enemyType)
            {
                case EnemyAI.EnemyType.Rabbit:
                    m_iconPanels[0].SetActive(true);
                    m_iconPanels[2].SetActive(true);
                    m_maskPanels[0].SetActive(true);
                    break;
                case EnemyAI.EnemyType.Rat:
                    m_iconPanels[1].SetActive(true);
                    m_iconPanels[2].SetActive(true);
                    m_maskPanels[2].SetActive(true);
                    break;
                case EnemyAI.EnemyType.Dog:
                    m_iconPanels[2].SetActive(true);
                    m_maskPanels[1].SetActive(true);
                    break;
                case EnemyAI.EnemyType.Boss:
                    m_iconPanels[0].SetActive(true);
                    m_iconPanels[1].SetActive(true);
                    m_iconPanels[2].SetActive(true);
                    m_maskPanels[3].SetActive(true);
                    break;
                default:
                    break;
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
                switch (_iconsToSet[i])
                {                    
                    case QTEInput.NorthFace:
                        m_northButtonIcon.color = _color;
                        m_northButtonText.color = _color;
                        break;
                    case QTEInput.EastFace:
                        m_eastButtonIcon.color = _color;
                        m_eastButtonText.color = _color;
                        break;
                    case QTEInput.SouthFace:
                        m_southButtonIcon.color = _color;
                        m_southButtonText.color = _color;
                        break;
                    case QTEInput.WestFace:
                        m_westButtonIcon.color = _color;
                        m_westButtonText.color = _color;
                        break;
                    case QTEInput.LeftShoulder:
                        m_lShoulderButtonIcon.color = _color;
                        m_lShoulderButtonText.color = _color;
                        break;
                    case QTEInput.LeftTrigger:
                        m_lTriggerButtonIcon.color = _color;
                        m_lTriggerButtonText.color = _color;
                        break;
                    case QTEInput.RightShoulder:
                        m_rShoulderButtonIcon.color = _color;
                        m_rShoulderButtonText.color = _color;
                        break;
                    case QTEInput.RightTrigger:
                        m_rTriggerButtonIcon.color = _color;
                        m_rTriggerButtonText.color = _color;
                        break;
                }
            }           
        }

        public void SetIconColor(QTEInput _iconToSet, Color _color)
        {            
            switch (_iconToSet)
            {
                case QTEInput.NorthFace:
                    m_northButtonIcon.color = _color;
                    m_northButtonText.color = _color;
                    break;
                case QTEInput.EastFace:
                    m_eastButtonIcon.color = _color;
                    m_eastButtonText.color = _color;
                    break;
                case QTEInput.SouthFace:
                    m_southButtonIcon.color = _color;
                    m_southButtonText.color = _color;
                    break;
                case QTEInput.WestFace:
                    m_westButtonIcon.color = _color;
                    m_westButtonText.color = _color;
                    break;
                case QTEInput.LeftShoulder:
                    m_lShoulderButtonIcon.color = _color;
                    m_lShoulderButtonText.color = _color;
                    break;
                case QTEInput.LeftTrigger:
                    m_lTriggerButtonIcon.color = _color;
                    m_lTriggerButtonText.color = _color;
                    break;
                case QTEInput.RightShoulder:
                    m_rShoulderButtonIcon.color = _color;
                    m_rShoulderButtonText.color = _color;
                    break;
                case QTEInput.RightTrigger:
                    m_rTriggerButtonIcon.color = _color;
                    m_rTriggerButtonText.color = _color;
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
            FinishingCues.Add(ActiveVisualCues[0]);
        }

        //Comment
        public void MissedInput(List<QTEInput> _iconsToShake)
        {
            int iterator = 0;
            foreach(QTEInput input in _iconsToShake)
            {
                Image image = GetIcon(input);
                //animate corresponding icon and play audio
                m_iconAnimation.IncorrectInput(image);
                FinishingCues.Add(ActiveVisualCues[iterator]);
                iterator++;
            }
        }

        public void MissedInput(QTEInput _iconsToShake, int _iterator)
        {
            Image image = GetIcon(_iconsToShake);
            //animate corresponding icon and play audio
            m_iconAnimation.IncorrectInput(image);
            FinishingCues.Add(ActiveVisualCues[_iterator]);
        }       

        public void SuccessfulInput(QTEInput _icon, int _selector)
        {
            RectTransform rect = ActiveVisualCues[_selector].GetComponent<Image>().rectTransform;
            Vector2 size = new Vector2(rect.sizeDelta.x * 3, rect.sizeDelta.y * 3); 
            m_iconAnimation.SuccessfulInput(rect, size, 0.15f);
            SetIconColor(_icon, Color.green);            
            FinishingCues.Add(ActiveVisualCues[_selector]);
        }       

        public void Input(string _input)
        {
            //get corresponding icon and send through to animation script
            m_iconAnimation.InputButton(GetIcon(_input));            
        }

        public void InputReleased(string _incorrectInput)
        {   
            //get corresponding icon and send through to animation script
            SetIconColor(GetInput(_incorrectInput), Color.white);
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
                default:
                    return m_northButtonIcon;
            }
        }

        public QTEInput GetInput(string _inputName)
        {
            switch (_inputName)
            {
                case "North":
                    return QTEInput.NorthFace;
                case "East":
                    return QTEInput.EastFace;
                case "South":
                    return QTEInput.SouthFace;
                case "West":
                    return QTEInput.WestFace;
                case "LShoulder":
                    return QTEInput.LeftShoulder;
                case "LTrigger":
                    return QTEInput.LeftTrigger;
                case "RShoulder":
                    return QTEInput.RightShoulder;
                case "RTrigger":
                    return QTEInput.RightTrigger;
                default:
                    return QTEInput.NorthFace;
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
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_northButtonParent.transform));
                    break;
                case QTEInput.EastFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_eastButtonParent.transform));
                    break;
                case QTEInput.SouthFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_southButtonParent.transform));
                    break;
                case QTEInput.WestFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_westButtonParent.transform));
                    break;
                case QTEInput.LeftShoulder:
                    ActiveVisualCues.Add(Instantiate(m_lShoulderButtonCue, m_lShoulderButtonParent.transform));
                    break;
                case QTEInput.RightShoulder:
                    ActiveVisualCues.Add(Instantiate(m_rShoulderButtonCue, m_rShoulderButtonParent.transform));
                    break;
                case QTEInput.LeftTrigger:
                    ActiveVisualCues.Add(Instantiate(m_lTriggerCue, m_lTriggerButtonParent.transform));
                    break;                
                case QTEInput.RightTrigger:
                    ActiveVisualCues.Add(Instantiate(m_rTriggerCue, m_rTriggerButtonParent.transform));
                    break;
            }            
        }

        public void AnimateCue(float _targetTime, int _selector, QTEInput _input)
        {
            Vector2 targetSize = GetIcon(_input).rectTransform.sizeDelta;            
            Image image = ActiveVisualCues[_selector].GetComponent<Image>();
            SetCueSize(m_cueSizeRatio, _selector, image.rectTransform);
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

        /// <summary>
        /// Set the size of the cue before it is activated
        /// </summary>
        /// <param name="_sizePercentage"></param>
        /// <param name="_selector"></param>
        /// <param name="_transform"></param>
        public void SetCueSize(float _sizePercentage, int _selector, RectTransform _transform)
        {           
            
            Vector2 targetSize = _transform.sizeDelta;
            float sizeRangeX = (m_cueSizeRatio * targetSize.x) - targetSize.x;
            float sizeRangeY = (m_cueSizeRatio * targetSize.y) - targetSize.y;
            Image image = ActiveVisualCues[_selector].GetComponent<Image>();

            image.rectTransform.sizeDelta = new Vector2(targetSize.x + (sizeRangeX * _sizePercentage), targetSize.y + (sizeRangeY * _sizePercentage));
        }

        /// <summary>
        /// Turn alpha of Ring Colour from 0 to 1 (255)
        /// </summary>
        /// <param name="_count"></param>
        /// <param name="_color"></param>
        public void ActivateCue(int _count, Color _color)
        {            
            Image image = ActiveVisualCues[_count].GetComponent<Image>();
            image.color = _color;
            
        }
        /// <summary>
        /// Turn alphpa of cue to 0
        /// </summary>
        /// <param name="_count"></param>
        public void DeactivateCue(int _count)
        {
            Image image = ActiveVisualCues[_count].GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        /// <summary>
        /// Make the cue flash to signify that the player needs to mash the button
        /// </summary>
        /// <param name="_timeLimit"></param>
        /// <param name="_selector"></param>
        /// <param name="_input"></param>
        public void AnimateMashCue(float _timeLimit, int _selector, QTEInput _input)
        {
            Vector2 ringSize = GetIcon(_input).rectTransform.sizeDelta;
            Image image = ActiveVisualCues[_selector].GetComponent<Image>();
            image.rectTransform.sizeDelta = ringSize;
            m_iconAnimation.FlashRing(image);
        }
        //cancel the flash of the cue 
        public void StopFlash()
        {
            m_iconAnimation.CancelFlash();
        }
        /// <summary>
        /// Fade Active UI panels to 1 alpha
        /// </summary>
        /// <param name="_duration"></param>
        public void FadeInUI(float _duration)
        {

            List<Image> images = new List<Image>();
            //Get all Images from the Poise bar
            Image[] poiseBarImages = m_poiseBar.GetComponentsInChildren<Image>();            
            foreach (Image image in poiseBarImages)
            {
                images.Add(image);
            }
            //Get all Images from the active icon panels
            foreach (GameObject gObject in m_iconPanels)
            {
                if (gObject.activeInHierarchy)
                {
                    Image[] iconImages = gObject.GetComponentsInChildren<Image>();
                    foreach(Image image in iconImages)
                    {
                        images.Add(image);
                    }
                }
            }
            //Get active mask panel image
            foreach(GameObject gObject in m_maskPanels)
            {
                if(gObject.activeInHierarchy)
                {
                    Image image = gObject.GetComponentInChildren<Image>();
                    images.Add(image);
                    break;
                }
            }
            //Fade all UI images
            foreach(Image image in images)
            {
                m_iconAnimation.FadeInUI(image, _duration);
            }           
        }
        /// <summary>
        /// Fade Active UI panels to 0 alpha
        /// </summary>
        /// <param name="_duration"></param>
        public void FadeOutUI(float _duration)
        {
            List<Image> images = new List<Image>();
            //Get all Images from the Poise bar
            Image[] poiseBarImages = m_poiseBar.GetComponentsInChildren<Image>();
            foreach (Image image in poiseBarImages)
            {
                images.Add(image);
            }
            //Get all Images from the active icon panels
            foreach (GameObject gObject in m_iconPanels)
            {
                if (gObject.activeInHierarchy)
                {
                    Image[] iconImages = gObject.GetComponentsInChildren<Image>();
                    foreach (Image image in iconImages)
                    {
                        images.Add(image);
                    }
                }
            }
            //Get active mask panel image
            foreach (GameObject gObject in m_maskPanels)
            {
                if (gObject.activeInHierarchy)
                {
                    Image image = gObject.GetComponentInChildren<Image>();
                    images.Add(image);
                    break;
                }
            }
            //Fade all UI images
            foreach (Image image in images)
            {
                m_iconAnimation.FadeOutUI(image, _duration);
            }
        }

        public void Pause()
        {
            m_iconAnimation.Pause();
        }

        public void Resume()
        {
            m_iconAnimation.Resume();
        }

        public void ClearTweens()
        {
            m_iconAnimation.ClearTweens();
        }
        #endregion

    }
}
