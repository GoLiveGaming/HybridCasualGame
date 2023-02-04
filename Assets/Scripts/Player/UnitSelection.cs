using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    private void OnEnable()
    {
        transform.DOScale(Vector3.one, 0.5f);
    }
    private void OnDisable()
    {
        transform.localScale = Vector3.zero;
    }
}
