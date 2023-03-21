using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawnMarker : MonoBehaviour
{
    public EnemyTypes enemyType;
    [SerializeField] private float markerduration = 3;
    [SerializeField] private TextMeshProUGUI enemyCountText;

    public void ShowMarker(Transform targetTransform, int enemyCount)
    {
        enemyCountText.text = enemyCount.ToString();
        // Calculate the position of the target in screen space
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(targetTransform.position);

        // Calculate the size of the canvas and the image
        RectTransform canvasRect = transform.parent.GetComponent<RectTransform>();
        RectTransform imageRect = transform.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        float imageWidth = imageRect.rect.width;
        float imageHeight = imageRect.rect.height;

        // Position the image on the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPos, null, out Vector2 imagePos);
        imageRect.anchoredPosition = imagePos;

        // Clamp the position of the image to stay on the edges of the screen if it is outside of the screen
        Vector2 anchoredPosition = imageRect.anchoredPosition;
        float minX = -canvasWidth / 2f + imageWidth / 2f;
        float maxX = canvasWidth / 2f - imageWidth / 2f;
        float minY = -canvasHeight / 2f + imageHeight / 2f;
        float maxY = canvasHeight / 2f - imageHeight / 2f;

        if (targetScreenPos.x < 0 || targetScreenPos.x > Screen.width || targetScreenPos.y < 0 || targetScreenPos.y > Screen.height)
        {
            anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, minX, maxX);
            anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, minY, maxY);
            imageRect.anchoredPosition = anchoredPosition;
        }
        else
        {
            // Keep the image on the edge of the screen that is closest to the target
            float distLeft = targetScreenPos.x;
            float distRight = Screen.width - targetScreenPos.x;
            float distBottom = targetScreenPos.y;
            float distTop = Screen.height - targetScreenPos.y;

            float minDist = Mathf.Min(distLeft, distRight, distBottom, distTop);

            if (minDist == distLeft)
            {
                anchoredPosition.x = minX;
            }
            else if (minDist == distRight)
            {
                anchoredPosition.x = maxX;
            }
            else if (minDist == distBottom)
            {
                anchoredPosition.y = minY;
            }
            else
            {
                anchoredPosition.y = maxY;
            }

            imageRect.anchoredPosition = anchoredPosition;
        }
        StartCoroutine(EndShowMarker());
    }

    private IEnumerator EndShowMarker()
    {
        yield return new WaitForSeconds(markerduration);
        Destroy(gameObject);
    }


}
