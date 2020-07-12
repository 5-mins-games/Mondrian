using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float scaleSpeed = 5.0f;
    private float minScale = 7.7f;

    private float maxScale = 11.2f;

    private float currentScale;

    private int transTime = 200;

    private int i = 0;

    void Start () 
    {

        //根据当前摄像机是正交还是透视进行对应赋值

        if (Camera.main.orthographic == true)
        {
            currentScale = Camera.main.orthographicSize;
        }
        else
        {
            currentScale = Camera.main.fieldOfView;


        } 
    }

// Update is called once per frame

void Update () {
        if(i<transTime)
        {
            currentScale -= 0.019f;
            this.transform.Translate(0.0065f,0.0187f,0,Space.World);  
            i += 1;
        }
        
        currentScale -= Input.GetAxis("Mouse ScrollWheel") * scaleSpeed;

        currentScale = Mathf.Clamp(currentScale, minScale, maxScale);

        //根据当前摄像机是正交还是透视进行对应赋值，放大缩小

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
