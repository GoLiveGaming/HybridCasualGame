using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(Button))]
public class EnhancedButtonTimeIndependent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] internal float onSelectSclaeMultiplier = 1.25f;
    [SerializeField, ReadOnly] private Button m_Button;

    [Space(5), Header("ADD BUTTON EVENTS HERE INSTEAD OF BUTTON COMPONENT"), Space(5)]
    //Button Events
    [SerializeField] protected UnityEvent buttonPressedEvent = new();
    [SerializeField] protected UnityEvent buttonReleasedEvent = new();


    void Start()
    {
        m_Button = GetComponent<Button>();
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownAction();

    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpAction();
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
