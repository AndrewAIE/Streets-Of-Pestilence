using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerController;
using UnityEngine.Windows;
using UnityEngine.Rendering;
using System.Linq;

namespace QTESystem
{
    public class QTEDisplay : MonoBehaviour
    {
        //******************** Variables *******************//
        #region Variables
        [SerializeField]
        [Tooltip("Order: N, E, S, W, LS, RS, LT, RT")]
        private GameObject[] m_buttonParents = new GameObject[8];
        private Image[,] m_buttonImages = new Image[8, 3];
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
        private SFXController_Player m_audio;        
        #endregion       

        private void Awake()
        {
            m_iconAnimation = GetComponentInChildren<QTEUIAnimation>();
            m_audio = FindObjectOfType<SFXController_Player>();            
            //assign all button parents and their icons
            for(int i = 0; i < m_buttonImages.GetLength(0); i++)
            {                
                for(int j = 0; j < m_buttonImages.GetLength(1); j++)
                {                   
                    m_buttonImages[i, j] = m_buttonParents[i].GetComponentsInChildren<Image>()[j];                    
                }                
            }
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
                SetIconColor(_iconsToSet[i], _color);
            }                          
        }

        public void ResetAllActiveIconColours() 
        {
            foreach(Image image in m_buttonImages)
            {
                if (image.gameObject.activeInHierarchy)
                {
                    image.color = Color.white;
                }                
            }
        }


        public void SetIconColor(QTEInput _iconToSet, Color _color)
        {            
            switch (_iconToSet)
            {
                case QTEInput.NorthFace:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[0, j].color = _color;
                    }
                    break;
                case QTEInput.EastFace:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[1, j].color = _color;
                    }
                    break;
                case QTEInput.SouthFace:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[2, j].color = _color;
                    }
                    break;
                case QTEInput.WestFace:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[3, j].color = _color;
                    }
                    break;
                case QTEInput.LeftShoulder:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[4, j].color = _color;
                    }
                    break;
                case QTEInput.RightShoulder:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[5, j].color = _color;
                    }
                    break;
                case QTEInput.LeftTrigger:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[6, j].color = _color;
                    }
                    break;
                case QTEInput.RightTrigger:
                    for (int j = 0; j < m_buttonImages.GetLength(1); j++)
                    {
                        m_buttonImages[7, j].color = _color;
                    }
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
            SetIconColor(GetInput(_incorrectInput), Color.red);
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
                SetIconColor(input, Color.red);
                iterator++;
            }
            m_audio.Play_Impact_QTEFailure();
        }

        public void MissedInput(QTEInput _iconToShake, int _iterator)
        {
            Image image = GetIcon(_iconToShake);
            //animate corresponding icon and play audio
            m_iconAnimation.IncorrectInput(image);
            FinishingCues.Add(ActiveVisualCues[_iterator]);
            SetIconColor(_iconToShake, Color.red);
            m_audio.Play_Impact_QTEFailure();
        }       

        public void SuccessfulInput(QTEInput _icon, int _selector)
        {
            RectTransform rect = ActiveVisualCues[_selector].GetComponent<Image>().rectTransform;
            Vector2 size = new Vector2(rect.sizeDelta.x * 3, rect.sizeDelta.y * 3); 
            m_iconAnimation.SuccessfulInput(rect, size, 0.15f);
            SetIconColor(_icon, Color.green);            
            FinishingCues.Add(ActiveVisualCues[_selector]);
            m_audio.Play_Impact_QTESuccess();
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
            switch (_input)
            {
                case "North":
                    return m_buttonImages[0,0];                    
                case "East":
                    return m_buttonImages[1, 0];
                case "South":
                    return m_buttonImages[2,0];                    
                case "West":
                    return m_buttonImages[3,0];                    
                case "LShoulder":
                    return m_buttonImages[4,0];
                case "RShoulder":
                    return m_buttonImages[5,0];
                case "LTrigger":
                    return m_buttonImages[6,0];
                case "RTrigger":
                    return m_buttonImages[7,0];
                default:                   
                    return m_buttonImages[0,0];                    
            }            
        }

        public Image GetIcon(QTEInput _input)
        {
            switch (_input)
            {
                case QTEInput.NorthFace:
                    return m_buttonImages[0,0];                  
                case QTEInput.EastFace:
                    return m_buttonImages[1,0];                  
                case QTEInput.SouthFace:
                    return m_buttonImages[2,0];                   
                case QTEInput.WestFace:
                    return m_buttonImages[3,0];                   
                case QTEInput.LeftShoulder:
                    return m_buttonImages[4,0];                 
                case QTEInput.RightShoulder:
                    return m_buttonImages[5,0];                    
                case QTEInput.LeftTrigger:
                    return m_buttonImages[6,0];                  
                case QTEInput.RightTrigger:
                    return m_buttonImages[7,0];                   
                default:
                    return m_buttonImages[0, 0];
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
                case "RShoulder":
                    return QTEInput.RightShoulder;
                case "LTrigger":
                    return QTEInput.LeftTrigger;                
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
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_buttonParents[0].transform));
                    break;
                case QTEInput.EastFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_buttonParents[1].transform));
                    break;
                case QTEInput.SouthFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_buttonParents[2].transform));
                    break;
                case QTEInput.WestFace:
                    ActiveVisualCues.Add(Instantiate(m_faceButtonCue, m_buttonParents[3].transform));
                    break;
                case QTEInput.LeftShoulder:
                    ActiveVisualCues.Add(Instantiate(m_lShoulderButtonCue, m_buttonParents[4].transform));
                    break;
                case QTEInput.RightShoulder:
                    ActiveVisualCues.Add(Instantiate(m_rShoulderButtonCue, m_buttonParents[5].transform));
                    break;
                case QTEInput.LeftTrigger:
                    ActiveVisualCues.Add(Instantiate(m_lTriggerCue, m_buttonParents[6].transform));
                    break;                
                case QTEInput.RightTrigger:
                    ActiveVisualCues.Add(Instantiate(m_rTriggerCue, m_buttonParents[7].transform));
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
        /// <param name="_index"></param>
        public void DeactivateCue(int _index)
        {
            Image image = ActiveVisualCues[_index].GetComponent<Image>();
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
                        m_iconAnimation.FadeInUI(image, _duration, Color.white);
                    }
                }
            }
            //Fade mask panel images to reduced opacity
            foreach(GameObject gObject in m_maskPanels)
            {
                Color fadedOpacity = new Color(1, 1, 1, 0.3f);
                if(gObject.activeInHierarchy)
                {
                    Image image = gObject.GetComponentInChildren<Image>();
                    m_iconAnimation.FadeInUI(image, _duration, fadedOpacity);
                    break;
                }
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
