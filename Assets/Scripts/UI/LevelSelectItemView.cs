using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(Canvas))]
public class LevelSelectItemView : MonoBehaviour
{
    [Header("BUTTON ANIMATION PARAMETERS"), Space(2)]
    [SerializeField] private float onDeselectScaleMultiplier = 0.75f;

    [Header("LEVEL SELECTION PARAMETERS"), Space(2)]
    [SerializeField] private CanvasGroup deselectedPanel;
    [SerializeField] private GameObject lockedIndicator;
    [SerializeField] private int canvasDefaultSortingOrder;

    private Canvas canvas;

    [ReadOnly] public bool isSelected = false;
    [ReadOnly] public bool isUnlocked = false;


    public void Initialize(bool isUnlocked)
    {
        canvas = GetComponent<Canvas>();
        canvas.sortingOrder = canvasDefaultSortingOrder;
        Deselect();
        SetLocked(isUnlocked);
    }
    private void SetLocked(bool value)
    {
        isUnlocked = value;
        lockedIndicator.SetActive(!value);

    }
    public void Select()
    {
        isSelected = true;
        canvas.sortingOrder = canvasDefaultSortingOrder + 1;
        deselectedPanel.DOFade(0, 0.25f);
        (transform as RectTransform).DOScale(Vector3.one, 0.25f);

    }

    public void Deselect()
    {
        isSelected = false;
        canvas.sortingOrder = canvasDefaultSortingOrder;
        deselectedPanel.DOFade(1, 0.25f);
        (transform as RectTransform).DOScale(Vector3.one * onDeselectScaleMultiplier, 0.25f);
    }
}
