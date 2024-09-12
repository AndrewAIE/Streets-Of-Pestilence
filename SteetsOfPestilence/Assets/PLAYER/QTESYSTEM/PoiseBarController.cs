using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoiseBarController : MonoBehaviour
{
    [SerializeField, Range(0,1)] float _fillAmount;

    [Header("Blood Fill")]
    [SerializeField] RectMask2D _fillMask;
    [SerializeField] float _maskPlayerWin;
    [SerializeField] float _maskEnemyWin;

    [Header("Plunger")]
    [SerializeField] RectTransform _plungerTransform;
    [SerializeField] float _plungerX_PlayerWin;
    [SerializeField] float _plungerX_EnemyWin;

    
    public IEnumerator UpdatePoiseBar(float deltaPoise)
    {
        _fillAmount += deltaPoise;

        _fillMask.padding = new Vector4((_maskEnemyWin - _maskPlayerWin) * _fillAmount, 0, 0, 0);
        _plungerTransform.anchoredPosition = new Vector3(Mathf.Lerp(_plungerX_PlayerWin, _plungerX_EnemyWin, _fillAmount), 0, 0);
        yield return null;
    }




}
