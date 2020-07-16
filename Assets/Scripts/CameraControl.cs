using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float scaleSpeed = 5.0f;

    private float minScale = 7.7f;
    private float maxScale = 11.2f;

    private float currentScale;

    void Start() 
    {
        if (Camera.main.orthographic == true)
        {
            currentScale = Camera.main.orthographicSize;
        }
        else
        {
            currentScale = Camera.main.fieldOfView;
        }
    }

    void Update()
    {        
        currentScale -= Input.GetAxis("Mouse ScrollWheel") * scaleSpeed;

        currentScale = Mathf.Clamp(currentScale, minScale, maxScale);

        if (Camera.main.orthographic == true)
        {
            Camera.main.orthographicSize = currentScale;
        }
        else 
        {
            Camera.main.fieldOfView = currentScale;
        }
    }
}
