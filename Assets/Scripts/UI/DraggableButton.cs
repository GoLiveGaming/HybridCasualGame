using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class DraggableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Space(2), Header("DRAGGABLE BUTTON PROPERTIES")]
    [SerializeField, Range(0.1f, 2f)]
    private float OnSelectSclaeMultiplier = 1.5f;
    [SerializeField] protected LayerMask layersToCollide;

    [SerializeField] protected Image costImg;

    [Space(2), Header("Readonly")]
    [ReadOnly, SerializeField] private Canvas parentCanvas;
    private bool isDragging = false;
    private Vector3 defaultPosition;


    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();

        defaultPosition = (transform as RectTransform).anchoredPosition;
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            (transform as RectTransform).anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        (transform as RectTransform).DOScale(Vector3.one * OnSelectSclaeMultiplier, 0.25f);
        //costImg.gameObject.SetActive(false);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        (transform as RectTransform).DOScale(Vector3.zero, 0.25f).OnComplete(() =>
        {
            (transform as RectTransform).DOAnchorPos(defaultPosition, 0.25f).OnComplete(() =>
            {
                (transform as RectTransform).DOScale(Vector3.one, 0.25f);
                //costImg.gameObject.SetActive(true);
            });
        });
    }
}



