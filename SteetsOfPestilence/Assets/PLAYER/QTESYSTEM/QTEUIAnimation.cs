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

    [SerializeField] AnimationCurve ringShrinkCurve;
    public void SuccessfulInput(RectTransform _ring, Vector2 _targetSize, float _timer)
    {
        Tween.Size(_ring, _targetSize, _timer, 0, null, Tween.LoopType.None, null, null, false);
        Image image = _ring.gameObject.GetComponent<Image>();
        image.color = Color.green;
        Color color = new Color(0, 1, 0, 0);
        Tween.Color(image, color, _timer, 0, null, Tween.LoopType.None, null, null, false);
    }

    public void FailAction(GameObject _icon)
    {         
        Tween.Shake(_icon.transform, Vector3.zero, new Vector3(5, 0, 0), m_incorrectVibrateLength, 0, Tween.LoopType.None, null, null, false);
        Gamepad.current.SetMotorSpeeds(m_incorrectVibrateStrength / 3, m_incorrectVibrateStrength);
        StartCoroutine("StopControllerVibrate");
    }

    public void IncorrectInput(Image _icon)
    {
        Tween.Shake(_icon.transform, Vector3.zero, new Vector3(5, 0, 0), m_incorrectVibrateLength, 0, Tween.LoopType.None, null, null, false);
        Gamepad.current.SetMotorSpeeds(m_incorrectVibrateStrength/3,m_incorrectVibrateStrength);
        StartCoroutine("StopControllerVibrate");
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

    public IEnumerator StopControllerVibrate()
    {
        yield return new WaitForSecondsRealtime(m_incorrectVibrateLength);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    public void StartRingAnimation(RectTransform _ring, Vector2 _targetSize, float _timer)
    {
        Tween.Size(_ring, _targetSize, _timer, 0, ringShrinkCurve, Tween.LoopType.None, null, null, false);
    }

    public void HoldShake(RectTransform _ring, float _timer)
    {
        if (m_shakeTweens[0] == null)
        {
            m_shakeTweens[0] = Tween.Shake(_ring.transform, Vector3.zero, new Vector3(5, 0, 0), _timer, 0);           ;
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
                m_shakeTweens[i].Cancel();
                m_shakeTweens[i] = null;
            }
        }        
    }

    public void FlashRing(Image _ring)
    {
        if (m_flashTweens[0] == null)
        {
            _ring.color = Color.white;
            m_flashTweens[0] = Tween.Color(_ring, new Color(_ring.color.r, _ring.color.g , _ring.color.b,0), 0.25f, 0, null, Tween.LoopType.Loop, null, null, false);
        }
        else
        {
            StartCoroutine(FlashRingTwo(_ring));
        }        
    }

    private IEnumerator FlashRingTwo(Image _ring)
    {
        yield return new WaitForSecondsRealtime(0.125f);
        _ring.color = Color.white;
        m_flashTweens[1] = Tween.Color(_ring, new Color(_ring.color.r, _ring.color.g, _ring.color.b, 0), 0.25f, 0, null, Tween.LoopType.Loop, null, null, false);
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
