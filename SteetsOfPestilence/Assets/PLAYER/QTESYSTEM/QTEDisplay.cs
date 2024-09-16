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
        public List<GameObject> VisualCues;

        [HideInInspector]
        public PoiseBarController poiseBarController;

        #endregion

        //******************** Methods *********************//
        #region Methods

        //*** Panel ***//
        #region Panel

        //Comment
        public void ActivatePanel(int _indicator)
        {
            m_iconPanels[_indicator].SetActive(true);
        }

        //Comment
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

        //Comment
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
                case "Up":
                    m_northDirectionalButtonIcon.color = Color.red;
                    break;
                case "Right":
                    m_eastDirectionalButtonIcon.color = Color.red;
                    break;
                case "Down":
                    m_southDirectionalButtonIcon.color = Color.red;
                    break;
                case "Left":
                    m_westDirectionalButtonIcon.color = Color.red;
                    break;
            }
        }

        //Comment
        public void MissedInput(List<QTEInput> _iconsToShake)
        {
            
        }

        #endregion

        //*** Poise Bar ***//
        #region Poise Bar
/*
        //Comment
        public void UpdatePoiseBar(int _poiseValue)
        {

        }*/

        //Activate Poise Bar
        public void ActivatePoiseBar()
        {
            m_barObject.SetActive(true);
            m_poiseBar = m_barObject.GetComponent<PoiseBarController>();
        }

        //Deactivate Poise Bar
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
                case QTEInput.NorthDirectional:
                    VisualCues.Add(Instantiate(m_northDirectionalCue, m_northDirectionalButtonIcon.transform));
                    break;
                case QTEInput.EastDirectional:
                    VisualCues.Add(Instantiate(m_eastDirectionalCue, m_eastDirectionalButtonIcon.transform));
                    break;
                case QTEInput.SouthDirectional:
                    VisualCues.Add(Instantiate(m_southDirectionalCue, m_southDirectionalButtonIcon.transform));
                    break;
                case QTEInput.WestDirectional:
                    VisualCues.Add(Instantiate(m_westDirectionalCue, m_westDirectionalButtonIcon.transform));
                    break;
            }
        }

        //Comment
        public void SetCueSize(float _sizePercentage)
        {
            Vector2 targetSize = m_southButtonIcon.rectTransform.sizeDelta * 0.8f;
            float sizeRange = m_cueStartSize - targetSize.x;
            Image cue = VisualCues[0].GetComponent<Image>();

            cue.rectTransform.sizeDelta = new Vector2(targetSize.x + (sizeRange * _sizePercentage), targetSize.y + (sizeRange * _sizePercentage));
        }

        //Comment
        public void ActivateCue()
        {
            Image image = VisualCues[0].GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }

        #endregion

        #endregion
    }
}
