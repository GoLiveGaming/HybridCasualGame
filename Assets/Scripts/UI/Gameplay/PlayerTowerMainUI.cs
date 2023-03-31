using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTowerMainUI : MonoBehaviour
{

    [SerializeField] protected Image healthBarImage;
    public Image HealthBarImage { get { return healthBarImage; } }
    public virtual void InitializeUI(PlayerTowerMain playerMainTower)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 ownerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, playerMainTower.transform.position);
        rectTransform.position = ownerScreenPos;



        ((RectTransform)transform).localScale = Vector3.zero;

        ((RectTransform)transform).DOScale(Vector3.one * 1.25f, 0.15f).OnComplete(() =>
            ((RectTransform)transform).DOScale(Vector3.one, 0.15f));
    }

}
