using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QTESystem
{
    public class QTEDisplay : MonoBehaviour
    {

        [SerializeField]
        private Image m_northButtonIcon, m_eastButtonIcon, m_southButtonIcon, m_westButtonIcon, 
            m_lShoulderButtonIcon, m_lTriggerButtonIcon, m_rShoulderButtonIcon, m_rTriggerButtonIcon;
        [SerializeField]
        private GameObject m_iconParent;
        [SerializeField]
        private float m_activeOpacity;
        private List<QTEInput> m_iconsToActivate = new List<QTEInput>();
        [SerializeField]
        private GameObject m_barObject;
        private Slider m_poiseBar;
        [SerializeField]
        private List<GameObject> m_iconPanels;        
        public Image EndGameImage;
        [Header("Visual Cue Prefabs")]
        [SerializeField]
        GameObject m_faceButtonCue;
        [SerializeField]
        GameObject m_lShoulderButtonCue, m_rShoulderButtonCue, m_lTriggerCue, m_rTriggerCue;
        [SerializeField]
        int m_cueStartSize;
        public List<GameObject> VisualCues;

        // Start is called before the first frame update
        void Awake()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

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
                }
            }
            m_iconsToActivate.Clear();
        }

        public void IncorrectInput(string _incorrectInput)
        {
            Debug.Log(_incorrectInput);
            switch (_incorrectInput)
            {
                case "North":
                    m_northButtonIcon.color = Color.red;
                    break;
                case "East":
                    m_eastButtonIcon.color = Color.red;
                    break;
                case "South":
                    m_southButtonIcon.color = Color.red;
                    break;
                case "West":
                    m_westButtonIcon.color = Color.red;
                    break;
                case "LShoulder":
                    m_lShoulderButtonIcon.color = Color.red;
                    break;
                case "LTrigger":
                    m_lTriggerButtonIcon.color = Color.red;
                    break;
                case "RShoulder":
                    m_rShoulderButtonIcon.color = Color.red;
                    break;
                case "RTrigger":
                    m_rTriggerButtonIcon.color = Color.red;
                    break;
            }
        }

        
        public void MissedInput(List<QTEInput> _iconsToShake)
        {
            
        }
        
        public void UpdatePoiseBar(int _poiseValue)
        {
            m_poiseBar.value = _poiseValue;
        }
        

        public void ActivatePoiseBar()
        {
            m_barObject.SetActive(true);
            m_poiseBar = m_barObject.GetComponent<Slider>();
        }

        public void DeactivatePoiseBar()
        {
            m_barObject.SetActive(false);
        }
        
        public void CreateInputPrompt(QTEInput _input)
        {
            switch(_input)
            {
                case QTEInput.NorthFace:
                    VisualCues.Add(Instantiate(m_faceButtonCue, m_northButtonIcon.transform));
                    break;
                case QTEInput.EastFace:
                    VisualCues.Add(Instantiate(m_faceButtonCue, m_eastButtonIcon.transform));
                    break;
                case QTEInput.SouthFace:
                    VisualCues.Add(Instantiate(m_faceButtonCue, m_southButtonIcon.transform));
                    break;
                case QTEInput.WestFace:
                    VisualCues.Add(Instantiate(m_faceButtonCue, m_westButtonIcon.transform));
                    break;
                case QTEInput.LeftShoulder:
                    VisualCues.Add(Instantiate(m_lShoulderButtonCue, m_lShoulderButtonIcon.transform));
                    break;
                case QTEInput.RightShoulder:
                    VisualCues.Add(Instantiate(m_rShoulderButtonCue, m_rShoulderButtonIcon.transform));
                    break;
                case QTEInput.LeftTrigger:
                    VisualCues.Add(Instantiate(m_lTriggerCue, m_lTriggerButtonIcon.transform));
                    break;                
                case QTEInput.RightTrigger:
                    VisualCues.Add(Instantiate(m_rTriggerCue, m_rTriggerButtonIcon.transform));
                    break;
            }
        }

        public void SetCueSize(float _sizePercentage)
        {
            Vector2 targetSize = m_southButtonIcon.rectTransform.sizeDelta * 0.8f;
            float sizeRange = m_cueStartSize - targetSize.x;
            Image cue = VisualCues[0].GetComponent<Image>();

            cue.rectTransform.sizeDelta = new Vector2(targetSize.x + (sizeRange * _sizePercentage), targetSize.y + (sizeRange * _sizePercentage));
        }

        public void ActivateCue()
        {
            Image image = VisualCues[0].GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }        
    }
}
