using UnityEngine;
using Pixelplacement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class QTEUIAnimation : MonoBehaviour
{
    [SerializeField]
    [Range(0,1)]
    float m_pressValue, m_successVibrateStrength, m_successVibrateLength, 
        m_incorrectVibrateStrength, m_incorrectVibrateLength; 

    public void SuccessfulInput(RectTransform _ring, Vector2 _targetSize, float _timer)
    {
        Tween.Size(_ring, _targetSize, _timer, 0);
        Image image = _ring.gameObject.GetComponent<Image>();
        Color color = new Color(image.color.r, image.color.g, image.color.b, 0);
        Tween.Color(image, color, _timer, 0);
        Gamepad.current.SetMotorSpeeds(m_successVibrateStrength / 3, m_successVibrateStrength);
        Invoke("StopControllerVibrate", m_successVibrateLength);
    }

    public void FailAction(GameObject _icon)
    {         
        Tween.Shake(_icon.transform, Vector3.zero, new Vector3(5, 0, 0), m_incorrectVibrateLength, 0);
        Gamepad.current.SetMotorSpeeds(m_incorrectVibrateStrength / 3, m_incorrectVibrateStrength);        
        Invoke("StopControllerVibrate", m_incorrectVibrateLength);
    }

    public void IncorrectInput(GameObject _icon)
    {
        Tween.Shake(_icon.transform, Vector3.zero, new Vector3(5, 0, 0), m_incorrectVibrateLength, 0);
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

    public void StartRingAnimation(RectTransform _ring, Vector2 _targetSize, float _timer)
    {
        Tween.Size(_ring, _targetSize, _timer, 0);
    }

    public void HoldShake(RectTransform _ring, float _timer)
    {
        Tween.Shake(_ring.transform, Vector3.zero, new Vector3(5, 0, 0), _timer, 0);
        Gamepad.current.SetMotorSpeeds(m_incorrectVibrateStrength / 3, m_incorrectVibrateStrength);
        Invoke("StopControllerVibrate", _timer);
    }

    public void FlashRing(Image _ring, float _timer)
    {
        Tween.Color(_ring, Color.clear, 0.1f, 0, null, Tween.LoopType.PingPong);
    }
    
}
