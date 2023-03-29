using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class EnemySpawnMarker : MonoBehaviour
{
    public void ShowMarker(Transform targetTransform, float duration)
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        const int loopCount = 3;
        canvasGroup.DOFade(1, 0.5f);
        var mySequence01 = DOTween.Sequence().SetRecyclable(true);
        mySequence01.Append(((RectTransform)transform).DOScale(Vector3.one * 1.1f, duration / (loopCount * 2)));
        mySequence01.Append(((RectTransform)transform).DOScale(Vector3.one, duration / (loopCount * 2)));
        mySequence01.SetLoops(loopCount);


        // Calculate the position of the target in screen space
        if (Camera.main != null)
        {
            var position = targetTransform.position;
            var targetScreenPos = Camera.main.WorldToScreenPoint(position);

            // Calculate the size of the canvas and the image
            var canvasRect = transform.parent.GetComponent<RectTransform>();
            var imageRect = transform.GetComponent<RectTransform>();
            var rect = canvasRect.rect;
            var rect1 = imageRect.rect;
            var canvasWidth = rect.width;
            var canvasHeight = rect.height;
            var imageWidth = rect1.width;
            var imageHeight = rect1.height;

            // Position the image on the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPos, null, out Vector2 imagePos);
            imageRect.anchoredPosition = imagePos;

            // Clamp the position of the image to stay on the edges of the screen if it is outside of the screen
            var anchoredPosition = imageRect.anchoredPosition;
            var minX = -canvasWidth / 2f + imageWidth / 2f;
            var maxX = canvasWidth / 2f - imageWidth / 2f;
            var minY = -canvasHeight / 2f + imageHeight / 2f;
            var maxY = canvasHeight / 2f - imageHeight / 2f;

            if (targetScreenPos.x < 0 || targetScreenPos.x > Screen.width || targetScreenPos.y < 0 || targetScreenPos.y > Screen.height)
            {
                anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, minX, maxX);
                anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, minY, maxY);
                imageRect.anchoredPosition = anchoredPosition;
            }
            else
            {
                // Keep the image on the edge of the screen that is closest to the target
                var distLeft = targetScreenPos.x;
                var distRight = Screen.width - targetScreenPos.x;
                var distBottom = targetScreenPos.y;
                var distTop = Screen.height - targetScreenPos.y;

                var minDist = Mathf.Min(distLeft, distRight, distBottom, distTop);

                if (Math.Abs(minDist - distLeft) < 0.1f)
                {
                    anchoredPosition.x = minX;
                }
                else if (Math.Abs(minDist - distRight) < 0.1f)
                {
                    anchoredPosition.x = maxX;
                }
                else if (Math.Abs(minDist - distBottom) < 0.1f)
                {
                    anchoredPosition.y = minY;
                }
                else
                {
                    anchoredPosition.y = maxY;
                }

                imageRect.anchoredPosition = anchoredPosition;
            }
        }

        Destroy(gameObject, duration);
    }
}