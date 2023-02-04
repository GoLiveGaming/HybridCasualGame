using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        transform.DOScale(Vector3.one, 0.25f);
    }
    private void OnDisable()
    {
        transform.DOScale(Vector3.zero, 0.1f);
    }

    public void FireUnitSelected()
    {
        
    }

    public void WindUnitSelected()
    {

    }
}