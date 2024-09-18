using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.UI;
using TMPro.EditorUtilities;
using Pixelplacement.TweenSystem;
using UnityEngine.InputSystem;

public class QTEUIAnimation : MonoBehaviour
{
    [SerializeField]
    [Range(0,1)]
    float m_pressValue, m_successVibrateStrength, m_successVibrateLength, 
        m_incorrectVibrateStrength, m_incorrectVibrateLength; 

    public void SuccessfulInput(GameObject _icon)
    {
        Gamepad.current.SetMotorSpeeds(m_successVibrateStrength / 3, m_successVibrateStrength);
        Invoke("StopControllerVibrate", m_successVibrateLength);
    }

    public void FailAction(GameObject _icon)
    {

    }

    public void IncorrectInput(GameObject _icon)
    {
        Tween.Shake(_icon.transform, _icon.transform.localPosition, new Vector3(5, 0, 0), m_incorrectVibrateLength, 0);
        Gamepad.current.SetMotorSpeeds(m_incorrectVibrateStrength/3,m_incorrectVibrateStrength);
        Invoke("StopControllerVibrate", m_incorrectVibrateLength);
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

    public void StopControllerVibrate()
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}
