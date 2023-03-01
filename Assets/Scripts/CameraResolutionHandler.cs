using UnityEngine;

public class CameraResolutionHandler : MonoBehaviour
{
    public float referenceResolutionHeight = 1920f;
    public float referenceOrthoSize = 5f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        float screenRatio = (float)Screen.height / (float)referenceResolutionHeight;
        cam.orthographicSize = referenceOrthoSize * screenRatio;
    }

    private void Update()
    {
        float screenRatio = (float)Screen.height / (float)referenceResolutionHeight;
        cam.orthographicSize = referenceOrthoSize * screenRatio;
    }
}