using UnityEngine;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class QTEUIAnimation : MonoBehaviour
{
    [SerializeField]
    [Range(0,1)]
    float m_pressValue, m_successVibrateStrength, m_successVibrateLength, 
        m_incorrectVibrateStrength, m_incorrectVibrateLength; 
    private TweenBase[] m_shakeTweens = new TweenBase[2];
    private TweenBase[] m_flashTweens = new TweenBase[2];
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
        Debug.Log("Stop vibrating please");
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    public void StartRingAnimation(RectTransform _ring, Vector2 _targetSize, float _timer)
    {
        Tween.Size(_ring, _targetSize, _timer, 0);
    }

    public void HoldShake(RectTransform _ring, float _timer)
    {
        if (m_shakeTweens[0] == null)
        {
            m_shakeTweens[0] = Tween.Shake(_ring.transform, Vector3.zero, new Vector3(5, 0, 0), _timer, 0);
            Gamepad.current.SetMotorSpeeds(m_incorrectVibrateStrength / 3, m_incorrectVibrateStrength);
            //Invoke("StopControllerVibrate", _timer);
        }
        else
        {
            m_shakeTweens[1] = Tween.Shake(_ring.transform, Vector3.zero, new Vector3(5, 0, 0), _timer, 0);
        }
        
        
    }

    public void CancelShake()
    {
        for(int i = 0; i < m_shakeTweens.Length; i++) 
        {
            if (m_shakeTweens[i] == null)
            {
                continue;
            }
            if (m_shakeTweens[i].Status == Tween.TweenStatus.Running)
            {
                StopControllerVibrate();
                m_shakeTweens[i].Cancel();
                m_shakeTweens[i] = null;
            }
        }        
    }

    public void FlashRing(Image _ring)
    {
        if (m_flashTweens[0] == null)
        {
            m_flashTweens[0] = Tween.Color(_ring, new Color(_ring.color.r, _ring.color.g , _ring.color.b,0), 0.25f, 0, null, Tween.LoopType.Loop);
        }
        else
        {
            m_flashTweens[1] = Tween.Color(_ring, new Color(_ring.color.r, _ring.color.g, _ring.color.b, 0), 0.25f, 0, null, Tween.LoopType.Loop);
        }        
    }

    public void CancelFlash()
    {
        for (int i = 0; i < m_flashTweens.Length; i++)
        {
            if (m_flashTweens[i] == null)
            {
                continue;
            }
            if (m_flashTweens[i].Status == Tween.TweenStatus.Running)
            {
                m_flashTweens[i].Cancel();
                m_flashTweens[i] = null;
            }
        }
    }
    
}
