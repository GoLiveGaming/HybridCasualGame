using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class EnhancedPanels : MonoBehaviour
{
    [SerializeField] private float moveTime = 0.25f;
    [SerializeField] private float fadeTime = 0.2f;
    [SerializeField] private Vector2 moveInOffset = new(4000, 0);
    [SerializeField] private Vector2 moveOutOffset = new(-4000, 0);

    [SerializeField] public UnityEvent OnPanelActivation = new();
    [SerializeField] public UnityEvent OnPanelDeactivation = new();
    CanvasGroup Image;

    void Awake()
    {
        Image = GetComponent<CanvasGroup>();
    }
    public void TogglePanel()
    {
        if (gameObject.activeSelf)
            DisablePanel();
        else
            EnablePanel();
    }
    private void EnablePanel()
    {
        (transform as RectTransform).anchoredPosition = moveInOffset;
        transform.gameObject.SetActive(true);
        Image.DOFade(1, fadeTime);
        (transform as RectTransform).DOAnchorPos(Vector3.zero, moveTime);
        OnActivation();
    }

    private void DisablePanel()
    {
        (transform as RectTransform).anchoredPosition = Vector3.zero;

        OnDeactivation();
        Image.DOFade(0, fadeTime);
        (transform as RectTransform).DOAnchorPos(moveOutOffset, moveTime).OnComplete(() =>
        {
            transform.gameObject.SetActive(false);
        });
        OnDeactivation();

    }

    private void OnActivation()
    {
        OnPanelActivation?.Invoke();
        OnPanelActivation.RemoveAllListeners();
    }
    private void OnDeactivation()
    {
        OnPanelDeactivation?.Invoke();
        OnPanelDeactivation.RemoveAllListeners();
    }

}