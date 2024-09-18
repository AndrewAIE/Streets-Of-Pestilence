using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.UI;
using TMPro.EditorUtilities;

public class QTEUIAnimation : MonoBehaviour
{
    

    [SerializeField]
    [Range(0,1)]
    float m_pressValue;

    public void SuccessfulInput(GameObject _icon)
    {

    }

    public void FailAction(GameObject _icon)
    {

    }

    public void IncorrectInput(GameObject _icon)
    {
        Tween.Shake(_icon.transform, _icon.transform.position, new Vector3(0, 5, 0), 1, 0);
    }

    public void InputButton(Image _icon)
    {
        Color color = new Color(_icon.color.r * m_pressValue, _icon.color.g * m_pressValue, _icon.color.b * m_pressValue);
        _icon.color = color;
    }

    public void ReleaseButton(Image _icon)
    {
        _icon.color = Color.white;
    }

}
