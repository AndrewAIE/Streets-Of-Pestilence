using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class PoiseBarController : MonoBehaviour
{
    //******************** Variables *******************//
    #region Variables
    [SerializeField, Range(0, 1)] float _fillAmount;
    [SerializeField, Range(0, 1)] float _targetAmount;

    [Header("Poise Bar Values")]
    [SerializeField] public int _poise = 0;
    [SerializeField] public int _startPoise = -5;
    [SerializeField] public int _maxPoise = 10;
    [SerializeField] public int _minPoise = -10;

    [Header("Timing")]
    [SerializeField] float _tweenTime;
    [SerializeField] AnimationCurve _tweenCurve;

    [Header("Blood Fill")]
    [SerializeField] RectMask2D _fillMask;
    [SerializeField] float _maskPlayerWin;
    [SerializeField] float _maskEnemyWin;
    [Space]
    [SerializeField] RectTransform _bubbleRect;
    [SerializeField] float _startingYPos;
    [SerializeField] float _endYPos;


    [Header("Plunger")]
    [SerializeField] RectTransform _plungerTransform;
    [SerializeField] float _plungerX_PlayerWin;
    [SerializeField] float _plungerX_EnemyWin;

    [Header("Slider Timing Variables")]
    [SerializeField] float _timer;
    [SerializeField] float _MoveTime;

    [Header("Low Poise")]
    [SerializeField] public Animator _lowPoiseAnimator;
    [Space]
    [SerializeField] bool _isLowPoise;
    [SerializeField] float _lowPoiseThres;
    #endregion


    //******************** Methods *********************//

    private void Update()
    {
        if(isActiveAndEnabled)
        {
            if(_bubbleRect.position.y < _endYPos)
            {

                _bubbleRect.localPosition += new Vector3(0, Time.deltaTime * 20f, 0);
            }
            else
            {
                _bubbleRect.localPosition = new Vector3(0, _startingYPos, 0);
            }
        }
    }

    public void SetPoise(int deltaPoise)
    {
        _poise += deltaPoise;
        //do other stuff
        UpdatePoiseBar();

        if(_targetAmount <= _lowPoiseThres && !_isLowPoise)
        {
            _lowPoiseAnimator.SetTrigger("LowPoiseOn");
            _isLowPoise = true;
        }
        else if(_targetAmount > _lowPoiseThres && _isLowPoise) 
        {
            _lowPoiseAnimator.SetTrigger("LowPoiseOff");
            _isLowPoise = false;
        }
    }

    public void ResetPoise()
    {
        _poise = _startPoise;
        _isLowPoise = false;
        UpdatePoiseBar();

    }


    public float NormalizePoise()
    {
        return (_poise + 10f) / 20f;
    }


    public void UpdatePoiseBar()
    {
        //set a target
        _targetAmount = NormalizePoise();

        Tween.Value(_fillAmount, _targetAmount, UpdateFill, _tweenTime, 0, _tweenCurve, Tween.LoopType.None, null, null, false);

        _fillAmount = _targetAmount;
    }

    private void UpdateFill(float _amount)
    {
        _fillMask.padding = new Vector4((_maskEnemyWin - _maskPlayerWin) * _amount, 0, 0, 0);
        _plungerTransform.anchoredPosition = new Vector3(Mathf.Lerp(_plungerX_PlayerWin, _plungerX_EnemyWin, _amount), 0, 0);
    }


}
