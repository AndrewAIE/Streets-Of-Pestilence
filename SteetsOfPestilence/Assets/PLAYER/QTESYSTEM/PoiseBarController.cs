using System.Collections;
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
    [SerializeField] public int _maxPoise = 10;
    [SerializeField] public int _minPoise = -10;

    [Header("Timing")]
    [SerializeField] float _tweenTime;
    [SerializeField] AnimationCurve _tweenCurve;

    [Header("Blood Fill")]
    [SerializeField] RectMask2D _fillMask;
    [SerializeField] float _maskPlayerWin;
    [SerializeField] float _maskEnemyWin;

    [Header("Plunger")]
    [SerializeField] RectTransform _plungerTransform;
    [SerializeField] float _plungerX_PlayerWin;
    [SerializeField] float _plungerX_EnemyWin;

    [Header("Slider Timing Variables")]
    [SerializeField] float _timer;
    [SerializeField] float _MoveTime;
    #endregion


    //******************** Methods *********************//
    public void SetPoise(int deltaPoise)
    {
        _poise += deltaPoise;

        //do other stuff
        UpdatePoiseBar();
    }

    public void ResetPoise()
    {
        _poise = 0;

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

        Tween.Value(_fillAmount, _targetAmount, UpdateFill, _tweenTime, 0, _tweenCurve);

        _fillAmount = _targetAmount;
    }

    private void UpdateFill(float _amount)
    {
        _fillMask.padding = new Vector4((_maskEnemyWin - _maskPlayerWin) * _amount, 0, 0, 0);
        _plungerTransform.anchoredPosition = new Vector3(Mathf.Lerp(_plungerX_PlayerWin, _plungerX_EnemyWin, _amount), 0, 0);

    }


}
