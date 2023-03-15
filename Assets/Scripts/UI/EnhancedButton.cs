using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(Button))]
public class EnhancedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] internal float onSelectScaleMultiplier = 1.25f;
    [SerializeField] internal float onSelectResponseTime = 0.15f;
    [SerializeField, ReadOnly] private Button m_Button;

    [Space(5), Header("ADD BUTTON EVENTS HERE INSTEAD OF BUTTON COMPONENT"), Space(5)]
    //Button Events
    [SerializeField] protected UnityEvent buttonPressedEvent = new();
    [SerializeField] protected UnityEvent buttonReleasedEvent = new();


    protected virtual void Start()
    {
        m_Button = GetComponent<Button>();
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        (transform as RectTransform).DOScale(Vector3.one * onSelectScaleMultiplier, onSelectResponseTime).OnComplete(() =>
        {
            OnPointerDownAction();
        });

    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        (transform as RectTransform).DOScale(Vector3.one, onSelectResponseTime).OnComplete(() =>
        {
            OnPointerUpAction();
        });
    }

    protected void OnPointerDownAction()
    {
        if (!m_Button.IsInteractable()) return;
        buttonPressedEvent?.Invoke();
    }
    protected void OnPointerUpAction()
    {
        if (!m_Button.IsInteractable()) return;
        buttonReleasedEvent?.Invoke();
    }
}
